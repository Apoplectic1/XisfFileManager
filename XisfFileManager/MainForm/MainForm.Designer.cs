namespace XisfFileManager
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            ProgressBar_FileSelection_ReadProgress = new ProgressBar();
            GroupBox_FileSelection_DirectorySelection = new GroupBox();
            CheckBox_Solver = new CheckBox();
            CheckBox_FileSelection_DirectorySelection_CalibrationIds = new CheckBox();
            GroupBox_FileSelection_DirectorySelection_FluxDensity = new GroupBox();
            Button_FileSelection_DirectorySelection_FluxDensity_Run = new Button();
            CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable = new CheckBox();
            GroupBox_FileSelection_DirectorySelection_Masters = new GroupBox();
            TextBox_FileSelection_DirectorySelection_Masters_Rejection = new TextBox();
            CheckBox_FileSelection_DirectorySelection_Masters_Enable = new CheckBox();
            TextBox_FileSelection_DirectorySelection_Masters_Frames = new TextBox();
            CheckBox_FileSlection_DirectorySelection_NoStatistics = new CheckBox();
            Button_FileSelection_DirectorySelection_Rename = new Button();
            Button_FileSelection_DirectorySelection_Browse = new Button();
            CheckBox_FileSelection_DirectorySelection_Recurse = new CheckBox();
            GroupBox_FileSelection_Statistics = new GroupBox();
            Label_FileSelection_Statistics_OperationStatus = new Label();
            Label_FileSelection_Statistics_SubFrameOverhead = new Label();
            Label_FileSelection_Statistics_TempratureCoefficient = new Label();
            Label_FileSelection_BrowseFileName = new Label();
            GroupBox_FileSelection = new GroupBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            TabPage_Calibration = new TabPage();
            groupBox1 = new GroupBox();
            Label_CalibrationTab_Minimum = new Label();
            Button_CalibrationTab_FindPedestal = new Button();
            Label_CalibrationTab_Pedestal = new Label();
            NumericUpDown_CalibrationTab_MinBackground = new NumericUpDown();
            CheckBox_CalibrationTab_CreateNew = new CheckBox();
            TreeView_CalibrationTab_TargetFileTree = new TreeView();
            TextBox_CalibrationTab_Messgaes = new TextBox();
            GroupBox_CalibrationTab_MatchingTolerance = new GroupBox();
            CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest = new CheckBox();
            CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest = new CheckBox();
            CheckBox_CalibrationTab_MatchingTolerance_GainNearest = new CheckBox();
            CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest = new CheckBox();
            Label_CalibrationTab_MatchingTolerance_TemperatureDegrees = new Label();
            Label_CalibrationTab_MatchingTolerance_OffsetADU = new Label();
            Label_CalibrationTab_MatchingTolerance_GainUnits = new Label();
            Label_CalibrationTab_MatchingTolerance_ExposureSeconds = new Label();
            Label_CalibrationTab_MatchingTolerance_Percentage = new Label();
            TextBox_CalibrationTab_MatchingTolerance_Temperature = new TextBox();
            TextBox_CalibrationTab_MatchingTolerance_Offset = new TextBox();
            TextBox_CalibrationTab_MatchingTolerance_Gain = new TextBox();
            Label_CalibrationTab_MatchingTolerance_Temperature = new Label();
            Label_CalibrationTab_MatchingTolerance_Offset = new Label();
            Label_CalibrationTab_MatchingTolerance_Gain = new Label();
            Label_CalibrationTab_MatchingTolerance_Exposure = new Label();
            TextBox_CalibrationTab_MatchingTolerance_Exposure = new TextBox();
            Label_CalibrationTab_TotalFiles = new Label();
            ProgressBar_CalibrationTab = new ProgressBar();
            Label_CalibrationTab_ReadFileName = new Label();
            Button_CalibrationTab_CreateCalibrationDirectory = new Button();
            Button_CalibrationTab_MatchCalibrationFrames = new Button();
            Button_CalibrationTab_FindCalibrationFrames = new Button();
            TabPage_Keywords = new TabPage();
            Button_KeywordUpdateTab_Cancel = new Button();
            Label_KeywordUpdateTab_FileName = new Label();
            ProgressBar_KeywordUpdateTab_WriteProgress = new ProgressBar();
            GroupBox_KeywordUpdateTab_CaptureSoftware = new GroupBox();
            RadioButton_KeywordUpdateTab_CaptureSoftware_NINA = new RadioButton();
            Button_KeywordUpdateTab_CaptureSoftware_SetByFile = new Button();
            Button_KeywordUpdateTab_CaptureSoftware_SetAll = new Button();
            RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager = new RadioButton();
            RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap = new RadioButton();
            RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro = new RadioButton();
            RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX = new RadioButton();
            GroupBox_KeywordUpdateTab_Telescope = new GroupBox();
            TextBox_KeywordUpdateTab_Telescope_FocalLength = new ComboBox();
            Label_KeywordUpdateTab_Telescope_FocalLength = new Label();
            Button_KeywordUpdateTab_Telescope_SetByFile = new Button();
            Button_KeywordUpdateTab_Telescope_SetAll = new Button();
            CheckBox_KeywordUpdateTab_Telescope_Riccardi = new CheckBox();
            RadioButton_KeywordUpdateTab_Telescope_Newtonian254 = new RadioButton();
            RadioButton_KeywordUpdateTab_Telescope_EvoStar150 = new RadioButton();
            RadioButton_KeywordUpdateTab_Telescope_APM107 = new RadioButton();
            GroupBox_KeywordUpdateTab_SubFrameKeywords = new GroupBox();
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile = new ComboBox();
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName = new CheckBox();
            RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue = new RadioButton();
            RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues = new RadioButton();
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment = new ComboBox();
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue = new ComboBox();
            GroupBox_SubFrameKeywords_CalibrationFiles = new GroupBox();
            Button_SubFrameKeywords_CalibrationFiles_ClearAll = new Button();
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection = new GroupBox();
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect = new RadioButton();
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew = new RadioButton();
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force = new RadioButton();
            CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords = new CheckBox();
            Button_KeywordUpdateTab_SubFrameKeywords_Delete = new Button();
            Button_KeywordUpdateTab_SubFrameKeywords_AddReplace = new Button();
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName = new ComboBox();
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName = new CheckBox();
            Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords = new Button();
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames = new ComboBox();
            Label_KeywordUpdateTab_SubFrameKeywords_TagetName = new Label();
            GroupBox_KeywordUpdateTab_Camera = new GroupBox();
            ComboBox_KeywordUpdateTab_Camera_A144Binning = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Q178Binning = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Z183Binning = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Z533Binning = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_A144SensorTemp = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Q178SensorTemp = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Z183SensorTemp = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Z533SensorTemp = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Q178Offset = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Z183Offset = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Z533Offset = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Q178Gain = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Z183Gain = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Z533Gain = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_A144Seconds = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Q178Seconds = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Z533Seconds = new ComboBox();
            ComboBox_KeywordUpdateTab_Camera_Z183Seconds = new ComboBox();
            Label_KeywordUpdateTab_Camera_ToggleNBPreset = new Label();
            Label_KeywordUpdateTab_Camera_Seconds = new Label();
            Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB = new Button();
            CheckBox_KeywordUpdateTab_Camera_A144 = new CheckBox();
            CheckBox_KeywordUpdateTab_Camera_Q178 = new CheckBox();
            CheckBox_KeywordUpdateTab_Camera_Z183 = new CheckBox();
            CheckBox_KeywordUpdateTab_Camera_Z533 = new CheckBox();
            Label_KeywordUpdateTab_Camera_Binning = new Label();
            Label_KeywordUpdateTab_Camera_SensorTemp = new Label();
            Label_KeywordUpdateTab_Camera_Camera = new Label();
            Button_KeywordUpdateTab_Camera_SetByFile = new Button();
            Button_KeywordUpdateTab_Camera_SetAll = new Button();
            Label_KeywordUpdateTab_Camera_A144Gain = new Label();
            Label_KeywordUpdateTab_Camera_Offset = new Label();
            Label_KeywordUpdateTab_Camera_Gain = new Label();
            GroupBox_KeywordUpdateTab_ImageType = new GroupBox();
            Button_KeywordsTab_ImageType_SetByFile = new Button();
            Button_KeywordsTab_ImageType_SetAll = new Button();
            GroupBox_KeywordUpdateTab_ImageType_Frame = new GroupBox();
            Button_KeywordsTab_ImageType_Frame_SetMaster = new Button();
            RadioButton_KeywordsTab_ImageType_Frame_Bias = new RadioButton();
            RadioButton_KeywordsTab_ImageType_Frame_Flat = new RadioButton();
            RadioButton_KeywordsTab_ImageType_Frame_Dark = new RadioButton();
            RadioButton_KeywordsTab_ImageType_Frame_Light = new RadioButton();
            GroupBox_KeywordUpdateTab_ImageType_Filter = new GroupBox();
            RadioButton_KeywordsTab_ImageType_Filter_Luma = new RadioButton();
            RadioButton_KeywordsTab_ImageType_Filter_Shutter = new RadioButton();
            RadioButton_KeywordsTab_ImageType_Filter_Red = new RadioButton();
            RadioButton_KeywordsTab_ImageType_Filter_S2 = new RadioButton();
            RadioButton_KeywordsTab_ImageType_Filter_Ha = new RadioButton();
            RadioButton_KeywordsTab_ImageType_Filter_Blue = new RadioButton();
            RadioButton_KeywordsTab_ImageType_Filter_Green = new RadioButton();
            RadioButton_KeywordsTab_ImageType_Filter_O3 = new RadioButton();
            TabControl = new TabControl();
            CheckBox_VerifySha = new CheckBox();
            GroupBox_FileSelection_DirectorySelection.SuspendLayout();
            GroupBox_FileSelection_DirectorySelection_FluxDensity.SuspendLayout();
            GroupBox_FileSelection_DirectorySelection_Masters.SuspendLayout();
            GroupBox_FileSelection_Statistics.SuspendLayout();
            GroupBox_FileSelection.SuspendLayout();
            TabPage_Calibration.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_CalibrationTab_MinBackground).BeginInit();
            GroupBox_CalibrationTab_MatchingTolerance.SuspendLayout();
            TabPage_Keywords.SuspendLayout();
            GroupBox_KeywordUpdateTab_CaptureSoftware.SuspendLayout();
            GroupBox_KeywordUpdateTab_Telescope.SuspendLayout();
            GroupBox_KeywordUpdateTab_SubFrameKeywords.SuspendLayout();
            GroupBox_SubFrameKeywords_CalibrationFiles.SuspendLayout();
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.SuspendLayout();
            GroupBox_KeywordUpdateTab_Camera.SuspendLayout();
            GroupBox_KeywordUpdateTab_ImageType.SuspendLayout();
            GroupBox_KeywordUpdateTab_ImageType_Frame.SuspendLayout();
            GroupBox_KeywordUpdateTab_ImageType_Filter.SuspendLayout();
            TabControl.SuspendLayout();
            SuspendLayout();
            // 
            // ProgressBar_FileSelection_ReadProgress
            // 
            ProgressBar_FileSelection_ReadProgress.Location = new Point(20, 232);
            ProgressBar_FileSelection_ReadProgress.Margin = new Padding(4, 3, 4, 3);
            ProgressBar_FileSelection_ReadProgress.Name = "ProgressBar_FileSelection_ReadProgress";
            ProgressBar_FileSelection_ReadProgress.Size = new Size(1099, 13);
            ProgressBar_FileSelection_ReadProgress.Step = 1;
            ProgressBar_FileSelection_ReadProgress.TabIndex = 1;
            // 
            // GroupBox_FileSelection_DirectorySelection
            // 
            GroupBox_FileSelection_DirectorySelection.Controls.Add(CheckBox_VerifySha);
            GroupBox_FileSelection_DirectorySelection.Controls.Add(CheckBox_Solver);
            GroupBox_FileSelection_DirectorySelection.Controls.Add(CheckBox_FileSelection_DirectorySelection_CalibrationIds);
            GroupBox_FileSelection_DirectorySelection.Controls.Add(GroupBox_FileSelection_DirectorySelection_FluxDensity);
            GroupBox_FileSelection_DirectorySelection.Controls.Add(GroupBox_FileSelection_DirectorySelection_Masters);
            GroupBox_FileSelection_DirectorySelection.Controls.Add(CheckBox_FileSlection_DirectorySelection_NoStatistics);
            GroupBox_FileSelection_DirectorySelection.Controls.Add(Button_FileSelection_DirectorySelection_Rename);
            GroupBox_FileSelection_DirectorySelection.Controls.Add(Button_FileSelection_DirectorySelection_Browse);
            GroupBox_FileSelection_DirectorySelection.Controls.Add(CheckBox_FileSelection_DirectorySelection_Recurse);
            GroupBox_FileSelection_DirectorySelection.Location = new Point(20, 23);
            GroupBox_FileSelection_DirectorySelection.Margin = new Padding(4, 3, 4, 3);
            GroupBox_FileSelection_DirectorySelection.Name = "GroupBox_FileSelection_DirectorySelection";
            GroupBox_FileSelection_DirectorySelection.Padding = new Padding(4, 3, 4, 3);
            GroupBox_FileSelection_DirectorySelection.Size = new Size(388, 169);
            GroupBox_FileSelection_DirectorySelection.TabIndex = 7;
            GroupBox_FileSelection_DirectorySelection.TabStop = false;
            GroupBox_FileSelection_DirectorySelection.Text = "Directory Selection";
            // 
            // CheckBox_Solver
            // 
            CheckBox_Solver.AutoSize = true;
            CheckBox_Solver.Checked = true;
            CheckBox_Solver.CheckState = CheckState.Checked;
            CheckBox_Solver.Location = new Point(129, 46);
            CheckBox_Solver.Name = "CheckBox_Solver";
            CheckBox_Solver.Size = new Size(58, 19);
            CheckBox_Solver.TabIndex = 25;
            CheckBox_Solver.Text = "Solver";
            CheckBox_Solver.UseVisualStyleBackColor = true;
            // 
            // CheckBox_FileSelection_DirectorySelection_CalibrationIds
            // 
            CheckBox_FileSelection_DirectorySelection_CalibrationIds.AutoSize = true;
            CheckBox_FileSelection_DirectorySelection_CalibrationIds.Location = new Point(173, 146);
            CheckBox_FileSelection_DirectorySelection_CalibrationIds.Name = "CheckBox_FileSelection_DirectorySelection_CalibrationIds";
            CheckBox_FileSelection_DirectorySelection_CalibrationIds.Size = new Size(181, 19);
            CheckBox_FileSelection_DirectorySelection_CalibrationIds.TabIndex = 24;
            CheckBox_FileSelection_DirectorySelection_CalibrationIds.Text = "Include Calibration Frame IDs";
            CheckBox_FileSelection_DirectorySelection_CalibrationIds.UseVisualStyleBackColor = true;
            CheckBox_FileSelection_DirectorySelection_CalibrationIds.CheckedChanged += CheckBox_FileSelection_DirectorySelection_CalibrationIds_CheckedChanged;
            // 
            // GroupBox_FileSelection_DirectorySelection_FluxDensity
            // 
            GroupBox_FileSelection_DirectorySelection_FluxDensity.Controls.Add(Button_FileSelection_DirectorySelection_FluxDensity_Run);
            GroupBox_FileSelection_DirectorySelection_FluxDensity.Controls.Add(CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable);
            GroupBox_FileSelection_DirectorySelection_FluxDensity.Location = new Point(281, 46);
            GroupBox_FileSelection_DirectorySelection_FluxDensity.Name = "GroupBox_FileSelection_DirectorySelection_FluxDensity";
            GroupBox_FileSelection_DirectorySelection_FluxDensity.Size = new Size(100, 100);
            GroupBox_FileSelection_DirectorySelection_FluxDensity.TabIndex = 23;
            GroupBox_FileSelection_DirectorySelection_FluxDensity.TabStop = false;
            GroupBox_FileSelection_DirectorySelection_FluxDensity.Text = "Flux Density";
            // 
            // Button_FileSelection_DirectorySelection_FluxDensity_Run
            // 
            Button_FileSelection_DirectorySelection_FluxDensity_Run.Enabled = false;
            Button_FileSelection_DirectorySelection_FluxDensity_Run.Location = new Point(10, 62);
            Button_FileSelection_DirectorySelection_FluxDensity_Run.Margin = new Padding(4, 3, 4, 3);
            Button_FileSelection_DirectorySelection_FluxDensity_Run.Name = "Button_FileSelection_DirectorySelection_FluxDensity_Run";
            Button_FileSelection_DirectorySelection_FluxDensity_Run.Size = new Size(83, 27);
            Button_FileSelection_DirectorySelection_FluxDensity_Run.TabIndex = 1;
            Button_FileSelection_DirectorySelection_FluxDensity_Run.Text = "Run";
            Button_FileSelection_DirectorySelection_FluxDensity_Run.UseVisualStyleBackColor = true;
            Button_FileSelection_DirectorySelection_FluxDensity_Run.Click += Button_KeywordUpdateTab_SubFrameKeywords_SetupFluxDensity_Click;
            // 
            // CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable
            // 
            CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable.AutoSize = true;
            CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable.Location = new Point(22, 33);
            CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable.Name = "CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable";
            CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable.Size = new Size(61, 19);
            CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable.TabIndex = 0;
            CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable.Text = "Enable";
            CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable.UseVisualStyleBackColor = true;
            CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable.CheckedChanged += CheckBox_FileSelection_DirectorySelection_EnableFluxDensity_CheckedChanged;
            // 
            // GroupBox_FileSelection_DirectorySelection_Masters
            // 
            GroupBox_FileSelection_DirectorySelection_Masters.Controls.Add(TextBox_FileSelection_DirectorySelection_Masters_Rejection);
            GroupBox_FileSelection_DirectorySelection_Masters.Controls.Add(CheckBox_FileSelection_DirectorySelection_Masters_Enable);
            GroupBox_FileSelection_DirectorySelection_Masters.Controls.Add(TextBox_FileSelection_DirectorySelection_Masters_Frames);
            GroupBox_FileSelection_DirectorySelection_Masters.Location = new Point(13, 65);
            GroupBox_FileSelection_DirectorySelection_Masters.Name = "GroupBox_FileSelection_DirectorySelection_Masters";
            GroupBox_FileSelection_DirectorySelection_Masters.Size = new Size(249, 52);
            GroupBox_FileSelection_DirectorySelection_Masters.TabIndex = 15;
            GroupBox_FileSelection_DirectorySelection_Masters.TabStop = false;
            GroupBox_FileSelection_DirectorySelection_Masters.Text = "Masters";
            // 
            // TextBox_FileSelection_DirectorySelection_Masters_Rejection
            // 
            TextBox_FileSelection_DirectorySelection_Masters_Rejection.Location = new Point(182, 20);
            TextBox_FileSelection_DirectorySelection_Masters_Rejection.Name = "TextBox_FileSelection_DirectorySelection_Masters_Rejection";
            TextBox_FileSelection_DirectorySelection_Masters_Rejection.Size = new Size(59, 23);
            TextBox_FileSelection_DirectorySelection_Masters_Rejection.TabIndex = 2;
            TextBox_FileSelection_DirectorySelection_Masters_Rejection.Text = "WSC";
            TextBox_FileSelection_DirectorySelection_Masters_Rejection.TextAlign = HorizontalAlignment.Center;
            // 
            // CheckBox_FileSelection_DirectorySelection_Masters_Enable
            // 
            CheckBox_FileSelection_DirectorySelection_Masters_Enable.AutoSize = true;
            CheckBox_FileSelection_DirectorySelection_Masters_Enable.Enabled = false;
            CheckBox_FileSelection_DirectorySelection_Masters_Enable.Location = new Point(10, 22);
            CheckBox_FileSelection_DirectorySelection_Masters_Enable.Margin = new Padding(4, 3, 4, 3);
            CheckBox_FileSelection_DirectorySelection_Masters_Enable.Name = "CheckBox_FileSelection_DirectorySelection_Masters_Enable";
            CheckBox_FileSelection_DirectorySelection_Masters_Enable.Size = new Size(61, 19);
            CheckBox_FileSelection_DirectorySelection_Masters_Enable.TabIndex = 0;
            CheckBox_FileSelection_DirectorySelection_Masters_Enable.Text = "Enable";
            CheckBox_FileSelection_DirectorySelection_Masters_Enable.UseVisualStyleBackColor = true;
            CheckBox_FileSelection_DirectorySelection_Masters_Enable.CheckedChanged += CheckBox_FileSelection_DirectorySelection_Masters_Enable_CheckedChanged;
            // 
            // TextBox_FileSelection_DirectorySelection_Masters_Frames
            // 
            TextBox_FileSelection_DirectorySelection_Masters_Frames.Enabled = false;
            TextBox_FileSelection_DirectorySelection_Masters_Frames.Location = new Point(100, 20);
            TextBox_FileSelection_DirectorySelection_Masters_Frames.Name = "TextBox_FileSelection_DirectorySelection_Masters_Frames";
            TextBox_FileSelection_DirectorySelection_Masters_Frames.Size = new Size(59, 23);
            TextBox_FileSelection_DirectorySelection_Masters_Frames.TabIndex = 1;
            TextBox_FileSelection_DirectorySelection_Masters_Frames.Text = "Frames";
            TextBox_FileSelection_DirectorySelection_Masters_Frames.TextAlign = HorizontalAlignment.Center;
            // 
            // CheckBox_FileSlection_DirectorySelection_NoStatistics
            // 
            CheckBox_FileSlection_DirectorySelection_NoStatistics.AutoSize = true;
            CheckBox_FileSlection_DirectorySelection_NoStatistics.Location = new Point(173, 126);
            CheckBox_FileSlection_DirectorySelection_NoStatistics.Name = "CheckBox_FileSlection_DirectorySelection_NoStatistics";
            CheckBox_FileSlection_DirectorySelection_NoStatistics.Size = new Size(91, 19);
            CheckBox_FileSlection_DirectorySelection_NoStatistics.TabIndex = 3;
            CheckBox_FileSlection_DirectorySelection_NoStatistics.Text = "No Statistics";
            CheckBox_FileSlection_DirectorySelection_NoStatistics.UseVisualStyleBackColor = true;
            // 
            // Button_FileSelection_DirectorySelection_Rename
            // 
            Button_FileSelection_DirectorySelection_Rename.Location = new Point(17, 130);
            Button_FileSelection_DirectorySelection_Rename.Margin = new Padding(4, 3, 4, 3);
            Button_FileSelection_DirectorySelection_Rename.Name = "Button_FileSelection_DirectorySelection_Rename";
            Button_FileSelection_DirectorySelection_Rename.Size = new Size(145, 27);
            Button_FileSelection_DirectorySelection_Rename.TabIndex = 2;
            Button_FileSelection_DirectorySelection_Rename.Text = "Rename XISF Files";
            Button_FileSelection_DirectorySelection_Rename.UseVisualStyleBackColor = true;
            Button_FileSelection_DirectorySelection_Rename.Click += Button_FileSelection_DirectorySelection_Rename_Click;
            // 
            // Button_FileSelection_DirectorySelection_Browse
            // 
            Button_FileSelection_DirectorySelection_Browse.Location = new Point(22, 29);
            Button_FileSelection_DirectorySelection_Browse.Margin = new Padding(4, 3, 4, 3);
            Button_FileSelection_DirectorySelection_Browse.Name = "Button_FileSelection_DirectorySelection_Browse";
            Button_FileSelection_DirectorySelection_Browse.Size = new Size(88, 27);
            Button_FileSelection_DirectorySelection_Browse.TabIndex = 0;
            Button_FileSelection_DirectorySelection_Browse.Text = "Browse";
            Button_FileSelection_DirectorySelection_Browse.UseVisualStyleBackColor = true;
            Button_FileSelection_DirectorySelection_Browse.Click += Button_Browse_Click;
            // 
            // CheckBox_FileSelection_DirectorySelection_Recurse
            // 
            CheckBox_FileSelection_DirectorySelection_Recurse.AutoSize = true;
            CheckBox_FileSelection_DirectorySelection_Recurse.Checked = true;
            CheckBox_FileSelection_DirectorySelection_Recurse.CheckState = CheckState.Checked;
            CheckBox_FileSelection_DirectorySelection_Recurse.Location = new Point(129, 24);
            CheckBox_FileSelection_DirectorySelection_Recurse.Margin = new Padding(4, 3, 4, 3);
            CheckBox_FileSelection_DirectorySelection_Recurse.Name = "CheckBox_FileSelection_DirectorySelection_Recurse";
            CheckBox_FileSelection_DirectorySelection_Recurse.Size = new Size(126, 19);
            CheckBox_FileSelection_DirectorySelection_Recurse.TabIndex = 1;
            CheckBox_FileSelection_DirectorySelection_Recurse.Text = "Recurse Directories";
            CheckBox_FileSelection_DirectorySelection_Recurse.UseVisualStyleBackColor = true;
            // 
            // GroupBox_FileSelection_Statistics
            // 
            GroupBox_FileSelection_Statistics.Controls.Add(Label_FileSelection_Statistics_OperationStatus);
            GroupBox_FileSelection_Statistics.Controls.Add(Label_FileSelection_Statistics_SubFrameOverhead);
            GroupBox_FileSelection_Statistics.Controls.Add(Label_FileSelection_Statistics_TempratureCoefficient);
            GroupBox_FileSelection_Statistics.Location = new Point(416, 23);
            GroupBox_FileSelection_Statistics.Margin = new Padding(4, 3, 4, 3);
            GroupBox_FileSelection_Statistics.Name = "GroupBox_FileSelection_Statistics";
            GroupBox_FileSelection_Statistics.Padding = new Padding(4, 3, 4, 3);
            GroupBox_FileSelection_Statistics.Size = new Size(460, 169);
            GroupBox_FileSelection_Statistics.TabIndex = 20;
            GroupBox_FileSelection_Statistics.TabStop = false;
            GroupBox_FileSelection_Statistics.Text = "Statistics";
            // 
            // Label_FileSelection_Statistics_OperationStatus
            // 
            Label_FileSelection_Statistics_OperationStatus.AutoSize = true;
            Label_FileSelection_Statistics_OperationStatus.Location = new Point(16, 23);
            Label_FileSelection_Statistics_OperationStatus.Margin = new Padding(4, 0, 4, 0);
            Label_FileSelection_Statistics_OperationStatus.Name = "Label_FileSelection_Statistics_OperationStatus";
            Label_FileSelection_Statistics_OperationStatus.Size = new Size(95, 15);
            Label_FileSelection_Statistics_OperationStatus.TabIndex = 12;
            Label_FileSelection_Statistics_OperationStatus.Text = "Operation Status";
            // 
            // Label_FileSelection_Statistics_SubFrameOverhead
            // 
            Label_FileSelection_Statistics_SubFrameOverhead.AutoSize = true;
            Label_FileSelection_Statistics_SubFrameOverhead.Location = new Point(16, 103);
            Label_FileSelection_Statistics_SubFrameOverhead.Margin = new Padding(4, 0, 4, 0);
            Label_FileSelection_Statistics_SubFrameOverhead.Name = "Label_FileSelection_Statistics_SubFrameOverhead";
            Label_FileSelection_Statistics_SubFrameOverhead.Size = new Size(200, 15);
            Label_FileSelection_Statistics_SubFrameOverhead.TabIndex = 14;
            Label_FileSelection_Statistics_SubFrameOverhead.Text = "SubFrame Overhead: Not Computed";
            // 
            // Label_FileSelection_Statistics_TempratureCoefficient
            // 
            Label_FileSelection_Statistics_TempratureCoefficient.AutoSize = true;
            Label_FileSelection_Statistics_TempratureCoefficient.Location = new Point(16, 63);
            Label_FileSelection_Statistics_TempratureCoefficient.Margin = new Padding(4, 0, 4, 0);
            Label_FileSelection_Statistics_TempratureCoefficient.Name = "Label_FileSelection_Statistics_TempratureCoefficient";
            Label_FileSelection_Statistics_TempratureCoefficient.Size = new Size(221, 15);
            Label_FileSelection_Statistics_TempratureCoefficient.TabIndex = 13;
            Label_FileSelection_Statistics_TempratureCoefficient.Text = "Temperature Coefficient: Not Computed";
            // 
            // Label_FileSelection_BrowseFileName
            // 
            Label_FileSelection_BrowseFileName.AutoSize = true;
            Label_FileSelection_BrowseFileName.Location = new Point(20, 195);
            Label_FileSelection_BrowseFileName.Margin = new Padding(4, 0, 4, 0);
            Label_FileSelection_BrowseFileName.Name = "Label_FileSelection_BrowseFileName";
            Label_FileSelection_BrowseFileName.Size = new Size(101, 15);
            Label_FileSelection_BrowseFileName.TabIndex = 21;
            Label_FileSelection_BrowseFileName.Text = "Browse File Name";
            // 
            // GroupBox_FileSelection
            // 
            GroupBox_FileSelection.Controls.Add(Label_FileSelection_BrowseFileName);
            GroupBox_FileSelection.Controls.Add(GroupBox_FileSelection_Statistics);
            GroupBox_FileSelection.Controls.Add(GroupBox_FileSelection_DirectorySelection);
            GroupBox_FileSelection.Controls.Add(ProgressBar_FileSelection_ReadProgress);
            GroupBox_FileSelection.Location = new Point(14, 6);
            GroupBox_FileSelection.Margin = new Padding(4, 3, 4, 3);
            GroupBox_FileSelection.Name = "GroupBox_FileSelection";
            GroupBox_FileSelection.Padding = new Padding(4, 3, 4, 3);
            GroupBox_FileSelection.Size = new Size(1142, 260);
            GroupBox_FileSelection.TabIndex = 19;
            GroupBox_FileSelection.TabStop = false;
            GroupBox_FileSelection.Text = "File Selection";
            // 
            // TabPage_Calibration
            // 
            TabPage_Calibration.BackColor = SystemColors.Control;
            TabPage_Calibration.Controls.Add(groupBox1);
            TabPage_Calibration.Controls.Add(CheckBox_CalibrationTab_CreateNew);
            TabPage_Calibration.Controls.Add(TreeView_CalibrationTab_TargetFileTree);
            TabPage_Calibration.Controls.Add(TextBox_CalibrationTab_Messgaes);
            TabPage_Calibration.Controls.Add(GroupBox_CalibrationTab_MatchingTolerance);
            TabPage_Calibration.Controls.Add(Label_CalibrationTab_TotalFiles);
            TabPage_Calibration.Controls.Add(ProgressBar_CalibrationTab);
            TabPage_Calibration.Controls.Add(Label_CalibrationTab_ReadFileName);
            TabPage_Calibration.Controls.Add(Button_CalibrationTab_CreateCalibrationDirectory);
            TabPage_Calibration.Controls.Add(Button_CalibrationTab_MatchCalibrationFrames);
            TabPage_Calibration.Controls.Add(Button_CalibrationTab_FindCalibrationFrames);
            TabPage_Calibration.Location = new Point(4, 24);
            TabPage_Calibration.Margin = new Padding(4, 3, 4, 3);
            TabPage_Calibration.Name = "TabPage_Calibration";
            TabPage_Calibration.Padding = new Padding(4, 3, 4, 3);
            TabPage_Calibration.Size = new Size(1139, 507);
            TabPage_Calibration.TabIndex = 1;
            TabPage_Calibration.Text = "Calibration";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(Label_CalibrationTab_Minimum);
            groupBox1.Controls.Add(Button_CalibrationTab_FindPedestal);
            groupBox1.Controls.Add(Label_CalibrationTab_Pedestal);
            groupBox1.Controls.Add(NumericUpDown_CalibrationTab_MinBackground);
            groupBox1.Location = new Point(231, 187);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(301, 83);
            groupBox1.TabIndex = 17;
            groupBox1.TabStop = false;
            groupBox1.Text = "Background";
            // 
            // Label_CalibrationTab_Minimum
            // 
            Label_CalibrationTab_Minimum.AutoSize = true;
            Label_CalibrationTab_Minimum.Location = new Point(37, 29);
            Label_CalibrationTab_Minimum.Name = "Label_CalibrationTab_Minimum";
            Label_CalibrationTab_Minimum.Size = new Size(63, 15);
            Label_CalibrationTab_Minimum.TabIndex = 16;
            Label_CalibrationTab_Minimum.Text = "Minimum:";
            // 
            // Button_CalibrationTab_FindPedestal
            // 
            Button_CalibrationTab_FindPedestal.Location = new Point(187, 25);
            Button_CalibrationTab_FindPedestal.Name = "Button_CalibrationTab_FindPedestal";
            Button_CalibrationTab_FindPedestal.Size = new Size(75, 23);
            Button_CalibrationTab_FindPedestal.TabIndex = 1;
            Button_CalibrationTab_FindPedestal.Text = "Find Pedestal";
            Button_CalibrationTab_FindPedestal.UseVisualStyleBackColor = true;
            Button_CalibrationTab_FindPedestal.Click += Button_CalibrationTab_FindPedestal_Click;
            // 
            // Label_CalibrationTab_Pedestal
            // 
            Label_CalibrationTab_Pedestal.AutoSize = true;
            Label_CalibrationTab_Pedestal.Location = new Point(80, 56);
            Label_CalibrationTab_Pedestal.Name = "Label_CalibrationTab_Pedestal";
            Label_CalibrationTab_Pedestal.Size = new Size(113, 15);
            Label_CalibrationTab_Pedestal.TabIndex = 14;
            Label_CalibrationTab_Pedestal.Text = "Required Pedestal: 0";
            // 
            // NumericUpDown_CalibrationTab_MinBackground
            // 
            NumericUpDown_CalibrationTab_MinBackground.Increment = new decimal(new int[] { 16, 0, 0, 0 });
            NumericUpDown_CalibrationTab_MinBackground.Location = new Point(107, 25);
            NumericUpDown_CalibrationTab_MinBackground.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            NumericUpDown_CalibrationTab_MinBackground.Name = "NumericUpDown_CalibrationTab_MinBackground";
            NumericUpDown_CalibrationTab_MinBackground.Size = new Size(70, 23);
            NumericUpDown_CalibrationTab_MinBackground.TabIndex = 0;
            NumericUpDown_CalibrationTab_MinBackground.TextAlign = HorizontalAlignment.Center;
            NumericUpDown_CalibrationTab_MinBackground.ThousandsSeparator = true;
            // 
            // CheckBox_CalibrationTab_CreateNew
            // 
            CheckBox_CalibrationTab_CreateNew.AutoSize = true;
            CheckBox_CalibrationTab_CreateNew.Checked = true;
            CheckBox_CalibrationTab_CreateNew.CheckState = CheckState.Checked;
            CheckBox_CalibrationTab_CreateNew.Location = new Point(12, 211);
            CheckBox_CalibrationTab_CreateNew.Margin = new Padding(4, 3, 4, 3);
            CheckBox_CalibrationTab_CreateNew.Name = "CheckBox_CalibrationTab_CreateNew";
            CheckBox_CalibrationTab_CreateNew.Size = new Size(60, 19);
            CheckBox_CalibrationTab_CreateNew.TabIndex = 11;
            CheckBox_CalibrationTab_CreateNew.Text = "Create";
            CheckBox_CalibrationTab_CreateNew.UseVisualStyleBackColor = true;
            // 
            // TreeView_CalibrationTab_TargetFileTree
            // 
            TreeView_CalibrationTab_TargetFileTree.Location = new Point(575, 6);
            TreeView_CalibrationTab_TargetFileTree.Margin = new Padding(4, 3, 4, 3);
            TreeView_CalibrationTab_TargetFileTree.Name = "TreeView_CalibrationTab_TargetFileTree";
            TreeView_CalibrationTab_TargetFileTree.Size = new Size(514, 257);
            TreeView_CalibrationTab_TargetFileTree.TabIndex = 10;
            // 
            // TextBox_CalibrationTab_Messgaes
            // 
            TextBox_CalibrationTab_Messgaes.Location = new Point(25, 274);
            TextBox_CalibrationTab_Messgaes.Margin = new Padding(4, 3, 4, 3);
            TextBox_CalibrationTab_Messgaes.Multiline = true;
            TextBox_CalibrationTab_Messgaes.Name = "TextBox_CalibrationTab_Messgaes";
            TextBox_CalibrationTab_Messgaes.ScrollBars = ScrollBars.Both;
            TextBox_CalibrationTab_Messgaes.Size = new Size(1084, 161);
            TextBox_CalibrationTab_Messgaes.TabIndex = 8;
            // 
            // GroupBox_CalibrationTab_MatchingTolerance
            // 
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(CheckBox_CalibrationTab_MatchingTolerance_GainNearest);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(Label_CalibrationTab_MatchingTolerance_TemperatureDegrees);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(Label_CalibrationTab_MatchingTolerance_OffsetADU);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(Label_CalibrationTab_MatchingTolerance_GainUnits);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(Label_CalibrationTab_MatchingTolerance_ExposureSeconds);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(Label_CalibrationTab_MatchingTolerance_Percentage);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(TextBox_CalibrationTab_MatchingTolerance_Temperature);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(TextBox_CalibrationTab_MatchingTolerance_Offset);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(TextBox_CalibrationTab_MatchingTolerance_Gain);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(Label_CalibrationTab_MatchingTolerance_Temperature);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(Label_CalibrationTab_MatchingTolerance_Offset);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(Label_CalibrationTab_MatchingTolerance_Gain);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(Label_CalibrationTab_MatchingTolerance_Exposure);
            GroupBox_CalibrationTab_MatchingTolerance.Controls.Add(TextBox_CalibrationTab_MatchingTolerance_Exposure);
            GroupBox_CalibrationTab_MatchingTolerance.Location = new Point(231, 5);
            GroupBox_CalibrationTab_MatchingTolerance.Margin = new Padding(4, 3, 4, 3);
            GroupBox_CalibrationTab_MatchingTolerance.Name = "GroupBox_CalibrationTab_MatchingTolerance";
            GroupBox_CalibrationTab_MatchingTolerance.Padding = new Padding(4, 3, 4, 3);
            GroupBox_CalibrationTab_MatchingTolerance.Size = new Size(303, 180);
            GroupBox_CalibrationTab_MatchingTolerance.TabIndex = 7;
            GroupBox_CalibrationTab_MatchingTolerance.TabStop = false;
            GroupBox_CalibrationTab_MatchingTolerance.Text = "Match Tolerance";
            // 
            // CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest
            // 
            CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest.AutoSize = true;
            CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest.Location = new Point(219, 138);
            CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest.Name = "CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest";
            CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest.Size = new Size(66, 19);
            CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest.TabIndex = 7;
            CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest.Text = "Nearest";
            CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest.UseVisualStyleBackColor = true;
            CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest.CheckedChanged += CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest_CheckedChanged;
            // 
            // CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest
            // 
            CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest.AutoSize = true;
            CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest.Location = new Point(219, 109);
            CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest.Name = "CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest";
            CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest.Size = new Size(66, 19);
            CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest.TabIndex = 5;
            CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest.Text = "Nearest";
            CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest.UseVisualStyleBackColor = true;
            CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest.CheckedChanged += CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest_CheckedChanged;
            // 
            // CheckBox_CalibrationTab_MatchingTolerance_GainNearest
            // 
            CheckBox_CalibrationTab_MatchingTolerance_GainNearest.AutoSize = true;
            CheckBox_CalibrationTab_MatchingTolerance_GainNearest.Location = new Point(219, 81);
            CheckBox_CalibrationTab_MatchingTolerance_GainNearest.Name = "CheckBox_CalibrationTab_MatchingTolerance_GainNearest";
            CheckBox_CalibrationTab_MatchingTolerance_GainNearest.Size = new Size(66, 19);
            CheckBox_CalibrationTab_MatchingTolerance_GainNearest.TabIndex = 3;
            CheckBox_CalibrationTab_MatchingTolerance_GainNearest.Text = "Nearest";
            CheckBox_CalibrationTab_MatchingTolerance_GainNearest.UseVisualStyleBackColor = true;
            CheckBox_CalibrationTab_MatchingTolerance_GainNearest.CheckedChanged += CheckBox_CalibrationTab_MatchingTolerance_GainNearest_CheckedChanged;
            // 
            // CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest
            // 
            CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest.AutoSize = true;
            CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest.Location = new Point(219, 52);
            CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest.Name = "CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest";
            CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest.Size = new Size(66, 19);
            CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest.TabIndex = 1;
            CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest.Text = "Nearest";
            CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest.UseVisualStyleBackColor = true;
            CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest.CheckedChanged += CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest_CheckedChanged;
            // 
            // Label_CalibrationTab_MatchingTolerance_TemperatureDegrees
            // 
            Label_CalibrationTab_MatchingTolerance_TemperatureDegrees.AutoSize = true;
            Label_CalibrationTab_MatchingTolerance_TemperatureDegrees.Location = new Point(159, 140);
            Label_CalibrationTab_MatchingTolerance_TemperatureDegrees.Margin = new Padding(4, 0, 4, 0);
            Label_CalibrationTab_MatchingTolerance_TemperatureDegrees.Name = "Label_CalibrationTab_MatchingTolerance_TemperatureDegrees";
            Label_CalibrationTab_MatchingTolerance_TemperatureDegrees.Size = new Size(49, 15);
            Label_CalibrationTab_MatchingTolerance_TemperatureDegrees.TabIndex = 12;
            Label_CalibrationTab_MatchingTolerance_TemperatureDegrees.Text = "Degrees";
            // 
            // Label_CalibrationTab_MatchingTolerance_OffsetADU
            // 
            Label_CalibrationTab_MatchingTolerance_OffsetADU.AutoSize = true;
            Label_CalibrationTab_MatchingTolerance_OffsetADU.Location = new Point(159, 111);
            Label_CalibrationTab_MatchingTolerance_OffsetADU.Margin = new Padding(4, 0, 4, 0);
            Label_CalibrationTab_MatchingTolerance_OffsetADU.Name = "Label_CalibrationTab_MatchingTolerance_OffsetADU";
            Label_CalibrationTab_MatchingTolerance_OffsetADU.Size = new Size(31, 15);
            Label_CalibrationTab_MatchingTolerance_OffsetADU.TabIndex = 11;
            Label_CalibrationTab_MatchingTolerance_OffsetADU.Text = "ADU";
            // 
            // Label_CalibrationTab_MatchingTolerance_GainUnits
            // 
            Label_CalibrationTab_MatchingTolerance_GainUnits.AutoSize = true;
            Label_CalibrationTab_MatchingTolerance_GainUnits.Location = new Point(159, 83);
            Label_CalibrationTab_MatchingTolerance_GainUnits.Margin = new Padding(4, 0, 4, 0);
            Label_CalibrationTab_MatchingTolerance_GainUnits.Name = "Label_CalibrationTab_MatchingTolerance_GainUnits";
            Label_CalibrationTab_MatchingTolerance_GainUnits.Size = new Size(34, 15);
            Label_CalibrationTab_MatchingTolerance_GainUnits.TabIndex = 10;
            Label_CalibrationTab_MatchingTolerance_GainUnits.Text = "Units";
            // 
            // Label_CalibrationTab_MatchingTolerance_ExposureSeconds
            // 
            Label_CalibrationTab_MatchingTolerance_ExposureSeconds.AutoSize = true;
            Label_CalibrationTab_MatchingTolerance_ExposureSeconds.Location = new Point(159, 54);
            Label_CalibrationTab_MatchingTolerance_ExposureSeconds.Margin = new Padding(4, 0, 4, 0);
            Label_CalibrationTab_MatchingTolerance_ExposureSeconds.Name = "Label_CalibrationTab_MatchingTolerance_ExposureSeconds";
            Label_CalibrationTab_MatchingTolerance_ExposureSeconds.Size = new Size(51, 15);
            Label_CalibrationTab_MatchingTolerance_ExposureSeconds.TabIndex = 9;
            Label_CalibrationTab_MatchingTolerance_ExposureSeconds.Text = "Seconds";
            // 
            // Label_CalibrationTab_MatchingTolerance_Percentage
            // 
            Label_CalibrationTab_MatchingTolerance_Percentage.AutoSize = true;
            Label_CalibrationTab_MatchingTolerance_Percentage.Location = new Point(88, 26);
            Label_CalibrationTab_MatchingTolerance_Percentage.Margin = new Padding(4, 0, 4, 0);
            Label_CalibrationTab_MatchingTolerance_Percentage.Name = "Label_CalibrationTab_MatchingTolerance_Percentage";
            Label_CalibrationTab_MatchingTolerance_Percentage.Size = new Size(79, 15);
            Label_CalibrationTab_MatchingTolerance_Percentage.TabIndex = 8;
            Label_CalibrationTab_MatchingTolerance_Percentage.Text = "Match Within";
            // 
            // TextBox_CalibrationTab_MatchingTolerance_Temperature
            // 
            TextBox_CalibrationTab_MatchingTolerance_Temperature.Location = new Point(105, 136);
            TextBox_CalibrationTab_MatchingTolerance_Temperature.Margin = new Padding(4, 3, 4, 3);
            TextBox_CalibrationTab_MatchingTolerance_Temperature.Name = "TextBox_CalibrationTab_MatchingTolerance_Temperature";
            TextBox_CalibrationTab_MatchingTolerance_Temperature.Size = new Size(48, 23);
            TextBox_CalibrationTab_MatchingTolerance_Temperature.TabIndex = 6;
            TextBox_CalibrationTab_MatchingTolerance_Temperature.Text = "5";
            TextBox_CalibrationTab_MatchingTolerance_Temperature.TextAlign = HorizontalAlignment.Center;
            TextBox_CalibrationTab_MatchingTolerance_Temperature.TextChanged += TextBox_CalibrationTab_TemperatureTolerance_TextChanged;
            // 
            // TextBox_CalibrationTab_MatchingTolerance_Offset
            // 
            TextBox_CalibrationTab_MatchingTolerance_Offset.Location = new Point(105, 107);
            TextBox_CalibrationTab_MatchingTolerance_Offset.Margin = new Padding(4, 3, 4, 3);
            TextBox_CalibrationTab_MatchingTolerance_Offset.Name = "TextBox_CalibrationTab_MatchingTolerance_Offset";
            TextBox_CalibrationTab_MatchingTolerance_Offset.Size = new Size(48, 23);
            TextBox_CalibrationTab_MatchingTolerance_Offset.TabIndex = 4;
            TextBox_CalibrationTab_MatchingTolerance_Offset.Text = "0";
            TextBox_CalibrationTab_MatchingTolerance_Offset.TextAlign = HorizontalAlignment.Center;
            TextBox_CalibrationTab_MatchingTolerance_Offset.TextChanged += TextBox_CalibrationTab_OffsetTolerance_TextChanged;
            // 
            // TextBox_CalibrationTab_MatchingTolerance_Gain
            // 
            TextBox_CalibrationTab_MatchingTolerance_Gain.Location = new Point(105, 79);
            TextBox_CalibrationTab_MatchingTolerance_Gain.Margin = new Padding(4, 3, 4, 3);
            TextBox_CalibrationTab_MatchingTolerance_Gain.Name = "TextBox_CalibrationTab_MatchingTolerance_Gain";
            TextBox_CalibrationTab_MatchingTolerance_Gain.Size = new Size(48, 23);
            TextBox_CalibrationTab_MatchingTolerance_Gain.TabIndex = 2;
            TextBox_CalibrationTab_MatchingTolerance_Gain.Text = "0";
            TextBox_CalibrationTab_MatchingTolerance_Gain.TextAlign = HorizontalAlignment.Center;
            TextBox_CalibrationTab_MatchingTolerance_Gain.TextChanged += TextBox_CalibrationTab_GainTolerance_TextChanged;
            // 
            // Label_CalibrationTab_MatchingTolerance_Temperature
            // 
            Label_CalibrationTab_MatchingTolerance_Temperature.AutoSize = true;
            Label_CalibrationTab_MatchingTolerance_Temperature.Location = new Point(24, 140);
            Label_CalibrationTab_MatchingTolerance_Temperature.Margin = new Padding(4, 0, 4, 0);
            Label_CalibrationTab_MatchingTolerance_Temperature.Name = "Label_CalibrationTab_MatchingTolerance_Temperature";
            Label_CalibrationTab_MatchingTolerance_Temperature.Size = new Size(74, 15);
            Label_CalibrationTab_MatchingTolerance_Temperature.TabIndex = 4;
            Label_CalibrationTab_MatchingTolerance_Temperature.Text = "Temperature";
            // 
            // Label_CalibrationTab_MatchingTolerance_Offset
            // 
            Label_CalibrationTab_MatchingTolerance_Offset.AutoSize = true;
            Label_CalibrationTab_MatchingTolerance_Offset.Location = new Point(24, 111);
            Label_CalibrationTab_MatchingTolerance_Offset.Margin = new Padding(4, 0, 4, 0);
            Label_CalibrationTab_MatchingTolerance_Offset.Name = "Label_CalibrationTab_MatchingTolerance_Offset";
            Label_CalibrationTab_MatchingTolerance_Offset.Size = new Size(39, 15);
            Label_CalibrationTab_MatchingTolerance_Offset.TabIndex = 3;
            Label_CalibrationTab_MatchingTolerance_Offset.Text = "Offset";
            // 
            // Label_CalibrationTab_MatchingTolerance_Gain
            // 
            Label_CalibrationTab_MatchingTolerance_Gain.AutoSize = true;
            Label_CalibrationTab_MatchingTolerance_Gain.Location = new Point(24, 83);
            Label_CalibrationTab_MatchingTolerance_Gain.Margin = new Padding(4, 0, 4, 0);
            Label_CalibrationTab_MatchingTolerance_Gain.Name = "Label_CalibrationTab_MatchingTolerance_Gain";
            Label_CalibrationTab_MatchingTolerance_Gain.Size = new Size(31, 15);
            Label_CalibrationTab_MatchingTolerance_Gain.TabIndex = 2;
            Label_CalibrationTab_MatchingTolerance_Gain.Text = "Gain";
            // 
            // Label_CalibrationTab_MatchingTolerance_Exposure
            // 
            Label_CalibrationTab_MatchingTolerance_Exposure.AutoSize = true;
            Label_CalibrationTab_MatchingTolerance_Exposure.Location = new Point(24, 54);
            Label_CalibrationTab_MatchingTolerance_Exposure.Margin = new Padding(4, 0, 4, 0);
            Label_CalibrationTab_MatchingTolerance_Exposure.Name = "Label_CalibrationTab_MatchingTolerance_Exposure";
            Label_CalibrationTab_MatchingTolerance_Exposure.Size = new Size(54, 15);
            Label_CalibrationTab_MatchingTolerance_Exposure.TabIndex = 1;
            Label_CalibrationTab_MatchingTolerance_Exposure.Text = "Exposure";
            // 
            // TextBox_CalibrationTab_MatchingTolerance_Exposure
            // 
            TextBox_CalibrationTab_MatchingTolerance_Exposure.Location = new Point(105, 50);
            TextBox_CalibrationTab_MatchingTolerance_Exposure.Margin = new Padding(4, 3, 4, 3);
            TextBox_CalibrationTab_MatchingTolerance_Exposure.Name = "TextBox_CalibrationTab_MatchingTolerance_Exposure";
            TextBox_CalibrationTab_MatchingTolerance_Exposure.Size = new Size(48, 23);
            TextBox_CalibrationTab_MatchingTolerance_Exposure.TabIndex = 0;
            TextBox_CalibrationTab_MatchingTolerance_Exposure.Text = "0";
            TextBox_CalibrationTab_MatchingTolerance_Exposure.TextAlign = HorizontalAlignment.Center;
            TextBox_CalibrationTab_MatchingTolerance_Exposure.TextChanged += TextBox_CalibrationTab_ExposureTolerance_TextChanged;
            // 
            // Label_CalibrationTab_TotalFiles
            // 
            Label_CalibrationTab_TotalFiles.AutoSize = true;
            Label_CalibrationTab_TotalFiles.Location = new Point(56, 8);
            Label_CalibrationTab_TotalFiles.Margin = new Padding(4, 0, 4, 0);
            Label_CalibrationTab_TotalFiles.Name = "Label_CalibrationTab_TotalFiles";
            Label_CalibrationTab_TotalFiles.Size = new Size(125, 15);
            Label_CalibrationTab_TotalFiles.TabIndex = 6;
            Label_CalibrationTab_TotalFiles.Text = "No Calibration Frames";
            Label_CalibrationTab_TotalFiles.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ProgressBar_CalibrationTab
            // 
            ProgressBar_CalibrationTab.Location = new Point(20, 476);
            ProgressBar_CalibrationTab.Margin = new Padding(4, 3, 4, 3);
            ProgressBar_CalibrationTab.Name = "ProgressBar_CalibrationTab";
            ProgressBar_CalibrationTab.Size = new Size(1099, 13);
            ProgressBar_CalibrationTab.TabIndex = 5;
            // 
            // Label_CalibrationTab_ReadFileName
            // 
            Label_CalibrationTab_ReadFileName.AutoSize = true;
            Label_CalibrationTab_ReadFileName.Location = new Point(16, 440);
            Label_CalibrationTab_ReadFileName.Margin = new Padding(4, 0, 4, 0);
            Label_CalibrationTab_ReadFileName.Name = "Label_CalibrationTab_ReadFileName";
            Label_CalibrationTab_ReadFileName.Size = new Size(121, 15);
            Label_CalibrationTab_ReadFileName.TabIndex = 4;
            Label_CalibrationTab_ReadFileName.Text = "Calibration File Name";
            // 
            // Button_CalibrationTab_CreateCalibrationDirectory
            // 
            Button_CalibrationTab_CreateCalibrationDirectory.Location = new Point(74, 182);
            Button_CalibrationTab_CreateCalibrationDirectory.Margin = new Padding(4, 3, 4, 3);
            Button_CalibrationTab_CreateCalibrationDirectory.Name = "Button_CalibrationTab_CreateCalibrationDirectory";
            Button_CalibrationTab_CreateCalibrationDirectory.Size = new Size(88, 76);
            Button_CalibrationTab_CreateCalibrationDirectory.TabIndex = 2;
            Button_CalibrationTab_CreateCalibrationDirectory.Text = "Create Target Calibration Directory";
            Button_CalibrationTab_CreateCalibrationDirectory.UseVisualStyleBackColor = true;
            Button_CalibrationTab_CreateCalibrationDirectory.Click += CalibrationTab_CreateCalibrationDirectory_Click;
            // 
            // Button_CalibrationTab_MatchCalibrationFrames
            // 
            Button_CalibrationTab_MatchCalibrationFrames.Location = new Point(74, 104);
            Button_CalibrationTab_MatchCalibrationFrames.Margin = new Padding(4, 3, 4, 3);
            Button_CalibrationTab_MatchCalibrationFrames.Name = "Button_CalibrationTab_MatchCalibrationFrames";
            Button_CalibrationTab_MatchCalibrationFrames.Size = new Size(88, 76);
            Button_CalibrationTab_MatchCalibrationFrames.TabIndex = 1;
            Button_CalibrationTab_MatchCalibrationFrames.Text = "ReMatch Calibration Frames";
            Button_CalibrationTab_MatchCalibrationFrames.UseVisualStyleBackColor = true;
            Button_CalibrationTab_MatchCalibrationFrames.Click += CalibrationTab_ReMatchCalibrationFrames_Click;
            // 
            // Button_CalibrationTab_FindCalibrationFrames
            // 
            Button_CalibrationTab_FindCalibrationFrames.Location = new Point(74, 28);
            Button_CalibrationTab_FindCalibrationFrames.Margin = new Padding(4, 3, 4, 3);
            Button_CalibrationTab_FindCalibrationFrames.Name = "Button_CalibrationTab_FindCalibrationFrames";
            Button_CalibrationTab_FindCalibrationFrames.Size = new Size(88, 76);
            Button_CalibrationTab_FindCalibrationFrames.TabIndex = 0;
            Button_CalibrationTab_FindCalibrationFrames.Text = "Find Calibration Frames";
            Button_CalibrationTab_FindCalibrationFrames.UseVisualStyleBackColor = true;
            Button_CalibrationTab_FindCalibrationFrames.Click += CalibrationTab_FindCalibrationFrames_Click;
            // 
            // TabPage_Keywords
            // 
            TabPage_Keywords.BackColor = SystemColors.Control;
            TabPage_Keywords.Controls.Add(Button_KeywordUpdateTab_Cancel);
            TabPage_Keywords.Controls.Add(Label_KeywordUpdateTab_FileName);
            TabPage_Keywords.Controls.Add(ProgressBar_KeywordUpdateTab_WriteProgress);
            TabPage_Keywords.Controls.Add(GroupBox_KeywordUpdateTab_CaptureSoftware);
            TabPage_Keywords.Controls.Add(GroupBox_KeywordUpdateTab_Telescope);
            TabPage_Keywords.Controls.Add(GroupBox_KeywordUpdateTab_SubFrameKeywords);
            TabPage_Keywords.Controls.Add(GroupBox_KeywordUpdateTab_Camera);
            TabPage_Keywords.Controls.Add(GroupBox_KeywordUpdateTab_ImageType);
            TabPage_Keywords.Location = new Point(4, 24);
            TabPage_Keywords.Margin = new Padding(4, 3, 4, 3);
            TabPage_Keywords.Name = "TabPage_Keywords";
            TabPage_Keywords.Padding = new Padding(4, 3, 4, 3);
            TabPage_Keywords.Size = new Size(1139, 507);
            TabPage_Keywords.TabIndex = 0;
            TabPage_Keywords.Text = "Keywords";
            // 
            // Button_KeywordUpdateTab_Cancel
            // 
            Button_KeywordUpdateTab_Cancel.Location = new Point(1023, 441);
            Button_KeywordUpdateTab_Cancel.Name = "Button_KeywordUpdateTab_Cancel";
            Button_KeywordUpdateTab_Cancel.Size = new Size(88, 27);
            Button_KeywordUpdateTab_Cancel.TabIndex = 23;
            Button_KeywordUpdateTab_Cancel.Text = "Cancel";
            Button_KeywordUpdateTab_Cancel.UseVisualStyleBackColor = true;
            Button_KeywordUpdateTab_Cancel.Click += Button_KeywordUpdateTab_Cancel_Click;
            // 
            // Label_KeywordUpdateTab_FileName
            // 
            Label_KeywordUpdateTab_FileName.AutoSize = true;
            Label_KeywordUpdateTab_FileName.Location = new Point(20, 441);
            Label_KeywordUpdateTab_FileName.Margin = new Padding(4, 0, 4, 0);
            Label_KeywordUpdateTab_FileName.Name = "Label_KeywordUpdateTab_FileName";
            Label_KeywordUpdateTab_FileName.Size = new Size(77, 15);
            Label_KeywordUpdateTab_FileName.TabIndex = 19;
            Label_KeywordUpdateTab_FileName.Text = "Updating File";
            // 
            // ProgressBar_KeywordUpdateTab_WriteProgress
            // 
            ProgressBar_KeywordUpdateTab_WriteProgress.Location = new Point(20, 479);
            ProgressBar_KeywordUpdateTab_WriteProgress.Margin = new Padding(4, 3, 4, 3);
            ProgressBar_KeywordUpdateTab_WriteProgress.Name = "ProgressBar_KeywordUpdateTab_WriteProgress";
            ProgressBar_KeywordUpdateTab_WriteProgress.Size = new Size(1094, 13);
            ProgressBar_KeywordUpdateTab_WriteProgress.Step = 1;
            ProgressBar_KeywordUpdateTab_WriteProgress.TabIndex = 13;
            // 
            // GroupBox_KeywordUpdateTab_CaptureSoftware
            // 
            GroupBox_KeywordUpdateTab_CaptureSoftware.Controls.Add(RadioButton_KeywordUpdateTab_CaptureSoftware_NINA);
            GroupBox_KeywordUpdateTab_CaptureSoftware.Controls.Add(Button_KeywordUpdateTab_CaptureSoftware_SetByFile);
            GroupBox_KeywordUpdateTab_CaptureSoftware.Controls.Add(Button_KeywordUpdateTab_CaptureSoftware_SetAll);
            GroupBox_KeywordUpdateTab_CaptureSoftware.Controls.Add(RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager);
            GroupBox_KeywordUpdateTab_CaptureSoftware.Controls.Add(RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap);
            GroupBox_KeywordUpdateTab_CaptureSoftware.Controls.Add(RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro);
            GroupBox_KeywordUpdateTab_CaptureSoftware.Controls.Add(RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX);
            GroupBox_KeywordUpdateTab_CaptureSoftware.Location = new Point(20, 212);
            GroupBox_KeywordUpdateTab_CaptureSoftware.Margin = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_CaptureSoftware.Name = "GroupBox_KeywordUpdateTab_CaptureSoftware";
            GroupBox_KeywordUpdateTab_CaptureSoftware.Padding = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_CaptureSoftware.Size = new Size(148, 216);
            GroupBox_KeywordUpdateTab_CaptureSoftware.TabIndex = 22;
            GroupBox_KeywordUpdateTab_CaptureSoftware.TabStop = false;
            GroupBox_KeywordUpdateTab_CaptureSoftware.Text = "Capture Software";
            // 
            // RadioButton_KeywordUpdateTab_CaptureSoftware_NINA
            // 
            RadioButton_KeywordUpdateTab_CaptureSoftware_NINA.AutoSize = true;
            RadioButton_KeywordUpdateTab_CaptureSoftware_NINA.Location = new Point(23, 47);
            RadioButton_KeywordUpdateTab_CaptureSoftware_NINA.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_CaptureSoftware_NINA.Name = "RadioButton_KeywordUpdateTab_CaptureSoftware_NINA";
            RadioButton_KeywordUpdateTab_CaptureSoftware_NINA.Size = new Size(54, 19);
            RadioButton_KeywordUpdateTab_CaptureSoftware_NINA.TabIndex = 1;
            RadioButton_KeywordUpdateTab_CaptureSoftware_NINA.Text = "NINA";
            RadioButton_KeywordUpdateTab_CaptureSoftware_NINA.UseVisualStyleBackColor = true;
            // 
            // Button_KeywordUpdateTab_CaptureSoftware_SetByFile
            // 
            Button_KeywordUpdateTab_CaptureSoftware_SetByFile.Location = new Point(34, 181);
            Button_KeywordUpdateTab_CaptureSoftware_SetByFile.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordUpdateTab_CaptureSoftware_SetByFile.Name = "Button_KeywordUpdateTab_CaptureSoftware_SetByFile";
            Button_KeywordUpdateTab_CaptureSoftware_SetByFile.Size = new Size(88, 27);
            Button_KeywordUpdateTab_CaptureSoftware_SetByFile.TabIndex = 6;
            Button_KeywordUpdateTab_CaptureSoftware_SetByFile.Text = "Set By File";
            Button_KeywordUpdateTab_CaptureSoftware_SetByFile.UseVisualStyleBackColor = true;
            Button_KeywordUpdateTab_CaptureSoftware_SetByFile.Click += Button_CaptureSoftware_SetByFile_Click;
            // 
            // Button_KeywordUpdateTab_CaptureSoftware_SetAll
            // 
            Button_KeywordUpdateTab_CaptureSoftware_SetAll.Location = new Point(34, 150);
            Button_KeywordUpdateTab_CaptureSoftware_SetAll.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordUpdateTab_CaptureSoftware_SetAll.Name = "Button_KeywordUpdateTab_CaptureSoftware_SetAll";
            Button_KeywordUpdateTab_CaptureSoftware_SetAll.Size = new Size(88, 27);
            Button_KeywordUpdateTab_CaptureSoftware_SetAll.TabIndex = 5;
            Button_KeywordUpdateTab_CaptureSoftware_SetAll.Text = "Set All";
            Button_KeywordUpdateTab_CaptureSoftware_SetAll.UseVisualStyleBackColor = true;
            Button_KeywordUpdateTab_CaptureSoftware_SetAll.Click += Button_CaptureSoftware_SetAll_Click;
            // 
            // RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager
            // 
            RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager.AutoSize = true;
            RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager.Location = new Point(23, 93);
            RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager.Name = "RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager";
            RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager.Size = new Size(67, 19);
            RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager.TabIndex = 3;
            RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager.Text = "Voyager";
            RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap
            // 
            RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap.AutoSize = true;
            RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap.Location = new Point(23, 117);
            RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap.Name = "RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap";
            RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap.Size = new Size(76, 19);
            RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap.TabIndex = 4;
            RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap.Text = "SharpCap";
            RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro
            // 
            RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro.AutoSize = true;
            RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro.Location = new Point(23, 70);
            RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro.Name = "RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro";
            RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro.Size = new Size(57, 19);
            RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro.TabIndex = 2;
            RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro.Text = "SGPro";
            RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX
            // 
            RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX.AutoSize = true;
            RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX.Location = new Point(23, 24);
            RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX.Name = "RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX";
            RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX.Size = new Size(73, 19);
            RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX.TabIndex = 0;
            RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX.Text = "The SkyX";
            RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX.UseVisualStyleBackColor = true;
            // 
            // GroupBox_KeywordUpdateTab_Telescope
            // 
            GroupBox_KeywordUpdateTab_Telescope.Controls.Add(TextBox_KeywordUpdateTab_Telescope_FocalLength);
            GroupBox_KeywordUpdateTab_Telescope.Controls.Add(Label_KeywordUpdateTab_Telescope_FocalLength);
            GroupBox_KeywordUpdateTab_Telescope.Controls.Add(Button_KeywordUpdateTab_Telescope_SetByFile);
            GroupBox_KeywordUpdateTab_Telescope.Controls.Add(Button_KeywordUpdateTab_Telescope_SetAll);
            GroupBox_KeywordUpdateTab_Telescope.Controls.Add(CheckBox_KeywordUpdateTab_Telescope_Riccardi);
            GroupBox_KeywordUpdateTab_Telescope.Controls.Add(RadioButton_KeywordUpdateTab_Telescope_Newtonian254);
            GroupBox_KeywordUpdateTab_Telescope.Controls.Add(RadioButton_KeywordUpdateTab_Telescope_EvoStar150);
            GroupBox_KeywordUpdateTab_Telescope.Controls.Add(RadioButton_KeywordUpdateTab_Telescope_APM107);
            GroupBox_KeywordUpdateTab_Telescope.Location = new Point(176, 212);
            GroupBox_KeywordUpdateTab_Telescope.Margin = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_Telescope.Name = "GroupBox_KeywordUpdateTab_Telescope";
            GroupBox_KeywordUpdateTab_Telescope.Padding = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_Telescope.Size = new Size(211, 216);
            GroupBox_KeywordUpdateTab_Telescope.TabIndex = 21;
            GroupBox_KeywordUpdateTab_Telescope.TabStop = false;
            GroupBox_KeywordUpdateTab_Telescope.Text = "Telescope";
            // 
            // TextBox_KeywordUpdateTab_Telescope_FocalLength
            // 
            TextBox_KeywordUpdateTab_Telescope_FocalLength.FormattingEnabled = true;
            TextBox_KeywordUpdateTab_Telescope_FocalLength.Location = new Point(39, 139);
            TextBox_KeywordUpdateTab_Telescope_FocalLength.Name = "TextBox_KeywordUpdateTab_Telescope_FocalLength";
            TextBox_KeywordUpdateTab_Telescope_FocalLength.Size = new Size(67, 23);
            TextBox_KeywordUpdateTab_Telescope_FocalLength.TabIndex = 4;
            // 
            // Label_KeywordUpdateTab_Telescope_FocalLength
            // 
            Label_KeywordUpdateTab_Telescope_FocalLength.AutoSize = true;
            Label_KeywordUpdateTab_Telescope_FocalLength.Location = new Point(110, 143);
            Label_KeywordUpdateTab_Telescope_FocalLength.Margin = new Padding(4, 0, 4, 0);
            Label_KeywordUpdateTab_Telescope_FocalLength.Name = "Label_KeywordUpdateTab_Telescope_FocalLength";
            Label_KeywordUpdateTab_Telescope_FocalLength.Size = new Size(75, 15);
            Label_KeywordUpdateTab_Telescope_FocalLength.TabIndex = 18;
            Label_KeywordUpdateTab_Telescope_FocalLength.Text = "Focal Length";
            // 
            // Button_KeywordUpdateTab_Telescope_SetByFile
            // 
            Button_KeywordUpdateTab_Telescope_SetByFile.Location = new Point(108, 181);
            Button_KeywordUpdateTab_Telescope_SetByFile.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordUpdateTab_Telescope_SetByFile.Name = "Button_KeywordUpdateTab_Telescope_SetByFile";
            Button_KeywordUpdateTab_Telescope_SetByFile.Size = new Size(88, 27);
            Button_KeywordUpdateTab_Telescope_SetByFile.TabIndex = 6;
            Button_KeywordUpdateTab_Telescope_SetByFile.Text = "Set By File";
            Button_KeywordUpdateTab_Telescope_SetByFile.UseVisualStyleBackColor = true;
            Button_KeywordUpdateTab_Telescope_SetByFile.Click += Button_Telescope_SetByFile_Click;
            // 
            // Button_KeywordUpdateTab_Telescope_SetAll
            // 
            Button_KeywordUpdateTab_Telescope_SetAll.Location = new Point(13, 181);
            Button_KeywordUpdateTab_Telescope_SetAll.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordUpdateTab_Telescope_SetAll.Name = "Button_KeywordUpdateTab_Telescope_SetAll";
            Button_KeywordUpdateTab_Telescope_SetAll.Size = new Size(88, 27);
            Button_KeywordUpdateTab_Telescope_SetAll.TabIndex = 5;
            Button_KeywordUpdateTab_Telescope_SetAll.Text = "Set All";
            Button_KeywordUpdateTab_Telescope_SetAll.UseVisualStyleBackColor = true;
            Button_KeywordUpdateTab_Telescope_SetAll.Click += Button_Telescope_SetAll_Click;
            // 
            // CheckBox_KeywordUpdateTab_Telescope_Riccardi
            // 
            CheckBox_KeywordUpdateTab_Telescope_Riccardi.AutoSize = true;
            CheckBox_KeywordUpdateTab_Telescope_Riccardi.Checked = true;
            CheckBox_KeywordUpdateTab_Telescope_Riccardi.CheckState = CheckState.Checked;
            CheckBox_KeywordUpdateTab_Telescope_Riccardi.Location = new Point(20, 114);
            CheckBox_KeywordUpdateTab_Telescope_Riccardi.Margin = new Padding(4, 3, 4, 3);
            CheckBox_KeywordUpdateTab_Telescope_Riccardi.Name = "CheckBox_KeywordUpdateTab_Telescope_Riccardi";
            CheckBox_KeywordUpdateTab_Telescope_Riccardi.Size = new Size(138, 19);
            CheckBox_KeywordUpdateTab_Telescope_Riccardi.TabIndex = 3;
            CheckBox_KeywordUpdateTab_Telescope_Riccardi.Text = "Riccardi 0.75 Reducer";
            CheckBox_KeywordUpdateTab_Telescope_Riccardi.UseVisualStyleBackColor = true;
            CheckBox_KeywordUpdateTab_Telescope_Riccardi.CheckedChanged += CheckBox_KeywordTelescope_Riccardi_CheckedChanged;
            // 
            // RadioButton_KeywordUpdateTab_Telescope_Newtonian254
            // 
            RadioButton_KeywordUpdateTab_Telescope_Newtonian254.AutoSize = true;
            RadioButton_KeywordUpdateTab_Telescope_Newtonian254.Location = new Point(20, 84);
            RadioButton_KeywordUpdateTab_Telescope_Newtonian254.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_Telescope_Newtonian254.Name = "RadioButton_KeywordUpdateTab_Telescope_Newtonian254";
            RadioButton_KeywordUpdateTab_Telescope_Newtonian254.Size = new Size(104, 19);
            RadioButton_KeywordUpdateTab_Telescope_Newtonian254.TabIndex = 2;
            RadioButton_KeywordUpdateTab_Telescope_Newtonian254.Text = "Newtonian 254";
            RadioButton_KeywordUpdateTab_Telescope_Newtonian254.UseVisualStyleBackColor = true;
            RadioButton_KeywordUpdateTab_Telescope_Newtonian254.CheckedChanged += RadioButton_KeywordTelescope_NWT254_CheckedChanged;
            // 
            // RadioButton_KeywordUpdateTab_Telescope_EvoStar150
            // 
            RadioButton_KeywordUpdateTab_Telescope_EvoStar150.AutoSize = true;
            RadioButton_KeywordUpdateTab_Telescope_EvoStar150.Location = new Point(20, 54);
            RadioButton_KeywordUpdateTab_Telescope_EvoStar150.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_Telescope_EvoStar150.Name = "RadioButton_KeywordUpdateTab_Telescope_EvoStar150";
            RadioButton_KeywordUpdateTab_Telescope_EvoStar150.Size = new Size(85, 19);
            RadioButton_KeywordUpdateTab_Telescope_EvoStar150.TabIndex = 1;
            RadioButton_KeywordUpdateTab_Telescope_EvoStar150.Text = "EvoStar 150";
            RadioButton_KeywordUpdateTab_Telescope_EvoStar150.UseVisualStyleBackColor = true;
            RadioButton_KeywordUpdateTab_Telescope_EvoStar150.CheckedChanged += RadioButton_KeywordTelescope_EVO150_CheckedChanged;
            // 
            // RadioButton_KeywordUpdateTab_Telescope_APM107
            // 
            RadioButton_KeywordUpdateTab_Telescope_APM107.AutoSize = true;
            RadioButton_KeywordUpdateTab_Telescope_APM107.Location = new Point(20, 24);
            RadioButton_KeywordUpdateTab_Telescope_APM107.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_Telescope_APM107.Name = "RadioButton_KeywordUpdateTab_Telescope_APM107";
            RadioButton_KeywordUpdateTab_Telescope_APM107.Size = new Size(72, 19);
            RadioButton_KeywordUpdateTab_Telescope_APM107.TabIndex = 0;
            RadioButton_KeywordUpdateTab_Telescope_APM107.Text = "APM 107";
            RadioButton_KeywordUpdateTab_Telescope_APM107.UseVisualStyleBackColor = true;
            RadioButton_KeywordUpdateTab_Telescope_APM107.CheckedChanged += RadioButton_KeywordTelescope_APM107_CheckedChanged;
            // 
            // GroupBox_KeywordUpdateTab_SubFrameKeywords
            // 
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(GroupBox_SubFrameKeywords_CalibrationFiles);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(Button_KeywordUpdateTab_SubFrameKeywords_Delete);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(Button_KeywordUpdateTab_SubFrameKeywords_AddReplace);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Controls.Add(Label_KeywordUpdateTab_SubFrameKeywords_TagetName);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Location = new Point(20, 15);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Margin = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Name = "GroupBox_KeywordUpdateTab_SubFrameKeywords";
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Padding = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Size = new Size(1092, 191);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.TabIndex = 14;
            GroupBox_KeywordUpdateTab_SubFrameKeywords.TabStop = false;
            GroupBox_KeywordUpdateTab_SubFrameKeywords.Text = "SubFrame Keywords";
            // 
            // ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile
            // 
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile.Location = new Point(529, 16);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile.Margin = new Padding(4, 3, 4, 3);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile.Name = "ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile";
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile.Size = new Size(252, 23);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile.Sorted = true;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile.TabIndex = 7;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile.Text = "File";
            // 
            // CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName
            // 
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName.AutoSize = true;
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName.Location = new Point(9, 98);
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName.Margin = new Padding(4, 3, 4, 3);
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName.Name = "CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName";
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName.Size = new Size(151, 19);
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName.TabIndex = 2;
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName.Text = "Do Not Update CPANEL";
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue
            // 
            RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue.AutoSize = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue.Location = new Point(651, 133);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue.Name = "RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue";
            RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue.Size = new Size(97, 19);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue.TabIndex = 12;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue.Text = "Specific Value";
            RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues
            // 
            RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues.AutoSize = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues.Checked = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues.Location = new Point(567, 133);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues.Name = "RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues";
            RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues.Size = new Size(75, 19);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues.TabIndex = 11;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues.TabStop = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues.Text = "All Values";
            RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues.UseVisualStyleBackColor = true;
            // 
            // ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment
            // 
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment.Location = new Point(529, 103);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment.Margin = new Padding(4, 3, 4, 3);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment.Name = "ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment";
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment.Size = new Size(252, 23);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment.Sorted = true;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment.TabIndex = 10;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment.Text = "Comment";
            // 
            // ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue
            // 
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue.Location = new Point(529, 74);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue.Margin = new Padding(4, 3, 4, 3);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue.Name = "ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue";
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue.Size = new Size(252, 23);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue.Sorted = true;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue.TabIndex = 9;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue.Text = "Value";
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue.SelectedValueChanged += ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue_SelectedValueChanged;
            // 
            // GroupBox_SubFrameKeywords_CalibrationFiles
            // 
            GroupBox_SubFrameKeywords_CalibrationFiles.Controls.Add(Button_SubFrameKeywords_CalibrationFiles_ClearAll);
            GroupBox_SubFrameKeywords_CalibrationFiles.Location = new Point(252, 108);
            GroupBox_SubFrameKeywords_CalibrationFiles.Margin = new Padding(4, 3, 4, 3);
            GroupBox_SubFrameKeywords_CalibrationFiles.Name = "GroupBox_SubFrameKeywords_CalibrationFiles";
            GroupBox_SubFrameKeywords_CalibrationFiles.Padding = new Padding(4, 3, 4, 3);
            GroupBox_SubFrameKeywords_CalibrationFiles.Size = new Size(252, 53);
            GroupBox_SubFrameKeywords_CalibrationFiles.TabIndex = 6;
            GroupBox_SubFrameKeywords_CalibrationFiles.TabStop = false;
            GroupBox_SubFrameKeywords_CalibrationFiles.Text = "Calibration Files and Keywords";
            // 
            // Button_SubFrameKeywords_CalibrationFiles_ClearAll
            // 
            Button_SubFrameKeywords_CalibrationFiles_ClearAll.Location = new Point(44, 20);
            Button_SubFrameKeywords_CalibrationFiles_ClearAll.Margin = new Padding(4, 3, 4, 3);
            Button_SubFrameKeywords_CalibrationFiles_ClearAll.Name = "Button_SubFrameKeywords_CalibrationFiles_ClearAll";
            Button_SubFrameKeywords_CalibrationFiles_ClearAll.Size = new Size(168, 27);
            Button_SubFrameKeywords_CalibrationFiles_ClearAll.TabIndex = 0;
            Button_SubFrameKeywords_CalibrationFiles_ClearAll.Text = "Delete All Calibration Data";
            Button_SubFrameKeywords_CalibrationFiles_ClearAll.UseVisualStyleBackColor = true;
            Button_SubFrameKeywords_CalibrationFiles_ClearAll.Click += Button_SubFrameKeywords_CalibrationFiles_ClearAll_Click;
            // 
            // GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection
            // 
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.Controls.Add(RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect);
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.Controls.Add(RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew);
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.Controls.Add(RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force);
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.Location = new Point(252, 49);
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.Margin = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.Name = "GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection";
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.Padding = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.Size = new Size(252, 48);
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.TabIndex = 5;
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.TabStop = false;
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.Text = "Keyword Protection";
            // 
            // RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect
            // 
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect.AutoSize = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect.Location = new Point(14, 20);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect.Name = "RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect";
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect.Size = new Size(63, 19);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect.TabIndex = 0;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect.Text = "Protect";
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect.UseVisualStyleBackColor = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect.CheckedChanged += RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect_CheckedChanged;
            // 
            // RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew
            // 
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew.AutoSize = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew.Checked = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew.Location = new Point(85, 20);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew.Name = "RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew";
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew.Size = new Size(90, 19);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew.TabIndex = 1;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew.TabStop = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew.Text = "Update New";
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew.UseVisualStyleBackColor = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew.CheckedChanged += RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew_CheckedChanged;
            // 
            // RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force
            // 
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force.AutoSize = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force.Location = new Point(183, 20);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force.Name = "RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force";
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force.Size = new Size(54, 19);
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force.TabIndex = 2;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force.Text = "Force";
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force.UseVisualStyleBackColor = true;
            RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force.CheckedChanged += RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force_CheckedChanged;
            // 
            // CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords
            // 
            CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords.AutoSize = true;
            CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords.Checked = true;
            CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords.CheckState = CheckState.Checked;
            CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords.Location = new Point(263, 21);
            CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords.Margin = new Padding(4, 3, 4, 3);
            CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords.Name = "CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords";
            CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords.Size = new Size(142, 19);
            CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords.TabIndex = 4;
            CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords.Text = "Alphabetize Keywords";
            CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords.UseVisualStyleBackColor = true;
            // 
            // Button_KeywordUpdateTab_SubFrameKeywords_Delete
            // 
            Button_KeywordUpdateTab_SubFrameKeywords_Delete.Location = new Point(674, 158);
            Button_KeywordUpdateTab_SubFrameKeywords_Delete.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordUpdateTab_SubFrameKeywords_Delete.Name = "Button_KeywordUpdateTab_SubFrameKeywords_Delete";
            Button_KeywordUpdateTab_SubFrameKeywords_Delete.Size = new Size(108, 27);
            Button_KeywordUpdateTab_SubFrameKeywords_Delete.TabIndex = 14;
            Button_KeywordUpdateTab_SubFrameKeywords_Delete.Text = "Delete";
            Button_KeywordUpdateTab_SubFrameKeywords_Delete.UseVisualStyleBackColor = true;
            Button_KeywordUpdateTab_SubFrameKeywords_Delete.Click += Button_KeywordUpdateTab_SubFrameKeywords_Delete_Click;
            // 
            // Button_KeywordUpdateTab_SubFrameKeywords_AddReplace
            // 
            Button_KeywordUpdateTab_SubFrameKeywords_AddReplace.Location = new Point(529, 157);
            Button_KeywordUpdateTab_SubFrameKeywords_AddReplace.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordUpdateTab_SubFrameKeywords_AddReplace.Name = "Button_KeywordUpdateTab_SubFrameKeywords_AddReplace";
            Button_KeywordUpdateTab_SubFrameKeywords_AddReplace.Size = new Size(126, 27);
            Button_KeywordUpdateTab_SubFrameKeywords_AddReplace.TabIndex = 13;
            Button_KeywordUpdateTab_SubFrameKeywords_AddReplace.Text = "Add/Replace";
            Button_KeywordUpdateTab_SubFrameKeywords_AddReplace.UseVisualStyleBackColor = true;
            Button_KeywordUpdateTab_SubFrameKeywords_AddReplace.Click += Button_KeywordUpdateTab_SubFrameKeywords_AddReplace_Click;
            // 
            // ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName
            // 
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.Location = new Point(529, 45);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.Margin = new Padding(4, 3, 4, 3);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.Name = "ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName";
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.Size = new Size(252, 23);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.Sorted = true;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.TabIndex = 8;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.Text = "Keyword";
            ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName.SelectedIndexChanged += ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName_SelectedIndexChanged;
            // 
            // CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName
            // 
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName.AutoSize = true;
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName.Location = new Point(9, 73);
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName.Margin = new Padding(4, 3, 4, 3);
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName.Name = "CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName";
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName.Size = new Size(236, 19);
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName.TabIndex = 1;
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName.Text = "Update Target Name (and include Stars)";
            CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName.UseVisualStyleBackColor = true;
            // 
            // Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords
            // 
            Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords.Location = new Point(66, 127);
            Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords.Name = "Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords";
            Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords.Size = new Size(111, 27);
            Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords.TabIndex = 3;
            Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords.Text = "Update Keywords";
            Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords.UseVisualStyleBackColor = true;
            Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords.Click += Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords_Click;
            // 
            // ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames
            // 
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.AllowDrop = true;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.Location = new Point(24, 40);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.Margin = new Padding(4, 3, 4, 3);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.Name = "ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames";
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.Size = new Size(194, 23);
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.Sorted = true;
            ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames.TabIndex = 0;
            // 
            // Label_KeywordUpdateTab_SubFrameKeywords_TagetName
            // 
            Label_KeywordUpdateTab_SubFrameKeywords_TagetName.AllowDrop = true;
            Label_KeywordUpdateTab_SubFrameKeywords_TagetName.AutoSize = true;
            Label_KeywordUpdateTab_SubFrameKeywords_TagetName.Location = new Point(84, 22);
            Label_KeywordUpdateTab_SubFrameKeywords_TagetName.Margin = new Padding(4, 0, 4, 0);
            Label_KeywordUpdateTab_SubFrameKeywords_TagetName.Name = "Label_KeywordUpdateTab_SubFrameKeywords_TagetName";
            Label_KeywordUpdateTab_SubFrameKeywords_TagetName.Size = new Size(75, 15);
            Label_KeywordUpdateTab_SubFrameKeywords_TagetName.TabIndex = 0;
            Label_KeywordUpdateTab_SubFrameKeywords_TagetName.Text = "Target Name";
            Label_KeywordUpdateTab_SubFrameKeywords_TagetName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // GroupBox_KeywordUpdateTab_Camera
            // 
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_A144Binning);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Q178Binning);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Z183Binning);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Z533Binning);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_A144SensorTemp);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Q178SensorTemp);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Z183SensorTemp);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Z533SensorTemp);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Q178Offset);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Z183Offset);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Z533Offset);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Q178Gain);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Z183Gain);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Z533Gain);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_A144Seconds);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Q178Seconds);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Z533Seconds);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(ComboBox_KeywordUpdateTab_Camera_Z183Seconds);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(Label_KeywordUpdateTab_Camera_ToggleNBPreset);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(Label_KeywordUpdateTab_Camera_Seconds);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(CheckBox_KeywordUpdateTab_Camera_A144);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(CheckBox_KeywordUpdateTab_Camera_Q178);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(CheckBox_KeywordUpdateTab_Camera_Z183);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(CheckBox_KeywordUpdateTab_Camera_Z533);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(Label_KeywordUpdateTab_Camera_Binning);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(Label_KeywordUpdateTab_Camera_SensorTemp);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(Label_KeywordUpdateTab_Camera_Camera);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(Button_KeywordUpdateTab_Camera_SetByFile);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(Button_KeywordUpdateTab_Camera_SetAll);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(Label_KeywordUpdateTab_Camera_A144Gain);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(Label_KeywordUpdateTab_Camera_Offset);
            GroupBox_KeywordUpdateTab_Camera.Controls.Add(Label_KeywordUpdateTab_Camera_Gain);
            GroupBox_KeywordUpdateTab_Camera.Location = new Point(395, 212);
            GroupBox_KeywordUpdateTab_Camera.Margin = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_Camera.Name = "GroupBox_KeywordUpdateTab_Camera";
            GroupBox_KeywordUpdateTab_Camera.Padding = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_Camera.Size = new Size(385, 216);
            GroupBox_KeywordUpdateTab_Camera.TabIndex = 20;
            GroupBox_KeywordUpdateTab_Camera.TabStop = false;
            GroupBox_KeywordUpdateTab_Camera.Text = "Camera";
            // 
            // ComboBox_KeywordUpdateTab_Camera_A144Binning
            // 
            ComboBox_KeywordUpdateTab_Camera_A144Binning.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_A144Binning.Location = new Point(330, 133);
            ComboBox_KeywordUpdateTab_Camera_A144Binning.Name = "ComboBox_KeywordUpdateTab_Camera_A144Binning";
            ComboBox_KeywordUpdateTab_Camera_A144Binning.Size = new Size(44, 23);
            ComboBox_KeywordUpdateTab_Camera_A144Binning.TabIndex = 21;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Q178Binning
            // 
            ComboBox_KeywordUpdateTab_Camera_Q178Binning.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Q178Binning.Location = new Point(330, 103);
            ComboBox_KeywordUpdateTab_Camera_Q178Binning.Name = "ComboBox_KeywordUpdateTab_Camera_Q178Binning";
            ComboBox_KeywordUpdateTab_Camera_Q178Binning.Size = new Size(44, 23);
            ComboBox_KeywordUpdateTab_Camera_Q178Binning.TabIndex = 17;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Z183Binning
            // 
            ComboBox_KeywordUpdateTab_Camera_Z183Binning.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Z183Binning.Location = new Point(330, 73);
            ComboBox_KeywordUpdateTab_Camera_Z183Binning.Name = "ComboBox_KeywordUpdateTab_Camera_Z183Binning";
            ComboBox_KeywordUpdateTab_Camera_Z183Binning.Size = new Size(44, 23);
            ComboBox_KeywordUpdateTab_Camera_Z183Binning.TabIndex = 11;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Z533Binning
            // 
            ComboBox_KeywordUpdateTab_Camera_Z533Binning.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Z533Binning.Location = new Point(330, 43);
            ComboBox_KeywordUpdateTab_Camera_Z533Binning.Name = "ComboBox_KeywordUpdateTab_Camera_Z533Binning";
            ComboBox_KeywordUpdateTab_Camera_Z533Binning.Size = new Size(44, 23);
            ComboBox_KeywordUpdateTab_Camera_Z533Binning.TabIndex = 5;
            // 
            // ComboBox_KeywordUpdateTab_Camera_A144SensorTemp
            // 
            ComboBox_KeywordUpdateTab_Camera_A144SensorTemp.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_A144SensorTemp.Location = new Point(265, 133);
            ComboBox_KeywordUpdateTab_Camera_A144SensorTemp.Name = "ComboBox_KeywordUpdateTab_Camera_A144SensorTemp";
            ComboBox_KeywordUpdateTab_Camera_A144SensorTemp.Size = new Size(55, 23);
            ComboBox_KeywordUpdateTab_Camera_A144SensorTemp.TabIndex = 20;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Q178SensorTemp
            // 
            ComboBox_KeywordUpdateTab_Camera_Q178SensorTemp.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Q178SensorTemp.Location = new Point(265, 103);
            ComboBox_KeywordUpdateTab_Camera_Q178SensorTemp.Name = "ComboBox_KeywordUpdateTab_Camera_Q178SensorTemp";
            ComboBox_KeywordUpdateTab_Camera_Q178SensorTemp.Size = new Size(55, 23);
            ComboBox_KeywordUpdateTab_Camera_Q178SensorTemp.TabIndex = 16;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Z183SensorTemp
            // 
            ComboBox_KeywordUpdateTab_Camera_Z183SensorTemp.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Z183SensorTemp.Location = new Point(265, 73);
            ComboBox_KeywordUpdateTab_Camera_Z183SensorTemp.Name = "ComboBox_KeywordUpdateTab_Camera_Z183SensorTemp";
            ComboBox_KeywordUpdateTab_Camera_Z183SensorTemp.Size = new Size(55, 23);
            ComboBox_KeywordUpdateTab_Camera_Z183SensorTemp.TabIndex = 10;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Z533SensorTemp
            // 
            ComboBox_KeywordUpdateTab_Camera_Z533SensorTemp.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Z533SensorTemp.Location = new Point(265, 43);
            ComboBox_KeywordUpdateTab_Camera_Z533SensorTemp.Name = "ComboBox_KeywordUpdateTab_Camera_Z533SensorTemp";
            ComboBox_KeywordUpdateTab_Camera_Z533SensorTemp.Size = new Size(55, 23);
            ComboBox_KeywordUpdateTab_Camera_Z533SensorTemp.TabIndex = 4;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Q178Offset
            // 
            ComboBox_KeywordUpdateTab_Camera_Q178Offset.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Q178Offset.Location = new Point(204, 103);
            ComboBox_KeywordUpdateTab_Camera_Q178Offset.Name = "ComboBox_KeywordUpdateTab_Camera_Q178Offset";
            ComboBox_KeywordUpdateTab_Camera_Q178Offset.Size = new Size(51, 23);
            ComboBox_KeywordUpdateTab_Camera_Q178Offset.TabIndex = 15;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Z183Offset
            // 
            ComboBox_KeywordUpdateTab_Camera_Z183Offset.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Z183Offset.Location = new Point(204, 73);
            ComboBox_KeywordUpdateTab_Camera_Z183Offset.Name = "ComboBox_KeywordUpdateTab_Camera_Z183Offset";
            ComboBox_KeywordUpdateTab_Camera_Z183Offset.Size = new Size(51, 23);
            ComboBox_KeywordUpdateTab_Camera_Z183Offset.TabIndex = 9;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Z533Offset
            // 
            ComboBox_KeywordUpdateTab_Camera_Z533Offset.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Z533Offset.Location = new Point(204, 43);
            ComboBox_KeywordUpdateTab_Camera_Z533Offset.Name = "ComboBox_KeywordUpdateTab_Camera_Z533Offset";
            ComboBox_KeywordUpdateTab_Camera_Z533Offset.Size = new Size(51, 23);
            ComboBox_KeywordUpdateTab_Camera_Z533Offset.TabIndex = 3;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Q178Gain
            // 
            ComboBox_KeywordUpdateTab_Camera_Q178Gain.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Q178Gain.Location = new Point(144, 103);
            ComboBox_KeywordUpdateTab_Camera_Q178Gain.Name = "ComboBox_KeywordUpdateTab_Camera_Q178Gain";
            ComboBox_KeywordUpdateTab_Camera_Q178Gain.Size = new Size(51, 23);
            ComboBox_KeywordUpdateTab_Camera_Q178Gain.TabIndex = 14;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Z183Gain
            // 
            ComboBox_KeywordUpdateTab_Camera_Z183Gain.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Z183Gain.Location = new Point(144, 73);
            ComboBox_KeywordUpdateTab_Camera_Z183Gain.Name = "ComboBox_KeywordUpdateTab_Camera_Z183Gain";
            ComboBox_KeywordUpdateTab_Camera_Z183Gain.Size = new Size(51, 23);
            ComboBox_KeywordUpdateTab_Camera_Z183Gain.TabIndex = 8;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Z533Gain
            // 
            ComboBox_KeywordUpdateTab_Camera_Z533Gain.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Z533Gain.Location = new Point(144, 43);
            ComboBox_KeywordUpdateTab_Camera_Z533Gain.Name = "ComboBox_KeywordUpdateTab_Camera_Z533Gain";
            ComboBox_KeywordUpdateTab_Camera_Z533Gain.Size = new Size(51, 23);
            ComboBox_KeywordUpdateTab_Camera_Z533Gain.TabIndex = 2;
            // 
            // ComboBox_KeywordUpdateTab_Camera_A144Seconds
            // 
            ComboBox_KeywordUpdateTab_Camera_A144Seconds.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_A144Seconds.Location = new Point(78, 133);
            ComboBox_KeywordUpdateTab_Camera_A144Seconds.Name = "ComboBox_KeywordUpdateTab_Camera_A144Seconds";
            ComboBox_KeywordUpdateTab_Camera_A144Seconds.Size = new Size(55, 23);
            ComboBox_KeywordUpdateTab_Camera_A144Seconds.TabIndex = 19;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Q178Seconds
            // 
            ComboBox_KeywordUpdateTab_Camera_Q178Seconds.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Q178Seconds.Location = new Point(78, 103);
            ComboBox_KeywordUpdateTab_Camera_Q178Seconds.Name = "ComboBox_KeywordUpdateTab_Camera_Q178Seconds";
            ComboBox_KeywordUpdateTab_Camera_Q178Seconds.Size = new Size(55, 23);
            ComboBox_KeywordUpdateTab_Camera_Q178Seconds.TabIndex = 13;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Z533Seconds
            // 
            ComboBox_KeywordUpdateTab_Camera_Z533Seconds.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Z533Seconds.Location = new Point(78, 43);
            ComboBox_KeywordUpdateTab_Camera_Z533Seconds.Name = "ComboBox_KeywordUpdateTab_Camera_Z533Seconds";
            ComboBox_KeywordUpdateTab_Camera_Z533Seconds.Size = new Size(55, 23);
            ComboBox_KeywordUpdateTab_Camera_Z533Seconds.TabIndex = 1;
            // 
            // ComboBox_KeywordUpdateTab_Camera_Z183Seconds
            // 
            ComboBox_KeywordUpdateTab_Camera_Z183Seconds.FormattingEnabled = true;
            ComboBox_KeywordUpdateTab_Camera_Z183Seconds.Location = new Point(78, 73);
            ComboBox_KeywordUpdateTab_Camera_Z183Seconds.Name = "ComboBox_KeywordUpdateTab_Camera_Z183Seconds";
            ComboBox_KeywordUpdateTab_Camera_Z183Seconds.Size = new Size(55, 23);
            ComboBox_KeywordUpdateTab_Camera_Z183Seconds.TabIndex = 7;
            // 
            // Label_KeywordUpdateTab_Camera_ToggleNBPreset
            // 
            Label_KeywordUpdateTab_Camera_ToggleNBPreset.AutoSize = true;
            Label_KeywordUpdateTab_Camera_ToggleNBPreset.Location = new Point(323, 185);
            Label_KeywordUpdateTab_Camera_ToggleNBPreset.Margin = new Padding(4, 0, 4, 0);
            Label_KeywordUpdateTab_Camera_ToggleNBPreset.Name = "Label_KeywordUpdateTab_Camera_ToggleNBPreset";
            Label_KeywordUpdateTab_Camera_ToggleNBPreset.Size = new Size(39, 15);
            Label_KeywordUpdateTab_Camera_ToggleNBPreset.TabIndex = 25;
            Label_KeywordUpdateTab_Camera_ToggleNBPreset.Text = "Preset";
            // 
            // Label_KeywordUpdateTab_Camera_Seconds
            // 
            Label_KeywordUpdateTab_Camera_Seconds.AutoSize = true;
            Label_KeywordUpdateTab_Camera_Seconds.Location = new Point(80, 21);
            Label_KeywordUpdateTab_Camera_Seconds.Margin = new Padding(4, 0, 4, 0);
            Label_KeywordUpdateTab_Camera_Seconds.Name = "Label_KeywordUpdateTab_Camera_Seconds";
            Label_KeywordUpdateTab_Camera_Seconds.Size = new Size(51, 15);
            Label_KeywordUpdateTab_Camera_Seconds.TabIndex = 21;
            Label_KeywordUpdateTab_Camera_Seconds.Text = "Seconds";
            // 
            // Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB
            // 
            Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB.Location = new Point(265, 181);
            Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB.Name = "Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB";
            Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB.Size = new Size(56, 23);
            Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB.TabIndex = 24;
            Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB.Text = "Set";
            Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB.UseVisualStyleBackColor = true;
            Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB.Click += Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB_Click;
            // 
            // CheckBox_KeywordUpdateTab_Camera_A144
            // 
            CheckBox_KeywordUpdateTab_Camera_A144.AutoSize = true;
            CheckBox_KeywordUpdateTab_Camera_A144.Location = new Point(16, 135);
            CheckBox_KeywordUpdateTab_Camera_A144.Margin = new Padding(4, 3, 4, 3);
            CheckBox_KeywordUpdateTab_Camera_A144.Name = "CheckBox_KeywordUpdateTab_Camera_A144";
            CheckBox_KeywordUpdateTab_Camera_A144.Size = new Size(52, 19);
            CheckBox_KeywordUpdateTab_Camera_A144.TabIndex = 18;
            CheckBox_KeywordUpdateTab_Camera_A144.Text = "A144";
            CheckBox_KeywordUpdateTab_Camera_A144.UseVisualStyleBackColor = true;
            // 
            // CheckBox_KeywordUpdateTab_Camera_Q178
            // 
            CheckBox_KeywordUpdateTab_Camera_Q178.AutoSize = true;
            CheckBox_KeywordUpdateTab_Camera_Q178.Location = new Point(16, 105);
            CheckBox_KeywordUpdateTab_Camera_Q178.Margin = new Padding(4, 3, 4, 3);
            CheckBox_KeywordUpdateTab_Camera_Q178.Name = "CheckBox_KeywordUpdateTab_Camera_Q178";
            CheckBox_KeywordUpdateTab_Camera_Q178.Size = new Size(53, 19);
            CheckBox_KeywordUpdateTab_Camera_Q178.TabIndex = 12;
            CheckBox_KeywordUpdateTab_Camera_Q178.Text = "Q178";
            CheckBox_KeywordUpdateTab_Camera_Q178.UseVisualStyleBackColor = true;
            // 
            // CheckBox_KeywordUpdateTab_Camera_Z183
            // 
            CheckBox_KeywordUpdateTab_Camera_Z183.AutoSize = true;
            CheckBox_KeywordUpdateTab_Camera_Z183.Location = new Point(16, 75);
            CheckBox_KeywordUpdateTab_Camera_Z183.Margin = new Padding(4, 3, 4, 3);
            CheckBox_KeywordUpdateTab_Camera_Z183.Name = "CheckBox_KeywordUpdateTab_Camera_Z183";
            CheckBox_KeywordUpdateTab_Camera_Z183.Size = new Size(51, 19);
            CheckBox_KeywordUpdateTab_Camera_Z183.TabIndex = 6;
            CheckBox_KeywordUpdateTab_Camera_Z183.Text = "Z183";
            CheckBox_KeywordUpdateTab_Camera_Z183.UseVisualStyleBackColor = true;
            // 
            // CheckBox_KeywordUpdateTab_Camera_Z533
            // 
            CheckBox_KeywordUpdateTab_Camera_Z533.AutoSize = true;
            CheckBox_KeywordUpdateTab_Camera_Z533.Location = new Point(16, 45);
            CheckBox_KeywordUpdateTab_Camera_Z533.Margin = new Padding(4, 3, 4, 3);
            CheckBox_KeywordUpdateTab_Camera_Z533.Name = "CheckBox_KeywordUpdateTab_Camera_Z533";
            CheckBox_KeywordUpdateTab_Camera_Z533.Size = new Size(51, 19);
            CheckBox_KeywordUpdateTab_Camera_Z533.TabIndex = 0;
            CheckBox_KeywordUpdateTab_Camera_Z533.Text = "Z533";
            CheckBox_KeywordUpdateTab_Camera_Z533.UseVisualStyleBackColor = true;
            // 
            // Label_KeywordUpdateTab_Camera_Binning
            // 
            Label_KeywordUpdateTab_Camera_Binning.AutoSize = true;
            Label_KeywordUpdateTab_Camera_Binning.Location = new Point(328, 21);
            Label_KeywordUpdateTab_Camera_Binning.Margin = new Padding(4, 0, 4, 0);
            Label_KeywordUpdateTab_Camera_Binning.Name = "Label_KeywordUpdateTab_Camera_Binning";
            Label_KeywordUpdateTab_Camera_Binning.Size = new Size(48, 15);
            Label_KeywordUpdateTab_Camera_Binning.TabIndex = 18;
            Label_KeywordUpdateTab_Camera_Binning.Text = "Binning";
            // 
            // Label_KeywordUpdateTab_Camera_SensorTemp
            // 
            Label_KeywordUpdateTab_Camera_SensorTemp.AutoSize = true;
            Label_KeywordUpdateTab_Camera_SensorTemp.Location = new Point(257, 21);
            Label_KeywordUpdateTab_Camera_SensorTemp.Margin = new Padding(4, 0, 4, 0);
            Label_KeywordUpdateTab_Camera_SensorTemp.Name = "Label_KeywordUpdateTab_Camera_SensorTemp";
            Label_KeywordUpdateTab_Camera_SensorTemp.Size = new Size(72, 15);
            Label_KeywordUpdateTab_Camera_SensorTemp.TabIndex = 16;
            Label_KeywordUpdateTab_Camera_SensorTemp.Text = "SensorTemp";
            // 
            // Label_KeywordUpdateTab_Camera_Camera
            // 
            Label_KeywordUpdateTab_Camera_Camera.AutoSize = true;
            Label_KeywordUpdateTab_Camera_Camera.Location = new Point(12, 21);
            Label_KeywordUpdateTab_Camera_Camera.Margin = new Padding(4, 0, 4, 0);
            Label_KeywordUpdateTab_Camera_Camera.Name = "Label_KeywordUpdateTab_Camera_Camera";
            Label_KeywordUpdateTab_Camera_Camera.Size = new Size(48, 15);
            Label_KeywordUpdateTab_Camera_Camera.TabIndex = 23;
            Label_KeywordUpdateTab_Camera_Camera.Text = "Camera";
            // 
            // Button_KeywordUpdateTab_Camera_SetByFile
            // 
            Button_KeywordUpdateTab_Camera_SetByFile.Location = new Point(126, 181);
            Button_KeywordUpdateTab_Camera_SetByFile.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordUpdateTab_Camera_SetByFile.Name = "Button_KeywordUpdateTab_Camera_SetByFile";
            Button_KeywordUpdateTab_Camera_SetByFile.Size = new Size(88, 27);
            Button_KeywordUpdateTab_Camera_SetByFile.TabIndex = 23;
            Button_KeywordUpdateTab_Camera_SetByFile.Text = "Set By File";
            Button_KeywordUpdateTab_Camera_SetByFile.UseVisualStyleBackColor = true;
            Button_KeywordUpdateTab_Camera_SetByFile.Click += Button_KeywordCamera_SetByFile_Click;
            // 
            // Button_KeywordUpdateTab_Camera_SetAll
            // 
            Button_KeywordUpdateTab_Camera_SetAll.Location = new Point(30, 181);
            Button_KeywordUpdateTab_Camera_SetAll.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordUpdateTab_Camera_SetAll.Name = "Button_KeywordUpdateTab_Camera_SetAll";
            Button_KeywordUpdateTab_Camera_SetAll.Size = new Size(88, 27);
            Button_KeywordUpdateTab_Camera_SetAll.TabIndex = 22;
            Button_KeywordUpdateTab_Camera_SetAll.Text = "Set All";
            Button_KeywordUpdateTab_Camera_SetAll.UseVisualStyleBackColor = true;
            Button_KeywordUpdateTab_Camera_SetAll.Click += Button_KeywordCamera_SetAll_Click;
            // 
            // Label_KeywordUpdateTab_Camera_A144Gain
            // 
            Label_KeywordUpdateTab_Camera_A144Gain.AutoSize = true;
            Label_KeywordUpdateTab_Camera_A144Gain.Location = new Point(155, 137);
            Label_KeywordUpdateTab_Camera_A144Gain.Margin = new Padding(4, 0, 4, 0);
            Label_KeywordUpdateTab_Camera_A144Gain.Name = "Label_KeywordUpdateTab_Camera_A144Gain";
            Label_KeywordUpdateTab_Camera_A144Gain.Size = new Size(28, 15);
            Label_KeywordUpdateTab_Camera_A144Gain.TabIndex = 12;
            Label_KeywordUpdateTab_Camera_A144Gain.Text = "0.37";
            // 
            // Label_KeywordUpdateTab_Camera_Offset
            // 
            Label_KeywordUpdateTab_Camera_Offset.AutoSize = true;
            Label_KeywordUpdateTab_Camera_Offset.Location = new Point(210, 21);
            Label_KeywordUpdateTab_Camera_Offset.Margin = new Padding(4, 0, 4, 0);
            Label_KeywordUpdateTab_Camera_Offset.Name = "Label_KeywordUpdateTab_Camera_Offset";
            Label_KeywordUpdateTab_Camera_Offset.Size = new Size(39, 15);
            Label_KeywordUpdateTab_Camera_Offset.TabIndex = 11;
            Label_KeywordUpdateTab_Camera_Offset.Text = "Offset";
            // 
            // Label_KeywordUpdateTab_Camera_Gain
            // 
            Label_KeywordUpdateTab_Camera_Gain.AutoSize = true;
            Label_KeywordUpdateTab_Camera_Gain.Location = new Point(154, 21);
            Label_KeywordUpdateTab_Camera_Gain.Margin = new Padding(4, 0, 4, 0);
            Label_KeywordUpdateTab_Camera_Gain.Name = "Label_KeywordUpdateTab_Camera_Gain";
            Label_KeywordUpdateTab_Camera_Gain.Size = new Size(31, 15);
            Label_KeywordUpdateTab_Camera_Gain.TabIndex = 10;
            Label_KeywordUpdateTab_Camera_Gain.Text = "Gain";
            // 
            // GroupBox_KeywordUpdateTab_ImageType
            // 
            GroupBox_KeywordUpdateTab_ImageType.Controls.Add(Button_KeywordsTab_ImageType_SetByFile);
            GroupBox_KeywordUpdateTab_ImageType.Controls.Add(Button_KeywordsTab_ImageType_SetAll);
            GroupBox_KeywordUpdateTab_ImageType.Controls.Add(GroupBox_KeywordUpdateTab_ImageType_Frame);
            GroupBox_KeywordUpdateTab_ImageType.Controls.Add(GroupBox_KeywordUpdateTab_ImageType_Filter);
            GroupBox_KeywordUpdateTab_ImageType.Location = new Point(788, 212);
            GroupBox_KeywordUpdateTab_ImageType.Margin = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_ImageType.Name = "GroupBox_KeywordUpdateTab_ImageType";
            GroupBox_KeywordUpdateTab_ImageType.Padding = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_ImageType.Size = new Size(323, 216);
            GroupBox_KeywordUpdateTab_ImageType.TabIndex = 18;
            GroupBox_KeywordUpdateTab_ImageType.TabStop = false;
            GroupBox_KeywordUpdateTab_ImageType.Text = "Image Type";
            // 
            // Button_KeywordsTab_ImageType_SetByFile
            // 
            Button_KeywordsTab_ImageType_SetByFile.Location = new Point(170, 181);
            Button_KeywordsTab_ImageType_SetByFile.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordsTab_ImageType_SetByFile.Name = "Button_KeywordsTab_ImageType_SetByFile";
            Button_KeywordsTab_ImageType_SetByFile.Size = new Size(88, 27);
            Button_KeywordsTab_ImageType_SetByFile.TabIndex = 3;
            Button_KeywordsTab_ImageType_SetByFile.Text = "Set By File";
            Button_KeywordsTab_ImageType_SetByFile.UseVisualStyleBackColor = true;
            Button_KeywordsTab_ImageType_SetByFile.Click += Button_KeywordImageTypeFrame_SetByFile_Click;
            // 
            // Button_KeywordsTab_ImageType_SetAll
            // 
            Button_KeywordsTab_ImageType_SetAll.Location = new Point(68, 181);
            Button_KeywordsTab_ImageType_SetAll.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordsTab_ImageType_SetAll.Name = "Button_KeywordsTab_ImageType_SetAll";
            Button_KeywordsTab_ImageType_SetAll.Size = new Size(88, 27);
            Button_KeywordsTab_ImageType_SetAll.TabIndex = 2;
            Button_KeywordsTab_ImageType_SetAll.Text = "Set All";
            Button_KeywordsTab_ImageType_SetAll.UseVisualStyleBackColor = true;
            Button_KeywordsTab_ImageType_SetAll.Click += Button_KeywordImageTypeFrame_SetAll_Click;
            // 
            // GroupBox_KeywordUpdateTab_ImageType_Frame
            // 
            GroupBox_KeywordUpdateTab_ImageType_Frame.Controls.Add(Button_KeywordsTab_ImageType_Frame_SetMaster);
            GroupBox_KeywordUpdateTab_ImageType_Frame.Controls.Add(RadioButton_KeywordsTab_ImageType_Frame_Bias);
            GroupBox_KeywordUpdateTab_ImageType_Frame.Controls.Add(RadioButton_KeywordsTab_ImageType_Frame_Flat);
            GroupBox_KeywordUpdateTab_ImageType_Frame.Controls.Add(RadioButton_KeywordsTab_ImageType_Frame_Dark);
            GroupBox_KeywordUpdateTab_ImageType_Frame.Controls.Add(RadioButton_KeywordsTab_ImageType_Frame_Light);
            GroupBox_KeywordUpdateTab_ImageType_Frame.Location = new Point(10, 100);
            GroupBox_KeywordUpdateTab_ImageType_Frame.Margin = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_ImageType_Frame.Name = "GroupBox_KeywordUpdateTab_ImageType_Frame";
            GroupBox_KeywordUpdateTab_ImageType_Frame.Padding = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_ImageType_Frame.Size = new Size(299, 75);
            GroupBox_KeywordUpdateTab_ImageType_Frame.TabIndex = 1;
            GroupBox_KeywordUpdateTab_ImageType_Frame.TabStop = false;
            GroupBox_KeywordUpdateTab_ImageType_Frame.Text = "Frame";
            // 
            // Button_KeywordsTab_ImageType_Frame_SetMaster
            // 
            Button_KeywordsTab_ImageType_Frame_SetMaster.Location = new Point(112, 43);
            Button_KeywordsTab_ImageType_Frame_SetMaster.Margin = new Padding(4, 3, 4, 3);
            Button_KeywordsTab_ImageType_Frame_SetMaster.Name = "Button_KeywordsTab_ImageType_Frame_SetMaster";
            Button_KeywordsTab_ImageType_Frame_SetMaster.Size = new Size(88, 27);
            Button_KeywordsTab_ImageType_Frame_SetMaster.TabIndex = 4;
            Button_KeywordsTab_ImageType_Frame_SetMaster.Text = "Set Master";
            Button_KeywordsTab_ImageType_Frame_SetMaster.UseVisualStyleBackColor = true;
            Button_KeywordsTab_ImageType_Frame_SetMaster.Click += Button_KeywordImageTypeFrame_SetMaster_Click;
            // 
            // RadioButton_KeywordsTab_ImageType_Frame_Bias
            // 
            RadioButton_KeywordsTab_ImageType_Frame_Bias.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Frame_Bias.Location = new Point(231, 20);
            RadioButton_KeywordsTab_ImageType_Frame_Bias.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Frame_Bias.Name = "RadioButton_KeywordsTab_ImageType_Frame_Bias";
            RadioButton_KeywordsTab_ImageType_Frame_Bias.Size = new Size(46, 19);
            RadioButton_KeywordsTab_ImageType_Frame_Bias.TabIndex = 3;
            RadioButton_KeywordsTab_ImageType_Frame_Bias.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Frame_Bias.Text = "Bias";
            RadioButton_KeywordsTab_ImageType_Frame_Bias.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordsTab_ImageType_Frame_Flat
            // 
            RadioButton_KeywordsTab_ImageType_Frame_Flat.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Frame_Flat.Location = new Point(170, 20);
            RadioButton_KeywordsTab_ImageType_Frame_Flat.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Frame_Flat.Name = "RadioButton_KeywordsTab_ImageType_Frame_Flat";
            RadioButton_KeywordsTab_ImageType_Frame_Flat.Size = new Size(44, 19);
            RadioButton_KeywordsTab_ImageType_Frame_Flat.TabIndex = 2;
            RadioButton_KeywordsTab_ImageType_Frame_Flat.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Frame_Flat.Text = "Flat";
            RadioButton_KeywordsTab_ImageType_Frame_Flat.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordsTab_ImageType_Frame_Dark
            // 
            RadioButton_KeywordsTab_ImageType_Frame_Dark.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Frame_Dark.Location = new Point(104, 20);
            RadioButton_KeywordsTab_ImageType_Frame_Dark.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Frame_Dark.Name = "RadioButton_KeywordsTab_ImageType_Frame_Dark";
            RadioButton_KeywordsTab_ImageType_Frame_Dark.Size = new Size(49, 19);
            RadioButton_KeywordsTab_ImageType_Frame_Dark.TabIndex = 1;
            RadioButton_KeywordsTab_ImageType_Frame_Dark.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Frame_Dark.Text = "Dark";
            RadioButton_KeywordsTab_ImageType_Frame_Dark.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordsTab_ImageType_Frame_Light
            // 
            RadioButton_KeywordsTab_ImageType_Frame_Light.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Frame_Light.Location = new Point(35, 20);
            RadioButton_KeywordsTab_ImageType_Frame_Light.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Frame_Light.Name = "RadioButton_KeywordsTab_ImageType_Frame_Light";
            RadioButton_KeywordsTab_ImageType_Frame_Light.Size = new Size(52, 19);
            RadioButton_KeywordsTab_ImageType_Frame_Light.TabIndex = 0;
            RadioButton_KeywordsTab_ImageType_Frame_Light.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Frame_Light.Text = "Light";
            RadioButton_KeywordsTab_ImageType_Frame_Light.UseVisualStyleBackColor = true;
            // 
            // GroupBox_KeywordUpdateTab_ImageType_Filter
            // 
            GroupBox_KeywordUpdateTab_ImageType_Filter.Controls.Add(RadioButton_KeywordsTab_ImageType_Filter_Luma);
            GroupBox_KeywordUpdateTab_ImageType_Filter.Controls.Add(RadioButton_KeywordsTab_ImageType_Filter_Shutter);
            GroupBox_KeywordUpdateTab_ImageType_Filter.Controls.Add(RadioButton_KeywordsTab_ImageType_Filter_Red);
            GroupBox_KeywordUpdateTab_ImageType_Filter.Controls.Add(RadioButton_KeywordsTab_ImageType_Filter_S2);
            GroupBox_KeywordUpdateTab_ImageType_Filter.Controls.Add(RadioButton_KeywordsTab_ImageType_Filter_Ha);
            GroupBox_KeywordUpdateTab_ImageType_Filter.Controls.Add(RadioButton_KeywordsTab_ImageType_Filter_Blue);
            GroupBox_KeywordUpdateTab_ImageType_Filter.Controls.Add(RadioButton_KeywordsTab_ImageType_Filter_Green);
            GroupBox_KeywordUpdateTab_ImageType_Filter.Controls.Add(RadioButton_KeywordsTab_ImageType_Filter_O3);
            GroupBox_KeywordUpdateTab_ImageType_Filter.Location = new Point(10, 20);
            GroupBox_KeywordUpdateTab_ImageType_Filter.Margin = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_ImageType_Filter.Name = "GroupBox_KeywordUpdateTab_ImageType_Filter";
            GroupBox_KeywordUpdateTab_ImageType_Filter.Padding = new Padding(4, 3, 4, 3);
            GroupBox_KeywordUpdateTab_ImageType_Filter.Size = new Size(299, 81);
            GroupBox_KeywordUpdateTab_ImageType_Filter.TabIndex = 0;
            GroupBox_KeywordUpdateTab_ImageType_Filter.TabStop = false;
            GroupBox_KeywordUpdateTab_ImageType_Filter.Text = "Filter";
            // 
            // RadioButton_KeywordsTab_ImageType_Filter_Luma
            // 
            RadioButton_KeywordsTab_ImageType_Filter_Luma.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Filter_Luma.Location = new Point(34, 21);
            RadioButton_KeywordsTab_ImageType_Filter_Luma.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Filter_Luma.Name = "RadioButton_KeywordsTab_ImageType_Filter_Luma";
            RadioButton_KeywordsTab_ImageType_Filter_Luma.Size = new Size(55, 19);
            RadioButton_KeywordsTab_ImageType_Filter_Luma.TabIndex = 0;
            RadioButton_KeywordsTab_ImageType_Filter_Luma.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Filter_Luma.Text = "Luma";
            RadioButton_KeywordsTab_ImageType_Filter_Luma.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordsTab_ImageType_Filter_Shutter
            // 
            RadioButton_KeywordsTab_ImageType_Filter_Shutter.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Filter_Shutter.Location = new Point(231, 51);
            RadioButton_KeywordsTab_ImageType_Filter_Shutter.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Filter_Shutter.Name = "RadioButton_KeywordsTab_ImageType_Filter_Shutter";
            RadioButton_KeywordsTab_ImageType_Filter_Shutter.Size = new Size(63, 19);
            RadioButton_KeywordsTab_ImageType_Filter_Shutter.TabIndex = 7;
            RadioButton_KeywordsTab_ImageType_Filter_Shutter.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Filter_Shutter.Text = "Shutter";
            RadioButton_KeywordsTab_ImageType_Filter_Shutter.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordsTab_ImageType_Filter_Red
            // 
            RadioButton_KeywordsTab_ImageType_Filter_Red.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Filter_Red.Location = new Point(34, 51);
            RadioButton_KeywordsTab_ImageType_Filter_Red.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Filter_Red.Name = "RadioButton_KeywordsTab_ImageType_Filter_Red";
            RadioButton_KeywordsTab_ImageType_Filter_Red.Size = new Size(45, 19);
            RadioButton_KeywordsTab_ImageType_Filter_Red.TabIndex = 4;
            RadioButton_KeywordsTab_ImageType_Filter_Red.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Filter_Red.Text = "Red";
            RadioButton_KeywordsTab_ImageType_Filter_Red.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordsTab_ImageType_Filter_S2
            // 
            RadioButton_KeywordsTab_ImageType_Filter_S2.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Filter_S2.Location = new Point(231, 21);
            RadioButton_KeywordsTab_ImageType_Filter_S2.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Filter_S2.Name = "RadioButton_KeywordsTab_ImageType_Filter_S2";
            RadioButton_KeywordsTab_ImageType_Filter_S2.Size = new Size(40, 19);
            RadioButton_KeywordsTab_ImageType_Filter_S2.TabIndex = 3;
            RadioButton_KeywordsTab_ImageType_Filter_S2.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Filter_S2.Text = "S II";
            RadioButton_KeywordsTab_ImageType_Filter_S2.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordsTab_ImageType_Filter_Ha
            // 
            RadioButton_KeywordsTab_ImageType_Filter_Ha.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Filter_Ha.Location = new Point(108, 21);
            RadioButton_KeywordsTab_ImageType_Filter_Ha.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Filter_Ha.Name = "RadioButton_KeywordsTab_ImageType_Filter_Ha";
            RadioButton_KeywordsTab_ImageType_Filter_Ha.Size = new Size(40, 19);
            RadioButton_KeywordsTab_ImageType_Filter_Ha.TabIndex = 1;
            RadioButton_KeywordsTab_ImageType_Filter_Ha.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Filter_Ha.Text = "Ha";
            RadioButton_KeywordsTab_ImageType_Filter_Ha.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordsTab_ImageType_Filter_Blue
            // 
            RadioButton_KeywordsTab_ImageType_Filter_Blue.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Filter_Blue.Location = new Point(167, 51);
            RadioButton_KeywordsTab_ImageType_Filter_Blue.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Filter_Blue.Name = "RadioButton_KeywordsTab_ImageType_Filter_Blue";
            RadioButton_KeywordsTab_ImageType_Filter_Blue.Size = new Size(48, 19);
            RadioButton_KeywordsTab_ImageType_Filter_Blue.TabIndex = 6;
            RadioButton_KeywordsTab_ImageType_Filter_Blue.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Filter_Blue.Text = "Blue";
            RadioButton_KeywordsTab_ImageType_Filter_Blue.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordsTab_ImageType_Filter_Green
            // 
            RadioButton_KeywordsTab_ImageType_Filter_Green.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Filter_Green.Location = new Point(95, 51);
            RadioButton_KeywordsTab_ImageType_Filter_Green.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Filter_Green.Name = "RadioButton_KeywordsTab_ImageType_Filter_Green";
            RadioButton_KeywordsTab_ImageType_Filter_Green.Size = new Size(56, 19);
            RadioButton_KeywordsTab_ImageType_Filter_Green.TabIndex = 5;
            RadioButton_KeywordsTab_ImageType_Filter_Green.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Filter_Green.Text = "Green";
            RadioButton_KeywordsTab_ImageType_Filter_Green.UseVisualStyleBackColor = true;
            // 
            // RadioButton_KeywordsTab_ImageType_Filter_O3
            // 
            RadioButton_KeywordsTab_ImageType_Filter_O3.AutoSize = true;
            RadioButton_KeywordsTab_ImageType_Filter_O3.Location = new Point(167, 21);
            RadioButton_KeywordsTab_ImageType_Filter_O3.Margin = new Padding(4, 3, 4, 3);
            RadioButton_KeywordsTab_ImageType_Filter_O3.Name = "RadioButton_KeywordsTab_ImageType_Filter_O3";
            RadioButton_KeywordsTab_ImageType_Filter_O3.Size = new Size(46, 19);
            RadioButton_KeywordsTab_ImageType_Filter_O3.TabIndex = 2;
            RadioButton_KeywordsTab_ImageType_Filter_O3.TabStop = true;
            RadioButton_KeywordsTab_ImageType_Filter_O3.Text = "O III";
            RadioButton_KeywordsTab_ImageType_Filter_O3.UseVisualStyleBackColor = true;
            // 
            // TabControl
            // 
            TabControl.Controls.Add(TabPage_Keywords);
            TabControl.Controls.Add(TabPage_Calibration);
            TabControl.Location = new Point(14, 282);
            TabControl.Margin = new Padding(4, 3, 4, 3);
            TabControl.Name = "TabControl";
            TabControl.SelectedIndex = 0;
            TabControl.Size = new Size(1147, 535);
            TabControl.TabIndex = 23;
            TabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            TabControl.Selecting += TabControl_Selecting;
            // 
            // CheckBox_VerifySha
            // 
            CheckBox_VerifySha.AutoSize = true;
            CheckBox_VerifySha.Location = new Point(272, 24);
            CheckBox_VerifySha.Name = "CheckBox_VerifySha";
            CheckBox_VerifySha.Size = new Size(81, 19);
            CheckBox_VerifySha.TabIndex = 26;
            CheckBox_VerifySha.Text = "Verify SHA";
            CheckBox_VerifySha.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1170, 827);
            Controls.Add(TabControl);
            Controls.Add(GroupBox_FileSelection);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "XISF File Manager";
            GroupBox_FileSelection_DirectorySelection.ResumeLayout(false);
            GroupBox_FileSelection_DirectorySelection.PerformLayout();
            GroupBox_FileSelection_DirectorySelection_FluxDensity.ResumeLayout(false);
            GroupBox_FileSelection_DirectorySelection_FluxDensity.PerformLayout();
            GroupBox_FileSelection_DirectorySelection_Masters.ResumeLayout(false);
            GroupBox_FileSelection_DirectorySelection_Masters.PerformLayout();
            GroupBox_FileSelection_Statistics.ResumeLayout(false);
            GroupBox_FileSelection_Statistics.PerformLayout();
            GroupBox_FileSelection.ResumeLayout(false);
            GroupBox_FileSelection.PerformLayout();
            TabPage_Calibration.ResumeLayout(false);
            TabPage_Calibration.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown_CalibrationTab_MinBackground).EndInit();
            GroupBox_CalibrationTab_MatchingTolerance.ResumeLayout(false);
            GroupBox_CalibrationTab_MatchingTolerance.PerformLayout();
            TabPage_Keywords.ResumeLayout(false);
            TabPage_Keywords.PerformLayout();
            GroupBox_KeywordUpdateTab_CaptureSoftware.ResumeLayout(false);
            GroupBox_KeywordUpdateTab_CaptureSoftware.PerformLayout();
            GroupBox_KeywordUpdateTab_Telescope.ResumeLayout(false);
            GroupBox_KeywordUpdateTab_Telescope.PerformLayout();
            GroupBox_KeywordUpdateTab_SubFrameKeywords.ResumeLayout(false);
            GroupBox_KeywordUpdateTab_SubFrameKeywords.PerformLayout();
            GroupBox_SubFrameKeywords_CalibrationFiles.ResumeLayout(false);
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.ResumeLayout(false);
            GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection.PerformLayout();
            GroupBox_KeywordUpdateTab_Camera.ResumeLayout(false);
            GroupBox_KeywordUpdateTab_Camera.PerformLayout();
            GroupBox_KeywordUpdateTab_ImageType.ResumeLayout(false);
            GroupBox_KeywordUpdateTab_ImageType_Frame.ResumeLayout(false);
            GroupBox_KeywordUpdateTab_ImageType_Frame.PerformLayout();
            GroupBox_KeywordUpdateTab_ImageType_Filter.ResumeLayout(false);
            GroupBox_KeywordUpdateTab_ImageType_Filter.PerformLayout();
            TabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.ProgressBar ProgressBar_FileSelection_ReadProgress;
        private System.Windows.Forms.GroupBox GroupBox_FileSelection_DirectorySelection;
        private System.Windows.Forms.CheckBox CheckBox_FileSelection_DirectorySelection_Masters_Enable;
        private System.Windows.Forms.Button Button_FileSelection_DirectorySelection_Browse;
        private System.Windows.Forms.CheckBox CheckBox_FileSelection_DirectorySelection_Recurse;
        private System.Windows.Forms.Button Button_FileSelection_DirectorySelection_Rename;
        private System.Windows.Forms.GroupBox GroupBox_FileSelection_Statistics;
        private System.Windows.Forms.Label Label_FileSelection_Statistics_OperationStatus;
        private System.Windows.Forms.Label Label_FileSelection_Statistics_SubFrameOverhead;
        private System.Windows.Forms.Label Label_FileSelection_Statistics_TempratureCoefficient;
        private System.Windows.Forms.Label Label_FileSelection_BrowseFileName;
        private System.Windows.Forms.GroupBox GroupBox_FileSelection;
        private System.Windows.Forms.CheckBox CheckBox_FileSlection_DirectorySelection_NoStatistics;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TabPage TabPage_Calibration;
        private System.Windows.Forms.CheckBox CheckBox_CalibrationTab_CreateNew;
        private System.Windows.Forms.TreeView TreeView_CalibrationTab_TargetFileTree;
        private System.Windows.Forms.TextBox TextBox_CalibrationTab_Messgaes;
        private System.Windows.Forms.GroupBox GroupBox_CalibrationTab_MatchingTolerance;
        private System.Windows.Forms.Label Label_CalibrationTab_MatchingTolerance_TemperatureDegrees;
        private System.Windows.Forms.Label Label_CalibrationTab_MatchingTolerance_OffsetADU;
        private System.Windows.Forms.Label Label_CalibrationTab_MatchingTolerance_GainUnits;
        private System.Windows.Forms.Label Label_CalibrationTab_MatchingTolerance_ExposureSeconds;
        private System.Windows.Forms.Label Label_CalibrationTab_MatchingTolerance_Percentage;
        private System.Windows.Forms.TextBox TextBox_CalibrationTab_MatchingTolerance_Temperature;
        private System.Windows.Forms.TextBox TextBox_CalibrationTab_MatchingTolerance_Offset;
        private System.Windows.Forms.TextBox TextBox_CalibrationTab_MatchingTolerance_Gain;
        private System.Windows.Forms.Label Label_CalibrationTab_MatchingTolerance_Temperature;
        private System.Windows.Forms.Label Label_CalibrationTab_MatchingTolerance_Offset;
        private System.Windows.Forms.Label Label_CalibrationTab_MatchingTolerance_Gain;
        private System.Windows.Forms.Label Label_CalibrationTab_MatchingTolerance_Exposure;
        private System.Windows.Forms.TextBox TextBox_CalibrationTab_MatchingTolerance_Exposure;
        private System.Windows.Forms.Label Label_CalibrationTab_TotalFiles;
        private System.Windows.Forms.ProgressBar ProgressBar_CalibrationTab;
        private System.Windows.Forms.Label Label_CalibrationTab_ReadFileName;
        private System.Windows.Forms.Button Button_CalibrationTab_CreateCalibrationDirectory;
        private System.Windows.Forms.Button Button_CalibrationTab_MatchCalibrationFrames;
        private System.Windows.Forms.Button Button_CalibrationTab_FindCalibrationFrames;
        private System.Windows.Forms.TabPage TabPage_Keywords;
        private System.Windows.Forms.Label Label_KeywordUpdateTab_FileName;
        private System.Windows.Forms.ProgressBar ProgressBar_KeywordUpdateTab_WriteProgress;
        private System.Windows.Forms.GroupBox GroupBox_KeywordUpdateTab_CaptureSoftware;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_CaptureSoftware_NINA;
        private System.Windows.Forms.Button Button_KeywordUpdateTab_CaptureSoftware_SetByFile;
        private System.Windows.Forms.Button Button_KeywordUpdateTab_CaptureSoftware_SetAll;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_CaptureSoftware_Voyager;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_CaptureSoftware_SharpCap;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_CaptureSoftware_SGPro;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_CaptureSoftware_TheSkyX;
        private System.Windows.Forms.GroupBox GroupBox_KeywordUpdateTab_Telescope;
        private System.Windows.Forms.ComboBox TextBox_KeywordUpdateTab_Telescope_FocalLength;
        private System.Windows.Forms.Label Label_KeywordUpdateTab_Telescope_FocalLength;
        private System.Windows.Forms.Button Button_KeywordUpdateTab_Telescope_SetByFile;
        private System.Windows.Forms.Button Button_KeywordUpdateTab_Telescope_SetAll;
        private System.Windows.Forms.CheckBox CheckBox_KeywordUpdateTab_Telescope_Riccardi;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_Telescope_Newtonian254;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_Telescope_EvoStar150;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_Telescope_APM107;
        private System.Windows.Forms.GroupBox GroupBox_KeywordUpdateTab_SubFrameKeywords;
        private System.Windows.Forms.CheckBox CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdatePanelName;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_SubFrameKeywords_SpecificValue;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_SubFrameKeywords_AllValues;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordComment;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordValue;
        private System.Windows.Forms.GroupBox GroupBox_SubFrameKeywords_CalibrationFiles;
        private System.Windows.Forms.Button Button_SubFrameKeywords_CalibrationFiles_ClearAll;
        private System.Windows.Forms.GroupBox GroupBox_KeywordUpdateTab_SubFrameKeywords_KeywordProtection;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_UpdateNew;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Force;
        private System.Windows.Forms.CheckBox CheckBox_KeywordUpdateTab_SubFrameKeywords_AlphabetizeKeywords;
        private System.Windows.Forms.Button Button_KeywordUpdateTab_SubFrameKeywords_Delete;
        private System.Windows.Forms.Button Button_KeywordUpdateTab_SubFrameKeywords_AddReplace;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordName;
        private System.Windows.Forms.CheckBox CheckBox_KeywordUpdateTab_SubFrameKeywords_UpdateTargetName;
        private System.Windows.Forms.Button Button_KeywordUpdateTab_SubFrameKeywords_UpdateKeywords;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_SubFrameKeywords_TargetNames;
        private System.Windows.Forms.Label Label_KeywordUpdateTab_SubFrameKeywords_TagetName;
        private System.Windows.Forms.GroupBox GroupBox_KeywordUpdateTab_Camera;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_A144Binning;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Q178Binning;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Z183Binning;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Z533Binning;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_A144SensorTemp;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Q178SensorTemp;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Z183SensorTemp;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Z533SensorTemp;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Q178Offset;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Z183Offset;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Z533Offset;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Q178Gain;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Z183Gain;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Z533Gain;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_A144Seconds;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Q178Seconds;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Z533Seconds;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_Camera_Z183Seconds;
        private System.Windows.Forms.Label Label_KeywordUpdateTab_Camera_ToggleNBPreset;
        private System.Windows.Forms.Label Label_KeywordUpdateTab_Camera_Seconds;
        private System.Windows.Forms.Button Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB;
        private System.Windows.Forms.CheckBox CheckBox_KeywordUpdateTab_Camera_A144;
        private System.Windows.Forms.CheckBox CheckBox_KeywordUpdateTab_Camera_Q178;
        private System.Windows.Forms.CheckBox CheckBox_KeywordUpdateTab_Camera_Z183;
        private System.Windows.Forms.CheckBox CheckBox_KeywordUpdateTab_Camera_Z533;
        private System.Windows.Forms.Label Label_KeywordUpdateTab_Camera_Binning;
        private System.Windows.Forms.Label Label_KeywordUpdateTab_Camera_SensorTemp;
        private System.Windows.Forms.Label Label_KeywordUpdateTab_Camera_Camera;
        private System.Windows.Forms.Button Button_KeywordUpdateTab_Camera_SetByFile;
        private System.Windows.Forms.Button Button_KeywordUpdateTab_Camera_SetAll;
        private System.Windows.Forms.Label Label_KeywordUpdateTab_Camera_A144Gain;
        private System.Windows.Forms.Label Label_KeywordUpdateTab_Camera_Offset;
        private System.Windows.Forms.Label Label_KeywordUpdateTab_Camera_Gain;
        private System.Windows.Forms.GroupBox GroupBox_KeywordUpdateTab_ImageType;
        private System.Windows.Forms.Button Button_KeywordsTab_ImageType_SetByFile;
        private System.Windows.Forms.Button Button_KeywordsTab_ImageType_SetAll;
        private System.Windows.Forms.GroupBox GroupBox_KeywordUpdateTab_ImageType_Frame;
        private System.Windows.Forms.Button Button_KeywordsTab_ImageType_Frame_SetMaster;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Frame_Bias;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Frame_Flat;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Frame_Dark;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Frame_Light;
        private System.Windows.Forms.GroupBox GroupBox_KeywordUpdateTab_ImageType_Filter;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Filter_Luma;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Filter_Shutter;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Filter_Red;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Filter_S2;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Filter_Ha;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Filter_Blue;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Filter_Green;
        private System.Windows.Forms.RadioButton RadioButton_KeywordsTab_ImageType_Filter_O3;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.Button Button_KeywordUpdateTab_Cancel;
        private System.Windows.Forms.CheckBox CheckBox_CalibrationTab_MatchingTolerance_TemperatureNearest;
        private System.Windows.Forms.CheckBox CheckBox_CalibrationTab_MatchingTolerance_OffsetNearest;
        private System.Windows.Forms.CheckBox CheckBox_CalibrationTab_MatchingTolerance_GainNearest;
        private System.Windows.Forms.CheckBox CheckBox_CalibrationTab_MatchingTolerance_ExposureNearest;
        private System.Windows.Forms.RadioButton RadioButton_KeywordUpdateTab_SubFrameKeywords_KeywordProtection_Protect;
        private System.Windows.Forms.ComboBox ComboBox_KeywordUpdateTab_SubFrameKeywords_KeywordFile;
        private System.Windows.Forms.TextBox TextBox_FileSelection_DirectorySelection_Masters_Frames;
        private System.Windows.Forms.Label Label_CalibrationTab_Pedestal;
        private System.Windows.Forms.NumericUpDown NumericUpDown_CalibrationTab_MinBackground;
        private System.Windows.Forms.Button Button_CalibrationTab_FindPedestal;
        private System.Windows.Forms.Label Label_CalibrationTab_Minimum;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox TextBox_FileSelection_DirectorySelection_Masters_Rejection;
        private System.Windows.Forms.CheckBox CheckBox_FileSelection_DirectorySelection_FluxDensity_Enable;
        private System.Windows.Forms.Button Button_FileSelection_DirectorySelection_FluxDensity_Run;
        private System.Windows.Forms.GroupBox GroupBox_FileSelection_DirectorySelection_Masters;
        private System.Windows.Forms.GroupBox GroupBox_FileSelection_DirectorySelection_FluxDensity;
        private CheckBox CheckBox_FileSelection_DirectorySelection_CalibrationIds;
        private CheckBox CheckBox_Solver;
        private CheckBox CheckBox_VerifySha;
    }
}