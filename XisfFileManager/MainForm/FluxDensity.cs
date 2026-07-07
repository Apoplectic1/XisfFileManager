using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;
using XisfFileManager.Configuration;
using XisfFileManager.Files;
using XisfFileManager.Globals;

namespace XisfFileManager
{
    public partial class MainForm
    {
        private async Task ResetFluxDensityState()
        {
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

            Label_FileSelection_Statistics_TempratureCoefficient.Text = "Temperature Coefficient: Not Computed";
            Label_FileSelection_Statistics_SubFrameOverhead.Text = "SubFrame Overhead: Not Computed";

            ProgressBar_FileSelection_ReadProgress.Value = 0;
            ProgressBar_KeywordUpdateTab_WriteProgress.Value = 0;

            await Task.Yield();
        }

        private async Task<bool> ReadFluxDensityFilesAsync(string directory, List<string> excludeList, ExcludeType excludeType)
        {
            DialogResult result = Files.DirectoryOperations.FindTargetFilesString(directory, excludeList, excludeType);

            if ((result != DialogResult.OK) || (Files.DirectoryOperations.FileInfoList.Count == 0))
            {
                mFolderBrowseState = string.Empty;
            }
            else
            {
                mFolderBrowseState = Files.DirectoryOperations.SelectedFolder;
            }

            if (mFolderBrowseState == string.Empty)
                return false;

            Label_FileSelection_Statistics_OperationStatus.Text = "Reading " + Files.DirectoryOperations.FileInfoList.Count.ToString() + " Image Files";
            Label_FileSelection_Statistics_TempratureCoefficient.Text = "Temperature Coefficient: Not Computed";
            Label_FileSelection_Statistics_SubFrameOverhead.Text = "SubFrame Overhead: Not Computed";

            ProgressBar_FileSelection_ReadProgress.Value = 0;
            ProgressBar_FileSelection_ReadProgress.Maximum = Files.DirectoryOperations.FileInfoList.Count;

            foreach (FileInfo file in Files.DirectoryOperations.FileInfoList)
            {
                if (mBCancel)
                    break;

                Label_FileSelection_BrowseFileName.Text = file.DirectoryName + "\n" + file.Name;
                ProgressBar_FileSelection_ReadProgress.Value += 1;

                mFile = new XisfFile
                {
                    FilePath = file.FullName
                };

                await mXmlReader.ReadXisfFileHeaderKeywords(mFile);

                mFileList.Add(mFile);
            }

            FindCaptureSoftware();
            FindFilterFrameType();
            FindTelescope();
            FindCamera();
            await Task.Yield();

            return true;
        }

        private async Task<bool> WriteFilesToFluxDensityAsync()
        {
            ProgressBar_KeywordUpdateTab_WriteProgress.Value = 0;
            ProgressBar_KeywordUpdateTab_WriteProgress.Maximum = mFileList.Count;

            foreach (XisfFile xFile in mFileList)
            {
                if (mBCancel)
                    break;

                ProgressBar_KeywordUpdateTab_WriteProgress.Value += 1;

                Keyword? panel = xFile.GetKeyword("CPANEL");
                Keyword? stars = xFile.GetKeyword("CSTARS");
                Keyword? filter = xFile.GetKeyword("FILTER");

                string outputDir = AppPaths.FluxDensityDir + @"\";

                if (panel != null)
                    outputDir += panel.Value + @"\";

                if (stars != null)
                    outputDir += stars.Value + @"\" + filter?.Value + @"\";
                else
                    outputDir += filter?.Value + @"\";

                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                Label_KeywordUpdateTab_FileName.Text = outputDir + "\n" + System.IO.Path.GetFileName(xFile.FilePath);

                string outputPath = outputDir + System.IO.Path.GetFileName(xFile.FilePath);

                // Deliberate write to a separate export copy — declare intent so the PROTECT gate
                // inside UpdateFileAsync doesn't silently skip the export.
                xFile.KeywordUpdateMode = eKeywordUpdateMode.FORCE;
                var bStatus = await mXisfFileUpdate.UpdateFileAsync(xFile, outputPath);

                if (bStatus == false)
                {
                    Label_FileSelection_Statistics_OperationStatus.Text = "File Write Error";

                    MessageBox.Show(
                        "File Update Failed - Protected or I/O Error.\n\n" + Label_KeywordUpdateTab_FileName.Text,
                        "\nMainForm.cs Button_Update_Click()",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
                }
            }

            return true;
        }

        public async Task SetupFluxDensity()
        {
            string fluxDir = AppPaths.FluxDensityDir;
            if (Directory.Exists(fluxDir))
            {
                Directory.Delete(fluxDir, true);
            }

            Files.DirectoryOperations.Recurse = CheckBox_FileSelection_DirectorySelection_Recurse.Checked;

            // Pass 1: Debayered files
            await ResetFluxDensityState();
            List<string> emptyExcludes = new();
            if (await ReadFluxDensityFilesAsync(AppPaths.DebayeredDir, emptyExcludes, ExcludeType.None))
            {
                Button_KeywordImageTypeFrame_SetAll_Click(this, EventArgs.Empty);
                if (!await WriteFilesToFluxDensityAsync()) return;
            }

            // Pass 2: Target files (cosmetized or calibrated)
            await ResetFluxDensityState();
            string targetDir = Directory.Exists(AppPaths.CosmetizedDir) ? AppPaths.CosmetizedDir : AppPaths.CalibratedDir;
            if (await ReadFluxDensityFilesAsync(targetDir, DirectoryFilters.FluxDensityCfaExcludes, ExcludeType.Contains))
            {
                if (!await WriteFilesToFluxDensityAsync()) return;
            }
        }
    }
}
