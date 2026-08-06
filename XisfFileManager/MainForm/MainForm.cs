using System.ComponentModel;
using System.Reflection;
using Velopack;
using Velopack.Sources;
using XisfFileManager.Calculations;
using XisfFileManager.Configuration;
using XisfFileManager.Globals;
using XisfFileManager.Files;
using XisfFileManager.Models;

namespace XisfFileManager
{
    // Event used to update the Calibration Tab Page Text Boxes and Progress Bar
    public delegate void DataReceivedEvent(CalibrationTabPageValues data);

    // ##########################################################################################################################
    // ##########################################################################################################################

    //[DesignerCategory("Form")]
    public partial class MainForm : Form
    {
        private readonly Workspace mWorkspace = new();

        // The four members below are the session's shared mutable data. They live on
        // mWorkspace and are exposed here as shim properties so existing call sites
        // (mFileList, mFile, ImageParameterLists, mDirectoryProperties) keep working.
        private List<XisfFile> mFileList => mWorkspace.Files;
        private XisfFile mFile { get => mWorkspace.CurrentFile!; set => mWorkspace.CurrentFile = value; }
        private ImageCalculations ImageParameterLists => mWorkspace.ImageParameters;
        private DirectoryProperties mDirectoryProperties => mWorkspace.DirectoryProperties;

        private readonly Calibration mCalibration;
        private readonly XisfXmlReader mXmlReader;
        private readonly XisfFileRename mRenameFile;
        private string mFolderBrowseState = string.Empty;
        private bool mBCancel;
        private readonly XisfFileUpdate mXisfFileUpdate;
        private eKeywordUpdateMode mKeywordUpdateProtection;
        private eUiState mUiState;

        // ##########################################################################################################################
        // Constructor
        // ##########################################################################################################################
        public MainForm()
        {
            InitializeComponent();
            CalibrationTabPageEvent.CalibrationTabPage_InvokeEvent += EventHandler_UpdateCalibrationPageForm;

            mCalibration = new Calibration();
            mXmlReader = new XisfXmlReader();
            mXisfFileUpdate = new XisfFileUpdate();
            mKeywordUpdateProtection = eKeywordUpdateMode.UPDATE_NEW;
            Label_FileSelection_Statistics_OperationStatus.Text = "";
            mRenameFile = new XisfFileRename
            {
                RenameOrder = eOrder.INDEX
            };

            Label_FileSelection_Statistics_OperationStatus.Text = "No Images Selected";
            Label_FileSelection_Statistics_TempratureCoefficient.Text = "Temperature Coefficient: Not Computed";

            string version = (Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? "?").Split('+')[0];
            this.Text = $"XISF File Manager {version}";


            Utility.ToolTips.AddToolTip(RadioButton_FileSelection_Index_ByFilter, "Orders Files by Capture Time per Filter", "\"By Target\" orders each filter's files consecutively.\r\n\"By Night\" orders each filter's files consecutively by night.");
            Utility.ToolTips.AddToolTip(RadioButton_FileSelection_Index_ByTime, "Orders Files by Capture Time", "\"By Target\" orders all files consecutively.\r\n\"By Night\" orders all files consecutively by night.");
            UpdateUI(eUiState.DISABLED);
        }

        // ****************************************************************************************************************
        // ************************ Event Handlers ************************************************************************
        // ****************************************************************************************************************
        private void EventHandler_UpdateCalibrationPageForm(CalibrationTabPageValues data)
        {
            ProgressBar_CalibrationTab.Maximum = data.ProgressMax;
            ProgressBar_CalibrationTab.Value = data.Progress;
            Label_CalibrationTab_ReadFileName.Text = data.FileName;
            Label_CalibrationTab_TotalFiles.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            Label_CalibrationTab_TotalFiles.Text = "Found " + data.TotalFiles.ToString() + " Library Files";

            switch (data.MessageMode)
            {
                case eMessageMode.CLEAR:
                    TextBox_CalibrationTab_Messgaes.Clear();
                    break;

                case eMessageMode.APPEND:
                    TextBox_CalibrationTab_Messgaes.AppendText(data.MatchCalibrationMessage);
                    break;

                case eMessageMode.NEW:
                    TextBox_CalibrationTab_Messgaes.Clear();
                    TextBox_CalibrationTab_Messgaes.AppendText(data.MatchCalibrationMessage);
                    break;

                default:
                    break;

            }
            data.MessageMode = eMessageMode.KEEP;

            TextBox_CalibrationTab_MatchingTolerance_Exposure.Text = mCalibration.ExposureTolerance.ToString();
            TextBox_CalibrationTab_MatchingTolerance_Gain.Text = mCalibration.GainTolerance.ToString();
            TextBox_CalibrationTab_MatchingTolerance_Offset.Text = mCalibration.OffsetTolerance.ToString();
            TextBox_CalibrationTab_MatchingTolerance_Temperature.Text = mCalibration.TemperatureTolerance.ToString();

            TabPage_Calibration.Update();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            mFolderBrowseState = Properties.Settings.Default.Persist_FolderBrowseState;
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName.Checked = Properties.Settings.Default.Persist_UpdateTargetNameState;
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName.Checked = Properties.Settings.Default.Persist_UpdatePanelNameState;

            await CheckForUpdatesAsync();
        }

