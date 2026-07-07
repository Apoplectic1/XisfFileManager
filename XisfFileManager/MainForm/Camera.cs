using XisfFileManager.Files;
using XisfFileManager.Helpers;
using XisfFileManager.Models;
using XisfFileManager.Models.Cameras;
using XisfFileManager.Services;

namespace XisfFileManager;

public partial class MainForm
{
    // Camera checkbox and combobox mappings
    private Dictionary<CameraConfiguration, CheckBox>? _cameraCheckboxes;
    private Dictionary<CameraConfiguration, ComboBox>? _cameraSecondsComboBoxes;
    private Dictionary<CameraConfiguration, ComboBox>? _cameraGainComboBoxes;
    private Dictionary<CameraConfiguration, ComboBox>? _cameraOffsetComboBoxes;
    private Dictionary<CameraConfiguration, ComboBox>? _cameraBinningComboBoxes;
    private Dictionary<CameraConfiguration, ComboBox>? _cameraTempComboBoxes;

    private void InitializeCameraMappings()
    {
        _cameraCheckboxes = new Dictionary<CameraConfiguration, CheckBox>
        {
            { Z533Camera.Instance, CheckBox_KeywordUpdateTab_Camera_Z533 },
            { Z183Camera.Instance, CheckBox_KeywordUpdateTab_Camera_Z183 },
            { Q178Camera.Instance, CheckBox_KeywordUpdateTab_Camera_Q178 },
            { A144Camera.Instance, CheckBox_KeywordUpdateTab_Camera_A144 }
        };

        _cameraSecondsComboBoxes = new Dictionary<CameraConfiguration, ComboBox>
        {
            { Z533Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Z533Seconds },
            { Z183Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Z183Seconds },
            { Q178Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Q178Seconds },
            { A144Camera.Instance, ComboBox_KeywordUpdateTab_Camera_A144Seconds }
        };

        _cameraGainComboBoxes = new Dictionary<CameraConfiguration, ComboBox>
        {
            { Z533Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Z533Gain },
            { Z183Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Z183Gain },
            { Q178Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Q178Gain }
        };

        _cameraOffsetComboBoxes = new Dictionary<CameraConfiguration, ComboBox>
        {
            { Z533Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Z533Offset },
            { Z183Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Z183Offset },
            { Q178Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Q178Offset }
        };

        _cameraBinningComboBoxes = new Dictionary<CameraConfiguration, ComboBox>
        {
            { Z533Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Z533Binning },
            { Z183Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Z183Binning },
            { Q178Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Q178Binning },
            { A144Camera.Instance, ComboBox_KeywordUpdateTab_Camera_A144Binning }
        };

        _cameraTempComboBoxes = new Dictionary<CameraConfiguration, ComboBox>
        {
            { Z533Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Z533SensorTemp },
            { Z183Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Z183SensorTemp },
            { Q178Camera.Instance, ComboBox_KeywordUpdateTab_Camera_Q178SensorTemp },
            { A144Camera.Instance, ComboBox_KeywordUpdateTab_Camera_A144SensorTemp }
        };
    }

    private void ClearCameraGroup()
    {
        // Ensure mappings are initialized
        if (_cameraCheckboxes == null)
            InitializeCameraMappings();

        // Clear labels
        Label_KeywordUpdateTab_Camera_Camera.ForeColor = Color.Black;
        Label_KeywordUpdateTab_Camera_SensorTemp.ForeColor = Color.Black;
        Label_KeywordUpdateTab_Camera_Gain.ForeColor = Color.Black;
        Label_KeywordUpdateTab_Camera_Offset.ForeColor = Color.Black;
        Label_KeywordUpdateTab_Camera_Binning.ForeColor = Color.Black;
        Label_KeywordUpdateTab_Camera_Seconds.ForeColor = Color.Black;

        // Clear checkboxes
        foreach (var cb in _cameraCheckboxes!.Values)
        {
            cb.Checked = false;
            cb.ForeColor = Color.Black;
        }

        // Clear all comboboxes
        foreach (var cb in _cameraSecondsComboBoxes!.Values) UIHelpers.ClearComboBox(cb);
        foreach (var cb in _cameraGainComboBoxes!.Values) UIHelpers.ClearComboBox(cb);
        foreach (var cb in _cameraOffsetComboBoxes!.Values) UIHelpers.ClearComboBox(cb);
        foreach (var cb in _cameraBinningComboBoxes!.Values) UIHelpers.ClearComboBox(cb);
        foreach (var cb in _cameraTempComboBoxes!.Values) UIHelpers.ClearComboBox(cb);

        Button_KeywordUpdateTab_Camera_SetAll.ForeColor = Color.Black;
        Button_KeywordUpdateTab_Camera_SetByFile.ForeColor = Color.Black;
    }

