using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XisfFileManager.Globals;
using XisfFileManager.Files;

namespace XisfFileManager
{
    public partial class MainForm
    {
        /// <summary>
        /// Sets the file index for each XisfFile in the list based on either time or filter type.
        /// If bTime is true, the files are sequentially numbered based on time.
        /// If bTime is false, the files are sequentially numbered within each directory group based on their filter type.
        /// </summary>
        /// <param name="bTime">A boolean flag indicating whether to index files based on time (true) or filter type (false).</param>
        public void SetFileIndex(bool bTime)
        {
            if (bTime)
            {
                // Sequentially number files based on time
                mFileList
                    .Select((xFile, index) => new { xFile, index })
                    .ToList()
                    .ForEach(item => item.xFile.FileNameNumberIndex = item.index + 1);
                return;
            }

            // Sequentially number filter image files within each directory group
            // Note this depends on the directory statistics being set
            foreach (var group in mDirectoryProperties.DirectoryStatistics)
            {
                // Initialize filter indices for each filter type
                var filterIndices = new Dictionary<string, int>
                    {
                        { "L", 0 }, { "R", 0 }, { "G", 0 }, { "B", 0 },
                        { "H", 0 }, { "O", 0 }, { "S", 0 }, { "Shutter", 0 }
                    };

                // Filter files by directory group and update the index based on filter type
                mFileList
                    .Where(xFile => xFile.FilePath.Contains(group.Key))
                    .ToList()
                    .ForEach(xFile =>
                    {
                        // Update the index for the corresponding filter type
                        if (filterIndices.ContainsKey(xFile.FilterName))
                        {
                            xFile.FileNameNumberIndex = ++filterIndices[xFile.FilterName];
                        }
                    });
            }
        }


        /// <summary>
        /// Handles the click event for the file selection rename button.
        /// Renames the images in the file list based on selected indexing criteria (by filter or by time),
        /// updates progress bars, and displays status messages. Manages duplicate files and updates UI accordingly.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private async void Button_FileSelection_DirectorySelection_Rename_Click(object sender, EventArgs e)
        {
            bool bFilter = RadioButton_FileSelection_Index_ByFilter.Checked;
            bool bTime = RadioButton_FileSelection_Index_ByTime.Checked;

            Label_FileSelection_Statistics_OperationStatus.Text = "Renaming " + mFileList.Count.ToString() + " Images";

            ProgressBar_KeywordUpdateTab_WriteProgress.Maximum = mFileList.Count;
            ProgressBar_KeywordUpdateTab_WriteProgress.Value = 0;

            int duplicates = XisfFileRename.MoveDuplicates(mFileList);

            // Do not consider directory statistics if we are dealing with Master frames
            if (!CheckBox_FileSelection_DirectorySelection_Masters_Enable.Checked)
            {
                // Set or remove directory file statistics
                mDirectoryProperties.SetDirectoryFileStatistics(mFileList, CheckBox_FileSlection_DirectorySelection_NoStatistics.Checked);
            }

            // Set file index based on selected criteria
            SetFileIndex(bTime);

            // Rename files and update UI
            for (int i = 0; i < mFileList.Count; i++)
            {
                if (mBCancel) { mBCancel = false; break; }

                var xFile = mFileList[i];
                ProgressBar_KeywordUpdateTab_WriteProgress.Value = i + 1;

                xFile.FilePath = Path.GetDirectoryName(xFile.FilePath) + "\\" + Path.GetFileName(xFile.FilePath);

                Label_FileSelection_BrowseFileName.Text = Path.GetDirectoryName(xFile.FilePath) + "\n" + Path.GetFileName(xFile.FilePath);

                var renameTuple = await Task.Run(() => mRenameFile.RenameFile(xFile));

                Label_KeywordUpdateTab_FileName.Text = Path.GetDirectoryName(renameTuple.FileName) + "\n" + Path.GetFileName(renameTuple.FileName);
            }

            // Update progress bar to maximum value
            ProgressBar_KeywordUpdateTab_WriteProgress.Value = ProgressBar_KeywordUpdateTab_WriteProgress.Maximum;

            // Display completion message with number of renamed files and duplicates
            if (duplicates == 1)
                Label_FileSelection_Statistics_OperationStatus.Text = (mFileList.Count).ToString() + " Images Renamed\n" + duplicates.ToString() + " Duplicate";
            else
                Label_FileSelection_Statistics_OperationStatus.Text = (mFileList.Count).ToString() + " Images Renamed\n" + duplicates.ToString() + " Duplicates";

            // Delete directory statistics files
            mDirectoryProperties.DirectoryStatistics.Clear();
            mFileList.Clear();

            // Reset read progress bar
            ProgressBar_FileSelection_ReadProgress.Value = 0;
            UpdateUI(eUiState.RENAME);
        }

        /// <summary>
        /// Handles the CheckedChanged event for the WeightIndex radio button.
        /// Sets the rename order to WEIGHTINDEX if the radio button is checked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void RadioButton_WeightIndex_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButton_FileSelection_SequenceNumbering_WeightIndex.Checked)
            {
                mRenameFile.RenameOrder = eOrder.WEIGHTINDEX;
            }
        }


