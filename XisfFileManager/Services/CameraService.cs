using System.Drawing;
using XisfFileManager.Files;
using XisfFileManager.Models;
using XisfFileManager.Models.Cameras;

namespace XisfFileManager.Services;

/// <summary>
/// Service for camera detection, analysis, and keyword management
/// </summary>
public class CameraService
{
    /// <summary>
    /// All registered camera configurations
    /// </summary>
    public static IReadOnlyList<CameraConfiguration> AllCameras { get; } =
    [
        Z533Camera.Instance,
        Z183Camera.Instance,
        Q178Camera.Instance,
        A144Camera.Instance
    ];

    /// <summary>
    /// Find camera configuration by identifier
    /// </summary>
    public static CameraConfiguration? GetCamera(string identifier) =>
        AllCameras.FirstOrDefault(c => c.Identifier == identifier);

    /// <summary>
    /// Detect which cameras are present in a file list
    /// </summary>
    public static Dictionary<CameraConfiguration, bool> DetectCameras(IEnumerable<XisfFile> files)
    {
        var cameraNames = files.Select(f => f.Camera).ToList();
        return AllCameras.ToDictionary(
            camera => camera,
            camera => cameraNames.Any(name => camera.MatchesCamera(name)));
    }

    /// <summary>
    /// Analyze camera-identity resolution across a file list. A file is unresolved when its
    /// INSTRUME/Camera value is missing, blank, or matches no registered camera model.
    /// </summary>
    public static (bool AnyUnresolved, bool MultipleCameras) AnalyzeCameraResolution(IEnumerable<XisfFile> files)
    {
        bool anyUnresolved = false;
        var resolvedCameras = new HashSet<CameraConfiguration>();

        foreach (var file in files)
        {
            var camera = string.IsNullOrWhiteSpace(file.Camera)
                ? null
                : AllCameras.FirstOrDefault(c => c.MatchesCamera(file.Camera));

            if (camera == null)
                anyUnresolved = true;
            else
                resolvedCameras.Add(camera);
        }

        return (anyUnresolved, resolvedCameras.Count > 1);
    }

    /// <summary>
    /// Get files matching a specific camera
    /// </summary>
    public static IEnumerable<XisfFile> GetFilesForCamera(IEnumerable<XisfFile> files, CameraConfiguration camera) =>
        files.Where(f => camera.MatchesCamera(f.Camera));

    /// <summary>
    /// Count files matching a specific camera
    /// </summary>
    public static int CountFilesForCamera(IEnumerable<XisfFile> files, CameraConfiguration camera) =>
        files.Count(f => camera.MatchesCamera(f.Camera));

    /// <summary>
    /// Analyze a numeric property (double) for a specific camera
    /// </summary>
    public static PropertyAnalysis<double> AnalyzeDoubleProperty(
        IEnumerable<XisfFile> files,
        CameraConfiguration camera,
        Func<XisfFile, double> selector,
        double invalidValue = -273)
    {
        var cameraFiles = GetFilesForCamera(files, camera).ToList();
        var validValues = cameraFiles
            .Select(selector)
            .Where(v => v != invalidValue && v >= 0)
            .ToList();

        int totalCameraFiles = cameraFiles.Count;
        bool noValues = validValues.Count == 0;
        bool missingValues = validValues.Count != totalCameraFiles && !noValues;
        bool differentValues = validValues.Distinct().Count() > 1 && !missingValues;
        bool uniqueValue = !missingValues && !differentValues && !noValues;

        return new PropertyAnalysis<double>
        {
            NoValues = noValues,
            MissingValues = missingValues,
            DifferentValues = differentValues,
            UniqueValue = uniqueValue,
            DistinctValues = validValues.OrderBy(v => v).Distinct().ToList(),
            TotalCameraFiles = totalCameraFiles
        };
    }