    public void FindCamera()
    {
        ClearCameraGroup();

        var cameraNames = mFileList.Select(c => c.Camera).ToList();
        var detectedCameras = CameraService.DetectCameras(mFileList);

        bool bNoCameras = cameraNames.Count == 0;
        bool bMissingCameras = cameraNames.Count != mFileList.Count && !bNoCameras;
        int foundCount = detectedCameras.Count(kv => kv.Value);
        bool bDifferentCameras = foundCount >= 2 && !bMissingCameras;

        // Update checkboxes
        foreach (var (camera, checkbox) in _cameraCheckboxes!)
        {
            bool found = detectedCameras[camera];
            checkbox.Checked = found;
            checkbox.ForeColor = CameraService.GetCheckboxColor(found, bNoCameras, bMissingCameras, bDifferentCameras);
        }

        // Analyze properties for each camera
        var secondsAnalyses = new Dictionary<CameraConfiguration, PropertyAnalysis<double>>();
        var gainAnalyses = new Dictionary<CameraConfiguration, PropertyAnalysis<int>>();
        var offsetAnalyses = new Dictionary<CameraConfiguration, PropertyAnalysis<int>>();
        var tempAnalyses = new Dictionary<CameraConfiguration, PropertyAnalysis<double>>();
        var binningAnalyses = new Dictionary<CameraConfiguration, PropertyAnalysis<int>>();

        foreach (var camera in CameraService.AllCameras)
        {
            if (!detectedCameras[camera]) continue;

            // Exposure Seconds — presence-based: a missing exposure keyword maps to the excluded -1
            // sentinel (the getter's 0.0 would masquerade as a genuine 0s bias frame)
            var secondsAnalysis = CameraService.AnalyzeDoubleProperty(mFileList, camera, f => f.HasExposure ? f.ExposureSeconds : -1, -1);
            secondsAnalyses[camera] = secondsAnalysis;
            BindAnalysisToComboBox(_cameraSecondsComboBoxes![camera], secondsAnalysis);

            // Gain (only for cameras with gain control)
            if (camera.HasGain && _cameraGainComboBoxes!.TryGetValue(camera, out var gainCb))
            {
                var gainAnalysis = CameraService.AnalyzeIntProperty(mFileList, camera, f => f.Gain, 0);
                gainAnalyses[camera] = gainAnalysis;
                BindAnalysisToComboBox(gainCb, gainAnalysis);
            }

            // Offset (only for cameras with offset control)
            if (camera.HasOffset && _cameraOffsetComboBoxes!.TryGetValue(camera, out var offsetCb))
            {
                var offsetAnalysis = CameraService.AnalyzeIntProperty(mFileList, camera, f => f.Offset, 0);
                offsetAnalyses[camera] = offsetAnalysis;
                BindAnalysisToComboBox(offsetCb, offsetAnalysis);
            }

            // Temperature
            var tempAnalysis = CameraService.AnalyzeTemperature(mFileList, camera);
            tempAnalyses[camera] = tempAnalysis;
            BindAnalysisToComboBox(_cameraTempComboBoxes![camera], tempAnalysis);

            // Binning
            var binningAnalysis = CameraService.AnalyzeIntProperty(mFileList, camera, f => f.Binning, 1);
            binningAnalyses[camera] = binningAnalysis;
            BindAnalysisToComboBox(_cameraBinningComboBoxes![camera], binningAnalysis);
        }

        // Camera-identity header, same convention as the value headers: red = any file with a
        // missing/blank/unknown INSTRUME, green = all resolved across 2+ cameras, black = one camera
        var (anyUnresolved, multipleCameras) = CameraService.AnalyzeCameraResolution(mFileList);
        Label_KeywordUpdateTab_Camera_Camera.ForeColor = CameraService.GetLabelColor(anyUnresolved, false, multipleCameras);

        // Update aggregate label colors
        UpdateLabelFromAnalyses(Label_KeywordUpdateTab_Camera_Seconds, secondsAnalyses);
        UpdateLabelFromAnalyses(Label_KeywordUpdateTab_Camera_Gain, gainAnalyses);
        UpdateLabelFromAnalyses(Label_KeywordUpdateTab_Camera_Offset, offsetAnalyses);
        UpdateLabelFromAnalyses(Label_KeywordUpdateTab_Camera_SensorTemp, tempAnalyses);
        UpdateLabelFromAnalyses(Label_KeywordUpdateTab_Camera_Binning, binningAnalyses);
    }

