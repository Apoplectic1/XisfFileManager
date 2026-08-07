using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Astronomy.Diagnostics;
using XisfFileManager.Globals;
using XisfFileManager.Helpers;

namespace XisfFileManager.Files;

public class XisfFileRename
{
    private const double NoTemperature = -273.0;

    public bool IncludeCalibrationFrames { get; set; }

    /// <summary>
    /// Renames the specified XISF file in place based on its properties and index.
    /// </summary>
    /// <param name="xFile">The XISF file to rename.</param>
    /// <returns>
    /// NotRenamed: true when a needed rename did not happen (target name taken by another file, or
    /// an error — both logged); false on success or when the name was already correct.
    /// FilePath: the file's on-disk path after the operation.
    /// </returns>
    public (bool NotRenamed, string FilePath) RenameFile(XisfFile xFile)
    {
        try
        {
            string sourceFileDirectory = Path.GetDirectoryName(xFile.FilePath) ?? "";
            string newFileName = BuildFileName(xFile.FileNameNumberIndex, xFile) + ".xisf";

            if (sourceFileDirectory.Contains("reject", StringComparison.OrdinalIgnoreCase))
            {
                newFileName = "REJECT  " + newFileName;
            }

            string newFilePath = Path.Combine(sourceFileDirectory, newFileName);

            if (xFile.FilePath == newFilePath)
                return (false, newFilePath); // already named correctly

            if (File.Exists(newFilePath))
            {
                Log.Warn($"Rename skipped (target exists): {Path.GetFileName(xFile.FilePath)} -> {newFileName}");
                return (true, xFile.FilePath);
            }

            File.Move(xFile.FilePath, newFilePath);
            return (false, newFilePath);
        }
        catch (Exception ex)
        {
            Log.Error($"Rename failed: {Path.GetFileName(xFile.FilePath)}", ex);
            return (true, xFile.FilePath);
        }
    }

    /// <summary>
    /// Moves duplicate files in the given list to a 'Duplicates' directory.
    /// A duplicate is defined as a file with identical properties to another file in the list.
    /// </summary>
    /// <param name="fileList">The list of XisfFile objects to check for duplicates.</param>
    /// <returns>The count of files that were moved as duplicates.</returns>
    public static int MoveDuplicates(List<XisfFile> fileList)
    {
        // Group files by their properties to identify duplicates
        var groupedDuplicates = fileList.GroupBy(item => new
        {
            item.Camera,
            item.FrameType,
            item.Binning,
            item.FilterName,
            item.ExposureSeconds,
            item.Gain,
            item.Offset,
            item.CaptureTime
        })
        .Where(group => group.Skip(1).Any()) // Only keep groups with more than one item
        .ToList();

        // List to keep track of files that have been moved
        List<XisfFile> movedFiles = [];

        // Process each group of duplicates
        groupedDuplicates.ForEach(group =>
        {
            var items = group.ToList();
            for (int i = 1; i < items.Count; i++) // Skip the first item to keep it in place
            {
                var item = items[i];
                string sourceFilePath = Path.GetDirectoryName(item.FilePath) ?? "";
                string duplicatesPath = Path.Combine(sourceFilePath, "Duplicates");

                // Ensure the 'Duplicates' directory exists
                if (!Directory.Exists(duplicatesPath))
                    Directory.CreateDirectory(duplicatesPath);

                string destinationFilePath = Path.Combine(duplicatesPath, Path.GetFileName(item.FilePath));

                // Move the file to the 'Duplicates' directory
                File.Move(item.FilePath, destinationFilePath, true); // Overwrite if the file already exists

                // Add the file to the movedFiles list
                movedFiles.Add(item);
            }
        });

        // Remove the moved files from the original fileList
        fileList.RemoveAll(file => movedFiles.Contains(file));

        // Return the count of files that were moved as duplicates
        return movedFiles.Count;
    }

    #region File Name Building

    /// <summary>
    /// Builds a new file name for the given XISF file based on its properties and the specified index.
    /// </summary>
    private string BuildFileName(int index, XisfFile file)
    {
        // Handle Master frames
        if (file.TargetName.Equals("Master"))
        {
            return BuildMasterFileName(file);
        }

        // Handle regular frames
        return file.FrameType switch
        {
            eFrame.DARK => BuildCalibrationFileName(index, file, "Dark"),
            eFrame.BIAS => BuildCalibrationFileName(index, file, "Bias"),
            eFrame.FLAT => BuildFlatFileName(index, file),
            eFrame.LIGHT => BuildLightFileName(index, file),
            _ => $"{index:D3} Rename Failed"
        };
    }