        private static async Task CheckForUpdatesAsync()
        {
            try
            {
                var mgr = new UpdateManager(new GithubSource("https://github.com/Apoplectic1/XisfFileManager", null, false));
                var updateInfo = await mgr.CheckForUpdatesAsync();

                if (updateInfo != null)
                {
                    var result = MessageBox.Show(
                        $"Version {updateInfo.TargetFullRelease.Version} is available. Update now?",
                        "Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        await mgr.DownloadUpdatesAsync(updateInfo);
                        mgr.ApplyUpdatesAndRestart(updateInfo);
                    }
                }
            }
            catch (Exception)
            {
                // Silently ignore update check failures (no network, no releases yet, etc.)
            }
        }

        [Obsolete("Overrides Form.OnClosing which is obsolete. Use OnFormClosing instead.")]
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Properties.Settings.Default.Persist_FolderBrowseState = mFolderBrowseState;
            Properties.Settings.Default.Persist_UpdateTargetNameState = CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName.Checked;
            Properties.Settings.Default.Persist_UpdatePanelNameState = CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName.Checked;

            Properties.Settings.Default.Save();
        }

        // ##########################################################################################################################
        // ##########################################################################################################################

        private async void Button_Browse_Click(object sender, EventArgs e)
        {
            ResetSession();

            // Solver checked but ASTAP absent: refuse the browse up front (fail fast), never skip silently.
            if (CheckBox_Solver.Checked && !Solver.AstapSolver.IsInstalled)
            {
                MessageBox.Show(
                    "ASTAP solver not found at:\n\n" + Configuration.XisfConstants.AstapCliPath +
                    "\n\nInstall ASTAP (with a star database) or uncheck Solver.",
                    "Plate Solver Not Found");
                UpdateUI(eUiState.DISABLED);
                return;
            }

            if (!TrySelectSourceFolder())
                return;

            await ReadHeadersAsync();

            PopulateUiFromFiles();

            RefreshFeatureDetection();

            BuildTargetFileTree();

            // UI Updates
            UpdateUI(eUiState.ENABLED);
        }

        // Browse pipeline stages, executed in order by Button_Browse_Click above.

        private void ResetSession()
        {
            // Clear all lists - we are reading or re-reading what will become a new xisf file data set that will invalidate any existing data.
            // 
            mBCancel = false;
            mWorkspace.Clear();
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.Items.Clear();
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.Text = "Keyword";
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue.Items.Clear();
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue.Text = "Value";
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.Text = "";
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.Items.Clear();
            TextBox_CalibrationTab_Messgaes.Clear();
            TreeView_CalibrationTab_TargetFileTree.Nodes.Clear();

            mCalibration.ResetAll();

            ClearCaptureSoftwareGroup();
            ClearTelescopeGroup();
            ClearCameraGroup();
            ClearFilterFrameTypeGroup();

            ProgressBar_FileSelection_ReadProgress.Value = 0;
            ProgressBar_KeywordUpdateTab_WriteProgress.Value = 0;
        }