        /// <summary>
        /// Handles the CheckedChanged event for the Index radio button.
        /// Sets the rename order to INDEX if the radio button is checked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void RadioButton_Index_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButton_FileSelection_SequenceNumbering_IndexOnly.Checked)
            {
                mRenameFile.RenameOrder = eOrder.INDEX;
            }
        }


        /// <summary>
        /// Handles the CheckedChanged event for the Weight radio button.
        /// Sets the rename order to WEIGHT if the radio button is checked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void RadioButton_Weight_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButton_FileSelection_SequenceNumbering_WeightOnly.Checked)
            {
                mRenameFile.RenameOrder = eOrder.WEIGHT;
            }
        }


        /// <summary>
        /// Handles the CheckedChanged event for the IndexWeight radio button.
        /// Sets the rename order to INDEXWEIGHT if the radio button is checked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void RadioButton_IndexWeight_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButton_FileSelection_SequenceNumbering_IndexWeight.Checked)
            {
                mRenameFile.RenameOrder = eOrder.INDEXWEIGHT;
            }
        }


        /// <summary>
        /// Handles the CheckedChanged event for the Master checkbox.
        /// Updates the state of related UI elements and sets master frame keywords for each XisfFile in the file list if the checkbox is checked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void CheckBox_FileSelection_DirectorySelection_Masters_Enable_CheckedChanged(object sender, EventArgs e)
        {
            string rejection = string.Empty;
            string comment = string.Empty;

            Files.DirectoryOperations.Recurse = CheckBox_FileSelection_DirectorySelection_Recurse.Checked;

            TextBox_FileSelection_DirectorySelection_Masters_Frames.Enabled = CheckBox_FileSelection_DirectorySelection_Masters_Enable.Checked;
            TextBox_FileSelection_DirectorySelection_Masters_Rejection.Enabled = CheckBox_FileSelection_DirectorySelection_Masters_Enable.Checked;

            if (CheckBox_FileSelection_DirectorySelection_Masters_Enable.Checked)
            {
                // Set master frame keywords for each file in the file list
                mFileList.ForEach(file => file.KeywordList.SetMasterFrameKeywords());
            }
        }


        /// <summary>
        /// Handles the click event for removing subframe weight keywords.
        /// Updates the list of weight keywords based on the selected criteria (all or selected),
        /// removes the keywords from the XisfFile objects, and updates the UI accordingly.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void Button_KeywordSubFrameWeight_Remove_Click(object sender, EventArgs e)
        {
            List<string> WeightKeywords = new List<string>();

            bool bStatus;
            string frames = TextBox_FileSelection_DirectorySelection_Masters_Frames.Text;
            string algo = TextBox_FileSelection_DirectorySelection_Masters_Rejection.Text;

            int mTotalFrames = 0;
            bStatus = int.TryParse(frames, out mTotalFrames);

            ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.Items.Clear();

            // Repopulate the list of any present weight keywords (not values). Find unique Keywords, sort and populate Weight combobox
            WeightKeywords = mFileList
                .Select(xFile => xFile.WeightKeyword.ToString() ?? string.Empty)
                .Distinct()
                .OrderBy(q => q)
                .ToList();

            if (WeightKeywords.Count > 0)
            {
                WeightKeywords.ForEach(item =>
                {
                    ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.Items.Add(item).ToString();
                });

                Label_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.ForeColor =
                    (WeightKeywords.Count > 1) ? Color.Red : Color.Black;
            }
            else
            {
                ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.Items.Clear();
                Label_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.ForeColor = Color.Black;
                return;
            }

            // Remove ALL WEIGHT items
            if (RadioButton_KeywordUpdateTab_SubFrameKeywords_Weights_All.Checked)
            {
                ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.Items
                    .Cast<string>()
                    .ToList()
                    .ForEach(item =>
                    {
                        mFileList.ForEach(file => file.RemoveKeyword(item));
                    });

                Label_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.ForeColor = Color.Black;
                ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.Items.Clear();
                ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.Text = "";
                return;
            }

            // Only Remove selected item
            if (RadioButton_KeywordUpdateTab_SubFrameKeywords_Weights_Selected.Checked)
            {
                string selectedItem = ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.Text;

                mFileList.ForEach(file => file.RemoveKeyword(selectedItem));

                WeightKeywords.Remove(selectedItem);
                ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.Items.Remove(selectedItem);
                ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.Text = "";

                Label_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.ForeColor =
                    (WeightKeywords.Count > 1) ? Color.Red : Color.Black;

                if (WeightKeywords.Count > 0)
                {
                    ComboBox_KeywordUpdateTab_SubFrameKeywords_Weights_WeightKeywords.SelectedIndex = 0;
                }
            }
        }


        private void CheckBox_FileSelection_DirectorySelection_CalibrationIds_CheckedChanged(object sender, EventArgs e)
        {
            mRenameFile.IncludeCalibrationFrames = CheckBox_FileSelection_DirectorySelection_CalibrationIds.Checked;
        }


        private void Button_SubFrameKeywords_CalibrationFiles_ClearAll_Click(object sender, EventArgs e)
        {
            foreach (XisfFile file in mFileList)
            {
                file.CDARK = string.Empty;
                file.CFLAT = string.Empty;
                file.CBIAS = string.Empty;
                file.CPANEL = string.Empty;
                file.CSTARS = string.Empty;
                file.RemoveKeyword("CLIGHT");
                file.RemoveKeyword("CREJECT");
            }
        }
    }
}