    private static void BindAnalysisToComboBox<T>(ComboBox cb, PropertyAnalysis<T> analysis)
    {
        cb.DataSource = analysis.DistinctValues;
        cb.ForeColor = CameraService.GetComboBoxColor(analysis);
        cb.SelectedIndex = CameraService.GetComboBoxSelectedIndex(analysis);
    }

    private static void UpdateLabelFromAnalyses<T>(Label label, Dictionary<CameraConfiguration, PropertyAnalysis<T>> analyses)
    {
        bool anyNoOrMissing = analyses.Values.Any(a => a.NoValues || a.MissingValues);
        bool anyDifferent = analyses.Values.Any(a => a.DifferentValues);
        label.ForeColor = CameraService.GetLabelColor(anyNoOrMissing, false, anyDifferent);
    }

    private void Button_KeywordUpdateSubFrameKeywordsCamera_ToggleNB_Click(object sender, EventArgs e)
    {
        var selectedCamera = GetSelectedCamera();
        if (selectedCamera == null) return;

        bool isNbPreset = Label_KeywordUpdateTab_Camera_ToggleNBPreset.Text == "NB Preset";
        Label_KeywordUpdateTab_Camera_ToggleNBPreset.Text = isNbPreset ? "BB Preset" : "NB Preset";

        var preset = isNbPreset ? selectedCamera.BroadbandPreset : selectedCamera.NarrowbandPreset;

        if (_cameraGainComboBoxes!.TryGetValue(selectedCamera, out var gainCb))
            gainCb.Text = preset.Gain.ToString();
        if (_cameraOffsetComboBoxes!.TryGetValue(selectedCamera, out var offsetCb))
            offsetCb.Text = preset.Offset.ToString();
    }

    private CameraConfiguration? GetSelectedCamera()
    {
        if (_cameraCheckboxes == null) return null;
        return _cameraCheckboxes.FirstOrDefault(kv => kv.Value.Checked).Key;
    }

    private int GetCheckedCameraCount() =>
        _cameraCheckboxes?.Count(kv => kv.Value.Checked) ?? 0;

    private void Button_KeywordCamera_SetAll_Click(object sender, EventArgs e)
    {
        int checkedCount = GetCheckedCameraCount();
        if (mFileList.Count == 0 || checkedCount == 0)
            return;

        var checkedCameras = _cameraCheckboxes!
            .Where(kv => kv.Value.Checked).Select(kv => kv.Key).ToList();

        foreach (XisfFile file in mFileList)
        {
            // One camera checked: force identity onto every file (assert "all are this camera").
            // Two or more checked: route each file to the checked camera its INSTRUME matches.
            CameraConfiguration? fileCamera = checkedCount == 1
                ? checkedCameras[0]
                : checkedCameras.FirstOrDefault(c => c.MatchesCamera(file.Camera));

            // Multi-camera load: file matches no checked camera by keyword -> left untouched.
            // Surfacing a skipped count is a ROADMAP follow-up.
            if (fileCamera == null)
                continue;

            // Common keywords
            file.RemoveKeyword("NAXIS3");
            file.AddKeyword("BITPIX", "16", "Bits Per Pixel");
            file.AddKeyword("BSCALE", "1", "Multiply Raw Values by BSCALE");
            file.AddKeyword("BZERO", "32768", "Add value to scale to 65536 (16 bit) values");
            file.AddKeyword("NAXIS", "2", "XISF File Manager");

            // Apply camera-specific keywords + UI values for that camera's row
            fileCamera.ApplyKeywords(file);
            ApplyCameraValuesFromUI(file, fileCamera);
        }

        FindCamera();
    }