        private bool TrySelectSourceFolder()
        {
            // Exclude List
            // This list can contain any number of strings that will be used to exclude any full path (including a specified file name)
            // that contains the string below the selected folder.
            List<string> mExcludeList = DirectoryFilters.BrowseExcludes;

            // remove "Master" from the exclude list if the Masters checkbox is checkedc because we are processing masters
            if (CheckBox_FileSelection_DirectorySelection_Masters_Enable.Checked)
            {
                mExcludeList.Remove("Master");
                mExcludeList.Remove("Calibration");
            }

            // Recurese into subdirectories
            Files.DirectoryOperations.Recurse = CheckBox_FileSelection_DirectorySelection_Recurse.Checked;

            // Open a dialog to select a folder
            DialogResult result = Files.DirectoryOperations.FindTargetFilesDialog(mFolderBrowseState, mExcludeList, ExcludeType.Contains);

            if ((result != DialogResult.OK) || (Files.DirectoryOperations.FileInfoList.Count == 0))
            {
                UpdateUI(eUiState.DISABLED);
                MessageBox.Show("No Xisf Files Found", "Select a different folder");
                return false;
            }

            mFolderBrowseState = Files.DirectoryOperations.SelectedFolder;
            return true;
        }

        private async Task ReadHeadersAsync()
        {
            Label_FileSelection_Statistics_OperationStatus.Text = "Reading " + Files.DirectoryOperations.FileInfoList.Count.ToString() + " Image Files";
            Label_FileSelection_Statistics_TempratureCoefficient.Text = "Temperature Coefficient: Not Computed";
            Label_FileSelection_Statistics_SubFrameOverhead.Text = "SubFrame Overhead: Not Computed";

            ProgressBar_FileSelection_ReadProgress.Value = 0;
            ProgressBar_FileSelection_ReadProgress.Maximum = Files.DirectoryOperations.FileInfoList.Count;


            // Plate solving rides the read pass (astap-plate-solve): all light frames, masters excluded
            // (their own checkbox path); solved values land in the in-memory KeywordList and persist
            // through the normal save step.
            bool solverEnabled = CheckBox_Solver.Checked
                && !CheckBox_FileSelection_DirectorySelection_Masters_Enable.Checked;
            int solvedCount = 0;
            List<string> solveFailures = new();

            foreach (FileInfo xFile in Files.DirectoryOperations.FileInfoList)
            {
                Label_FileSelection_BrowseFileName.Text = xFile.DirectoryName + "\n" + xFile.Name;
                ProgressBar_FileSelection_ReadProgress.Value += 1;

                // Create a new xisf file instance
                mFile = new XisfFile
                {
                    FilePath = xFile.FullName
                };

                await mXmlReader.ReadXisfFileHeaderKeywords(mFile);

                if (solverEnabled && mFile.FrameType == eFrame.LIGHT)
                {
                    Label_FileSelection_BrowseFileName.Text = xFile.DirectoryName + "\nSolving: " + xFile.Name;
                    Solver.SolveResult solution = await Solver.AstapSolver.SolveAsync(mFile.FilePath, mFile.IsImageCompressed);
                    if (solution.Success)
                    {
                        mFile.KeywordList.SetPlateSolution(solution);
                        solvedCount++;
                    }
                    else
                    {
                        solveFailures.Add(xFile.Name + " — " + solution.ErrorText);
                    }
                }

                mFileList.Add(mFile);
            }

            mFileList.Sort((a, b) => a.CaptureTime.CompareTo(b.CaptureTime)); // oldest is first

            if (solverEnabled)
            {
                Label_FileSelection_Statistics_OperationStatus.Text =
                    $"Read {mFileList.Count} Image Files; Solved {solvedCount}" +
                    (solveFailures.Count > 0 ? $", {solveFailures.Count} failed" : "");
                if (solveFailures.Count > 0)
                {
                    MessageBox.Show(
                        $"Solved {solvedCount} of {solvedCount + solveFailures.Count} light frames.\n\nFailed:\n\n"
                        + string.Join("\n", solveFailures),
                        "Plate Solve Results");
                }
            }
        }