    private static string BuildMasterFileName(XisfFile file)
    {
        string frameInfo = FormatMasterFrameInfo(file);
        string cameraInfo = FormatCameraInfo(file);
        string captureDate = FormatCaptureDate(file);

        string newName = file.FrameType switch
        {
            eFrame.LIGHT => BuildMasterLightName(file, frameInfo, cameraInfo),
            eFrame.DARK  => $"Master Dark  {captureDate}  {frameInfo}  {cameraInfo}",
            eFrame.BIAS  => $"Master Bias  {captureDate}  {frameInfo}  {cameraInfo}",
            eFrame.FLAT  => $"Master Flat {file.FilterName}  {captureDate}  {frameInfo}  {cameraInfo}  {FormatTelescope(file)}",
            _ => "Master Unknown"
        };

        // Add algorithm and software suffix for non-LIGHT frames
        if (file.FrameType != eFrame.LIGHT)
        {
            newName += FormatAlgorithmSuffix(file);
        }

        return newName;
    }

    private static string BuildMasterLightName(XisfFile file, string frameInfo, string cameraInfo)
    {
        string name = $"Master Integration  {FormatFilterName(file)}  {frameInfo}  {cameraInfo}";
        name += !string.IsNullOrEmpty(file.MSTRALG)
            ? $"  ({file.MSTRALG}  {FormatCaptureDate(file)}"
            : $"  ({FormatCaptureDate(file)}";
        name += $"  {file.CaptureSoftware})";

        return name;
    }

    private static string BuildCalibrationFileName(int index, XisfFile file, string frameLabel)
    {
        string name = $"{index:D3}";
        name += $"  {frameLabel}";
        name += $"  {FormatCamera(file)}";
        name += $"  {FormatCaptureTime(file)}";

        return name;
    }

    private static string BuildFlatFileName(int index, XisfFile file)
    {
        string name = $"{index:D3}";
        name += $"  {FormatFlatFilter(file)}";
        name += $"  {FormatCamera(file)}";
        name += $"  {FormatTelescope(file)}";
        name += FormatAmbientTemperature(file);
        name += $"  {FormatFocuser(file)}";
        name += $"  {FormatCaptureTime(file)}";

        return name;
    }

    private string BuildLightFileName(int index, XisfFile file)
    {
        string name = $"{index:D3}";
        name += $"  {file.TargetName}";
        name += $"  {FormatFilterName(file)}";
        name += $"  {FormatCamera(file)}";
        name += $"  {FormatTelescope(file)}";
        name += FormatAmbientTemperature(file);
        name += $"  {FormatFocuser(file)}";
        string calibration = IncludeCalibrationFrames ? FormatCalibrationFrames(file) : "";
        name += $"  {FormatCaptureTime(file, calibration)}";

        return name;
    }

    #endregion

    #region Format Helpers

    private static string FormatFilterName(XisfFile file) =>
        $"L-{file.FilterName}";

    private static string FormatFlatFilter(XisfFile file) =>
        $"F-{file.FilterName}";

    private static string FormatCalibrationFrames(XisfFile file)
    {
        string result = "";
        if (!string.IsNullOrEmpty(file.CDARK))
            result += $"  {file.CDARK}";
        if (!string.IsNullOrEmpty(file.CFLAT))
            result += $"  {file.CFLAT}";
        return result;
    }

    private static string FormatMasterFrameInfo(XisfFile file) =>
        file.MSTRFRMS != -1
            ? $"{file.ExposureSeconds.FormatExposureTime()}x{file.Binning}x{file.MSTRFRMS}"
            : $"{file.ExposureSeconds.FormatExposureTime()}x{file.Binning}";

    private static string FormatCaptureDate(XisfFile file) =>
        $"{file.CaptureTime:yyyy-MM-dd}";

    private static string FormatCamera(XisfFile file) =>
        $"{file.ExposureSeconds.FormatExposureTime()}x{file.Binning}  {FormatCameraInfo(file)}";

    private static string FormatCameraInfo(XisfFile file) =>
        $"{file.Camera}G{file.Gain:D3}O{file.Offset}@{file.SensorTemperature.FormatTemperature()}C";

    private static string FormatTelescope(XisfFile file) =>
        $"{file.Telescope}@{file.FocalLength:F0}";

    private static string FormatAmbientTemperature(XisfFile file) =>
        file.AmbientTemperature != NoTemperature
            ? $"{file.AmbientTemperature.FormatTemperature()}C"
            : $"{file.FocuserTemperature.FormatTemperature()}C";

    private static string FormatFocuser(XisfFile file)
    {
        string result = $"F{file.FocuserPosition:D5}@{file.FocuserTemperature.FormatTemperature()}C";
        if (file.RotationAngle.StartsWith('S'))
            result += $"  {file.RotationAngle}";
        return result;
    }

    private static string FormatCaptureTime(XisfFile file, string extra = "") =>
        $"({file.CaptureTime:yyyy-MM-dd  hh-mm-ss tt}  {file.CaptureSoftware}{extra})";

    private static string FormatAlgorithmSuffix(XisfFile file)
    {
        if (!string.IsNullOrEmpty(file.MSTRALG))
        {
            return !string.IsNullOrEmpty(file.CaptureSoftware)
                ? $"  ({file.MSTRALG} {file.CaptureSoftware})"
                : $"  ({file.MSTRALG})";
        }

        return !string.IsNullOrEmpty(file.CaptureSoftware)
            ? $"  ({file.CaptureSoftware})"
            : "";
    }

    #endregion
}