    /// <summary>
    /// Analyze a numeric property (int) for a specific camera
    /// </summary>
    public static PropertyAnalysis<int> AnalyzeIntProperty(
        IEnumerable<XisfFile> files,
        CameraConfiguration camera,
        Func<XisfFile, int> selector,
        int minValidValue = 0)
    {
        var cameraFiles = GetFilesForCamera(files, camera).ToList();
        var validValues = cameraFiles
            .Select(selector)
            .Where(v => v >= minValidValue)
            .ToList();

        int totalCameraFiles = cameraFiles.Count;
        bool noValues = validValues.Count == 0;
        bool missingValues = validValues.Count != totalCameraFiles && !noValues;
        bool differentValues = validValues.Distinct().Count() > 1 && !missingValues;
        bool uniqueValue = !missingValues && !differentValues && !noValues;

        return new PropertyAnalysis<int>
        {
            NoValues = noValues,
            MissingValues = missingValues,
            DifferentValues = differentValues,
            UniqueValue = uniqueValue,
            DistinctValues = validValues.OrderBy(v => v).Distinct().ToList(),
            TotalCameraFiles = totalCameraFiles
        };
    }

    /// <summary>
    /// Analyze temperature property for a camera (uses correct temp source based on camera type)
    /// Allows negative temperatures since cooled cameras typically operate below 0°C
    /// </summary>
    public static PropertyAnalysis<double> AnalyzeTemperature(
        IEnumerable<XisfFile> files,
        CameraConfiguration camera)
    {
        const double InvalidTemperature = -273; // Absolute zero = invalid/missing

        var cameraFiles = GetFilesForCamera(files, camera).ToList();
        var validValues = cameraFiles
            .Select(f => camera.GetTemperature(f))
            .Where(v => v != InvalidTemperature) // Allow negative temps, just not absolute zero
            .ToList();

        int totalCameraFiles = cameraFiles.Count;
        bool noValues = validValues.Count == 0;
        bool missingValues = validValues.Count != totalCameraFiles && !noValues;
        bool differentValues = validValues.Distinct().Count() > 1 && !missingValues;
        bool uniqueValue = !missingValues && !differentValues && !noValues;

        return new PropertyAnalysis<double>
        {
            NoValues = noValues,
            MissingValues = missingValues,
            DifferentValues = differentValues,
            UniqueValue = uniqueValue,
            DistinctValues = validValues.OrderBy(v => v).Distinct().ToList(),
            TotalCameraFiles = totalCameraFiles
        };
    }

    /// <summary>
    /// Get the appropriate color for a label based on property analysis
    /// </summary>
    public static Color GetLabelColor(bool noValues, bool missingValues, bool differentValues)
    {
        if (noValues || missingValues)
            return Color.Red;
        if (differentValues)
            return Color.Green;
        return Color.Black;
    }

    /// <summary>
    /// Get the appropriate color for a ComboBox based on property analysis
    /// </summary>
    public static Color GetComboBoxColor<T>(PropertyAnalysis<T> analysis)
    {
        if (analysis.UniqueValue)
            return Color.Black;
        if (!analysis.MissingValues && analysis.DifferentValues)
            return Color.Green;
        if (analysis.MissingValues && !analysis.DifferentValues)
            return Color.DarkViolet;
        return Color.Black;
    }

    /// <summary>
    /// Get checkbox color based on camera detection state
    /// </summary>
    public static Color GetCheckboxColor(bool found, bool noCameras, bool missingCameras, bool differentCameras)
    {
        if (noCameras)
            return Color.Red;
        if (missingCameras)
            return found ? Color.DarkViolet : Color.Red;
        if (differentCameras)
            return found ? Color.Green : Color.Black;
        return Color.Black;
    }

    /// <summary>
    /// Get the selected index for a ComboBox based on property analysis
    /// </summary>
    public static int GetComboBoxSelectedIndex<T>(PropertyAnalysis<T> analysis)
    {
        if (analysis.DistinctValues.Count == 0)
            return -1;
        if (analysis.UniqueValue || (analysis.MissingValues && !analysis.DifferentValues))
            return 0;
        return -1;
    }
}