        private void PopulateUiFromFiles()
        {
            if (CheckBox_FileSelection_DirectorySelection_Masters_Enable.Checked)
            {
                TextBox_FileSelection_DirectorySelection_Masters_Frames.Text = mFileList[0].MSTRFRMS.ToString();
                TextBox_FileSelection_DirectorySelection_Masters_Rejection.Text = mFileList[0].MSTRALG;
            }
            else
            {
                TextBox_FileSelection_DirectorySelection_Masters_Frames.Text = "Frames";
                TextBox_FileSelection_DirectorySelection_Masters_Rejection.Text = "Algo";
            }

            // **********************************************************************
            // Get TargetName and and Weights to populate ComboBoxes

            // First get a list of all the target names found in the source files, then find unique names and sort.
            // Place culled list in the target name combobox
            List<string> targetNameList = new();
            List<string> weightKeywordList = new();

            foreach (XisfFile file in mFileList)
            {
                targetNameList.Add(file.TargetName);
            }

            targetNameList = targetNameList.Distinct().ToList();
            targetNameList = targetNameList.OrderBy(q => q).ToList();

            // Add the target names to the combobox
            foreach (string item in targetNameList)
            {
                ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.Items.Add(item);
            }

            // Select the first item in the combobox
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.SelectedIndex = 0;


            if (targetNameList.Count <= 1)
            {
                // Single name or blank
                Label_KeywordUpdateTab_SubFrameKeywords_TagetName.ForeColor = Color.Black;
            }
            else
            {
                // If target names are not unique, check for pairs
                Dictionary<string, int> matchCounts = new Dictionary<string, int>();
                foreach (string item in targetNameList)
                {
                    string baseItem = item.EndsWith(" stars") ? item.Substring(0, item.Length - 6) : item;

                    if (matchCounts.TryGetValue(baseItem, out int value))
                        matchCounts[baseItem] = ++value;
                    else
                        matchCounts[baseItem] = 1;
                }

                bool bGreen = true;

                foreach (int count in matchCounts.Values)
                {
                    if (count == 1)
                    {
                        // Rule for items without a pair
                        Label_KeywordUpdateTab_SubFrameKeywords_TagetName.ForeColor = Color.DarkViolet;
                        bGreen = false;
                        break;
                    }
                    else if (count != 2)
                    {
                        // Rule for items that do not form exact pairs
                        Label_KeywordUpdateTab_SubFrameKeywords_TagetName.ForeColor = Color.Red;
                        bGreen = false;
                        break;
                    }
                }

                if (bGreen)
                    Label_KeywordUpdateTab_SubFrameKeywords_TagetName.ForeColor = Color.Green;
            }


            // Now find a list of any present weight keywords (not values). Find unique Keyords, sort and populate Weight combobox
            foreach (XisfFile file in mFileList)
            {
                weightKeywordList = file.WeightKeyword;
            }

            if (weightKeywordList.Count > 0)
            {
                weightKeywordList = weightKeywordList.Distinct().ToList();
                weightKeywordList = weightKeywordList.OrderBy(q => q).ToList();

                foreach (string item in weightKeywordList)
                {
                    ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.Items.Add(item);
                }

                if (weightKeywordList.Count > 1)
                {
                    Label_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.ForeColor = Color.Red;
                }
                else
                {
                    Label_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.ForeColor = Color.Black;
                }

                ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.SelectedIndex = 0;
            }
            else
            {
                ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.Items.Clear();
                Label_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.ForeColor = Color.Black;
            }



            // Now make a list of all Keywords found in ALL files. Sort and populate comboBox
            List<string> keywordNamelist = new();

            foreach (XisfFile xFile in mFileList)
            {
                ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile.Items.Add(Path.GetFileName(xFile.FilePath));

                foreach (Keyword keywordName in xFile.KeywordList.mKeywordList)
                {
                    keywordNamelist.Add(keywordName.Name);
                }
            }

            keywordNamelist.Sort();
            keywordNamelist = keywordNamelist.Distinct().ToList();

            foreach (string name in keywordNamelist)
            {
                ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.Items.Add(name);
            }

            // **********************************************************************


            // **********************************************************************
            // Calculate Image paramters for UI
            foreach (XisfFile xFile in mFileList)
            {
                if (xFile.FilePath == string.Empty)
                    xFile.AddKeyword("FILENAME", "Original Name", Path.GetFileName(xFile.FilePath));

                ImageParameterLists.BuildImageParameterValueLists(xFile);
            }

            string readSummary = Files.DirectoryOperations.FileInfoList.Count == mFileList.Count
                ? "Read all " + mFileList.Count.ToString() + " Image Files"
                : "Read " + mFileList.Count.ToString() + " out of " + Files.DirectoryOperations.FileInfoList.Count + " Image Files";

            int compressedCount = mFileList.Count(file => file.IsImageCompressed);
            int uncompressedCount = mFileList.Count - compressedCount;
            Label_FileSelection_Statistics_OperationStatus.Text =
                readSummary + "\n" + compressedCount + " Compressed " + uncompressedCount + " Uncompressed";

            Label_FileSelection_Statistics_SubFrameOverhead.Text = ImageCalculations.CalculateOverhead(mFileList);
            string stepsPerDegree = ImageCalculations.CalculateFocuserTemperatureCompensationCoefficient(mFileList);
            Label_FileSelection_Statistics_TempratureCoefficient.Text = "Temperature Coefficient: " + stepsPerDegree;
        }