    private void ApplyCameraValuesFromUI(XisfFile file, CameraConfiguration camera)
    {
        // Exposure seconds
        if (_cameraSecondsComboBoxes!.TryGetValue(camera, out var secondsCb) &&
            double.TryParse(secondsCb.Text, out double seconds))
        {
            file.ExposureSeconds = seconds;
        }

        // Gain (if supported)
        if (camera.HasGain &&
            _cameraGainComboBoxes!.TryGetValue(camera, out var gainCb) &&
            int.TryParse(gainCb.Text, out int gain))
        {
            file.Gain = gain;
        }

        // Offset (if supported)
        if (camera.HasOffset &&
            _cameraOffsetComboBoxes!.TryGetValue(camera, out var offsetCb) &&
            int.TryParse(offsetCb.Text, out int offset))
        {
            file.Offset = offset;
        }

        // Binning
        if (_cameraBinningComboBoxes!.TryGetValue(camera, out var binningCb) &&
            int.TryParse(binningCb.Text, out int binning))
        {
            file.Binning = binning;
            // Update pixel size based on new binning
            var pixelSize = camera.GetPixelSizeForBinning(binning).ToString("F2");
            file.AddKeyword("XPIXSZ", pixelSize, "Sensor Horizontal Pixel Size in Microns");
            file.AddKeyword("YPIXSZ", pixelSize, "Sensor Vertical Pixel Size in Microns");
        }

        // Temperature
        if (_cameraTempComboBoxes!.TryGetValue(camera, out var tempCb) &&
            double.TryParse(tempCb.Text, out double temp))
        {
            camera.SetTemperature(file, temp);
        }
    }

    private void Button_KeywordCamera_SetByFile_Click(object sender, EventArgs e)
    {
        if (mFileList.Count == 0)
            return;

        var selectedCamera = GetSelectedCamera();

        foreach (XisfFile xFile in mFileList)
        {
            // Common keywords
            xFile.RemoveKeyword("NAXIS3");
            xFile.RemoveKeyword("EXPOSURE");
            xFile.AddKeyword("BITPIX", "16", "Bits Per Pixel");
            xFile.AddKeyword("BSCALE", "1", "Multiply Raw Values by BSCALE");
            xFile.AddKeyword("BZERO", "32768", "Add value to scale to 65536 (16 bit) values");
            xFile.AddKeyword("NAXIS", "2", "XISF File Manager");

            // Determine which camera to use for this file
            var fileCamera = selectedCamera ?? CameraService.AllCameras.FirstOrDefault(c => c.MatchesCamera(xFile.Camera));
            if (fileCamera == null) continue;

            // Apply camera keywords
            fileCamera.ApplyKeywords(xFile);

            // Use file's existing values or fall back to UI values
            ApplyFileOrUIValues(xFile, fileCamera);
        }

        FindCamera();
    }

    private void ApplyFileOrUIValues(XisfFile file, CameraConfiguration camera)
    {
        // Temperature - use file value if valid, otherwise UI value
        double fileTemp = camera.GetTemperature(file);
        if (fileTemp == -273 || fileTemp == 0)
        {
            if (_cameraTempComboBoxes!.TryGetValue(camera, out var tempCb) &&
                double.TryParse(tempCb.Text, out double uiTemp))
            {
                camera.SetTemperature(file, uiTemp);
            }
        }
        file.AddKeyword("CCD-TEMP", camera.GetTemperature(file).ToString(), "Actual Sensor Temperature");

        // Binning - use file value if valid, otherwise UI value
        if (file.Binning <= 0)
        {
            if (_cameraBinningComboBoxes!.TryGetValue(camera, out var binningCb) &&
                int.TryParse(binningCb.Text, out int uiBinning))
            {
                file.Binning = uiBinning;
            }
        }

        // Exposure seconds
        if (file.ExposureSeconds <= 0)
        {
            if (_cameraSecondsComboBoxes!.TryGetValue(camera, out var secondsCb) &&
                double.TryParse(secondsCb.Text, out double seconds))
            {
                file.ExposureSeconds = seconds;
            }
        }

        // Gain
        if (camera.HasGain)
        {
            if (file.Gain < 0 && _cameraGainComboBoxes!.TryGetValue(camera, out var gainCb) &&
                int.TryParse(gainCb.Text, out int uiGain))
            {
                file.Gain = uiGain;
            }
        }

        // Offset
        if (camera.HasOffset)
        {
            if (file.Offset < 0 && _cameraOffsetComboBoxes!.TryGetValue(camera, out var offsetCb) &&
                int.TryParse(offsetCb.Text, out int uiOffset))
            {
                file.Offset = uiOffset;
            }
        }
        else
        {
            file.RemoveKeyword("GAIN");
            file.RemoveKeyword("OFFSET");
        }
    }
}