        private void RefreshFeatureDetection()
        {

            // **********************************************************************

            FindCaptureSoftware();
            FindFilterFrameType();
            FindTelescope();
            FindCamera();
        }

        private void BuildTargetFileTree()
        {

            // **********************************************************************

            // TreeView_CalibrationTab_Dates

            // Create the TreeView

            TreeView_CalibrationTab_TargetFileTree.Nodes.Clear();

            IOrderedEnumerable<IGrouping<string, XisfFile>> groupedByTargetName = mFileList.GroupBy(item => item.TargetName).OrderBy(group => group.Key);

            // Create the hierarchical TreeView
            foreach (IGrouping<string, XisfFile> targetGroup in groupedByTargetName)
            {
                TreeNode targetNode = new TreeNode(targetGroup.Key);
                TreeView_CalibrationTab_TargetFileTree.Nodes.Add(targetNode);

                // Group the items by Camera
                IOrderedEnumerable<IGrouping<string, XisfFile>> groupedByCamera = targetGroup.GroupBy(item => item.Camera).OrderBy(group => group.Key);

                foreach (IGrouping<string, XisfFile> cameraGroup in groupedByCamera)
                {
                    TreeNode cameraNode = new TreeNode(cameraGroup.Key);
                    targetNode.Nodes.Add(cameraNode);

                    // Group the item by ExposureSeconds
                    IOrderedEnumerable<IGrouping<double, XisfFile>> groupedByExposureSeconds = cameraGroup.GroupBy(item => item.ExposureSeconds).OrderByDescending(group => group.Key);

                    foreach (IGrouping<double, XisfFile> exposureGroup in groupedByExposureSeconds)
                    {
                        TreeNode exposureNode = new TreeNode(exposureGroup.Key.ToString());
                        cameraNode.Nodes.Add(exposureNode);

                        // Group the items by Filter
                        IOrderedEnumerable<IGrouping<string, XisfFile>> groupedByFilter = exposureGroup.GroupBy(item => item.FilterName).OrderBy(group => group.Key);

                        foreach (IGrouping<string, XisfFile> filterGroup in groupedByFilter)
                        {
                            TreeNode filterNode = new TreeNode($"{filterGroup.Key} - {filterGroup.Count()} files");
                            exposureNode.Nodes.Add(filterNode);
                        }
                    }
                }
            }

            ExpandAllNodes(TreeView_CalibrationTab_TargetFileTree.Nodes);
        }

        /// <summary>
        /// Recursively expands all nodes in the specified tree node collection.
        /// </summary>
        /// <param name="nodes">The collection of tree nodes to expand.</param>
        private static void ExpandAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                // Expand the current node
                node.Expand();

                // Recursively expand all child nodes
                ExpandAllNodes(node.Nodes);
            }
        }

        private async void Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords_Click(object sender, EventArgs e)
        {
            if (RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect.Checked)
                return;

            bool bStatus;
            GroupBox_FileSelection.Enabled = false;
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Enabled = false;
            GroupBox_KeywordUpdateTab_CaptureSoftware.Enabled = false;
            GroupBox_KeywordUpdateTab_Telescope.Enabled = false;
            GroupBox_KeywordUpdateTab_Camera.Enabled = false;
            GroupBox_KeywordUpdateTab_ImageType.Enabled = false;
            ProgressBar_KeywordUpdateTab_WriteProgress.Value = 0;
            ProgressBar_KeywordUpdateTab_WriteProgress.Maximum = mFileList.Count;

            // If multiple Targets or if a Target has multiple Panels do not update with the ComboBox Text
            List<string> targetNames = new List<string>();
            targetNames.Clear();
            foreach (string target in ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.Items)
            {
                // Remove " Stars" from targetName so there is a single target name for the next foreach below (" Stars" will be added there)
                string targetName = target.Replace(" Stars", "");
                targetNames.Add(targetName.Trim());
            }
            targetNames = targetNames.Distinct().ToList();


            int count = 0;
            int compressedCount = 0;
            int verbatimCount = 0;
            int unchangedCount = 0;
            foreach (XisfFile xFile in mFileList)
            {
                xFile.KeywordUpdateMode = mKeywordUpdateProtection;
                if (xFile.KeywordUpdateMode == eKeywordUpdateMode.PROTECT)
                    return;

                if (mBCancel) { mBCancel = false; return; }

                xFile.SetObservationSite();
                xFile.KeepPanel = CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName.Checked;

                // Update with ComboBox Text if checked
                if (CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName.Checked)
                    // Rename everything to the ComboBox Text value
                    xFile.TargetName = ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.Text;

                ProgressBar_KeywordUpdateTab_WriteProgress.Value += 1;

                // The label is updated from inside UpdateFileAsync (via ShowFileBeingWritten) only when a write
                // actually happens, so it reflects the file being written — not files skipped by the save gate.
                bStatus = await mXisfFileUpdate.UpdateFileAsync(xFile, xFile.FilePath, ShowFileBeingWritten);

                if (bStatus == false)
                {
                    Label_FileSelection_Statistics_OperationStatus.Text = "File Write Error";

                    DialogResult result = MessageBox.Show(
                        "File Update Failed - Protected or I/O Error.\n\n" + Label_KeywordUpdateTab_FileName.Text,
                        "\nMainForm.cs Button_Update_Click()",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Error);

                    // if Cancel, exit application
                    if (result == DialogResult.Cancel)
                    {
                        GroupBox_FileSelection.Enabled = true;
                        GroupBox_KeywordUpdateTab_SubFrameKeywords.Enabled = true;
                        GroupBox_KeywordUpdateTab_CaptureSoftware.Enabled = true;
                        GroupBox_KeywordUpdateTab_Telescope.Enabled = true;
                        GroupBox_KeywordUpdateTab_Camera.Enabled = true;
                        GroupBox_KeywordUpdateTab_ImageType.Enabled = true;
                        return;
                    }

                    GroupBox_FileSelection.Enabled = true;
                    GroupBox_KeywordUpdateTab_SubFrameKeywords.Enabled = true;
                    GroupBox_KeywordUpdateTab_CaptureSoftware.Enabled = true;
                    GroupBox_KeywordUpdateTab_Telescope.Enabled = true;
                    GroupBox_KeywordUpdateTab_Camera.Enabled = true;
                    GroupBox_KeywordUpdateTab_ImageType.Enabled = true;
                    return;
                }

                count++;

                switch (mXisfFileUpdate.LastUpdateOutcome)
                {
                    case eUpdateOutcome.Compressed: compressedCount++; break;
                    case eUpdateOutcome.AlreadyCompressed: verbatimCount++; break;
                    case eUpdateOutcome.Skipped: unchangedCount++; break;
                }
            }

            int writtenCount = compressedCount + verbatimCount;
            Label_FileSelection_Statistics_OperationStatus.Text =
                $"{writtenCount} Images Updated · {compressedCount} compressed · {verbatimCount} already-compressed" +
                (unchangedCount > 0 ? $" · {unchangedCount} unchanged" : string.Empty);
            GroupBox_FileSelection.Enabled = true;
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Enabled = true;
            GroupBox_KeywordUpdateTab_CaptureSoftware.Enabled = true;
            GroupBox_KeywordUpdateTab_Telescope.Enabled = true;
            GroupBox_KeywordUpdateTab_Camera.Enabled = true;
            GroupBox_KeywordUpdateTab_ImageType.Enabled = true;


            FindFilterFrameType(); // Update UI - NOT SURE WHY I NEED THIS HERE
        }

        // Shows the file currently being written (keyword update and/or compression). Forces an immediate
        // repaint because the compress step runs synchronously on the UI thread, which would otherwise defer
        // the label paint until after the write completes.
        private void ShowFileBeingWritten(string path)
        {
            Label_KeywordUpdateTab_FileName.Text = Path.GetDirectoryName(path) + "\n" + Path.GetFileName(path);
            Label_KeywordUpdateTab_FileName.Update();
        }


        private void Button_KeywordUpdateTab_Cancel_Click(object sender, EventArgs e)
        {
            mBCancel = true;
        }
        // ************************************************************
        // Update UI
        private void UpdateUI(eUiState eState)
        {
            mUiState = eState;

            switch (eState)
            {
                case eUiState.DISABLED:
                    CheckBox_FileSelection_DirectorySelection_Masters_Enable.Enabled = true;
                    TextBox_FileSelection_DirectorySelection_Masters_Frames.Enabled = false;
                    TextBox_FileSelection_DirectorySelection_Masters_Rejection.Enabled = false;
                    Button_FileSelection_DirectorySelection_Rename.Enabled = false;
                    CheckBox_FileSelection_DirectorySelection_CalibrationIds.Enabled = false;
                    CheckBox_FileSlection_DirectorySelection_NoStatistics.Enabled = false;
                    GroupBox_FileSelection_SequenceNumbering.Enabled = false;
                    GroupBox_FileSelection_Statistics.Enabled = false;
                    Label_FileSelection_Statistics_OperationStatus.Text = "Operation Status: Idle";
                    Label_FileSelection_Statistics_SubFrameOverhead.Text = "SubFrame Overhead: Not Computed";
                    Label_FileSelection_Statistics_TempratureCoefficient.Text = "Temperature Coefficient: Not Computed";
                    Label_FileSelection_BrowseFileName.Text = "No Files Selected";
                    break;

                case eUiState.ENABLED:
                    CheckBox_FileSelection_DirectorySelection_Masters_Enable.Enabled = true;
                    TextBox_FileSelection_DirectorySelection_Masters_Frames.Enabled = true;
                    TextBox_FileSelection_DirectorySelection_Masters_Rejection.Enabled = true;
                    Button_FileSelection_DirectorySelection_Rename.Enabled = true;
                    CheckBox_FileSelection_DirectorySelection_CalibrationIds.Enabled = true;
                    CheckBox_FileSlection_DirectorySelection_NoStatistics.Enabled = true;
                    GroupBox_FileSelection_SequenceNumbering.Enabled = true;
                    GroupBox_FileSelection_Statistics.Enabled = true;
                    break;

                case eUiState.RENAME:
                    CheckBox_FileSelection_DirectorySelection_Masters_Enable.Enabled = false;
                    TextBox_FileSelection_DirectorySelection_Masters_Frames.Enabled = false;
                    TextBox_FileSelection_DirectorySelection_Masters_Rejection.Enabled = false;
                    Button_FileSelection_DirectorySelection_Rename.Enabled = false;
                    CheckBox_FileSelection_DirectorySelection_CalibrationIds.Enabled = false;
                    CheckBox_FileSlection_DirectorySelection_NoStatistics.Enabled = false;
                    GroupBox_FileSelection_SequenceNumbering.Enabled = false;
                    GroupBox_FileSelection_Statistics.Enabled = true;
                    Label_FileSelection_BrowseFileName.Text = "No Files Selected";
                    break;
            }
        }

        bool noStaticsState;
        private void CheckBox_FileSelection_DirectorySelection_EnableFluxDensity_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable.Checked)
            {
                noStaticsState = CheckBox_FileSlection_DirectorySelection_NoStatistics.Checked;
                Button_FileSelection_DirectorySelection_Rename.Enabled = false;
                CheckBox_FileSelection_DirectorySelection_CalibrationIds.Enabled = false;

                Button_FileSelection_DirectorySelection_FluxDensity_Run.Enabled = true;
                CheckBox_FileSlection_DirectorySelection_NoStatistics.Checked = true;

            }
            else
            {
                Button_FileSelection_DirectorySelection_Rename.Enabled = true;
                CheckBox_FileSelection_DirectorySelection_CalibrationIds.Enabled = true;

                Button_FileSelection_DirectorySelection_FluxDensity_Run.Enabled = false;
                CheckBox_FileSlection_DirectorySelection_NoStatistics.Checked = noStaticsState;
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Placeholder for future use
        }

        private void TabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // Block tab switches when no files are loaded
            if (mUiState != eUiState.ENABLED)
            {
                e.Cancel = true;
            }
        }

        private async void Button_KeywordUpdateTab_SubFrameKeywords_SetupFluxDensity_Click(object sender, EventArgs e)
        {
            await SetupFluxDensity();
        }

    }
}