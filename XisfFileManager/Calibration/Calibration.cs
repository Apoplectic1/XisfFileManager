using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using XisfFileManager.Configuration;
using XisfFileManager.Globals;
using XisfFileManager.Files;
using static System.Net.WebRequestMethods;

namespace XisfFileManager
{
    // ******************************************************************************************************************
    // ******************************************************************************************************************
    public class Calibration
    {
        private List<(XisfFile TargetFile, XisfFile? CalibrationFile)> mDarkMatchedTargetPairList;
        private List<(XisfFile TargetFile, XisfFile? CalibrationFile)> mFlatMatchedTargetPairList;
        private List<XisfFile> mBiasCalibrationFileList;
        private List<XisfFile> mDarkCalibrationFileList;
        private List<XisfFile> mFlatCalibrationFileList;
        private List<XisfFile> mLibraryCalibrationFileList;
        private List<XisfFile> mUniqueDarkCalibrationFileList;
        private List<XisfFile> mUniqueFlatCalibrationFileList;
        private List<XisfFile> mUniqueBiasCalibrationFileList;
        private List<XisfFile> mUnmatchedBiasTargetFileList;
        private List<XisfFile> mUnmatchedDarkTargetFileList;
        private List<XisfFile> mUnmatchedFlatTargetFileList;
        private XisfFileUpdate mXisfFileUpdate;
        private readonly CalibrationTabPageValues mCalibrationTabValues;

        public eFilter Filter { get; set; } = eFilter.ALL;
        public eFrame Frame { get; set; } = eFrame.ALL;
        public bool Recurse { get; set; } = true;
        public double ExposureTolerance { get; set; }
        public double FocuserTolerance { get; set; }
        public double GainTolerance { get; set; }
        public double OffsetTolerance { get; set; }
        public double RotationTolerance { get; set; }
        public double TemperatureTolerance { get; set; }

        // ******************************************************************************************************************
        // ******************************************************************************************************************
        // Constructor
        // ******************************************************************************************************************
        public Calibration()
        {
            ExposureTolerance = 0;
            Filter = eFilter.ALL;
            Frame = eFrame.ALL;
            FocuserTolerance = 10000;
            GainTolerance = 0;
            OffsetTolerance = 0;
            RotationTolerance = 180.0;
            TemperatureTolerance = 5;
            mBiasCalibrationFileList = new List<XisfFile>();
            mCalibrationTabValues = new CalibrationTabPageValues();
            mDarkCalibrationFileList = new List<XisfFile>();
            mDarkMatchedTargetPairList = new List<(XisfFile, XisfFile?)>();
            mFlatCalibrationFileList = new List<XisfFile>();
            mFlatMatchedTargetPairList = new List<(XisfFile, XisfFile?)>();
            mLibraryCalibrationFileList = new List<XisfFile>();
            mUniqueDarkCalibrationFileList = new List<XisfFile>();
            mUniqueFlatCalibrationFileList = new List<XisfFile>();
            mUniqueBiasCalibrationFileList = new List<XisfFile>();
            mUnmatchedBiasTargetFileList = new List<XisfFile>();
            mUnmatchedDarkTargetFileList = new List<XisfFile>();
            mUnmatchedFlatTargetFileList = new List<XisfFile>();
            mXisfFileUpdate = new XisfFileUpdate();
        }

        // ******************************************************************************************************************
        // ******************************************************************************************************************
        public void ResetAll()
        {
            ExposureTolerance = 0;
            Filter = eFilter.ALL;
            Frame = eFrame.ALL;
            FocuserTolerance = 10000;
            GainTolerance = 0;
            OffsetTolerance = 0;
            RotationTolerance = 180.0;
            TemperatureTolerance = 5;
            mBiasCalibrationFileList.Clear();
            mDarkCalibrationFileList.Clear();
            mDarkMatchedTargetPairList.Clear();
            mFlatCalibrationFileList.Clear();
            mFlatMatchedTargetPairList.Clear();
            mLibraryCalibrationFileList.Clear();
            mUniqueDarkCalibrationFileList.Clear();
            mUniqueFlatCalibrationFileList.Clear();
            mUniqueBiasCalibrationFileList.Clear();
            mUnmatchedBiasTargetFileList.Clear();
            mUnmatchedDarkTargetFileList.Clear();
            mUnmatchedFlatTargetFileList.Clear();
        }

        // ******************************************************************************************************************
        // ******************************************************************************************************************

        public async Task ReadCalibrationFramesAsync(string sCalibrationFrameDirectory)
        {
            // Recursively search sCalibrationFrameDirectory to find calibration frames
            // Add the frames to either mLocalCalibrationFileList or mLibraryCalibrationFileList

            XisfXmlReader xmlReader = new XisfXmlReader();

            mCalibrationTabValues.MessageMode = eMessageMode.CLEAR;
            mCalibrationTabValues.Progress = 0;
            CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);

            mLibraryCalibrationFileList.Clear();

            DirectoryInfo diDirectoryTree = new DirectoryInfo(sCalibrationFrameDirectory);

            List<string> mXisfExclude = DirectoryFilters.CalibrationExcludes;

            bool bStatus = Files.DirectoryOperations.FindCalibrationFiles(sCalibrationFrameDirectory, mXisfExclude, true);

            if (Files.DirectoryOperations.FileInfoList.Count == 0)
            {
                mCalibrationTabValues.MatchCalibrationMessage = "\r\n\r\n\r\n\r\n            No Calibration Files Found under " + sCalibrationFrameDirectory + "\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                return;
            }

            mCalibrationTabValues.TotalFiles = Files.DirectoryOperations.FileInfoList.Count;

            foreach (FileInfo file in Files.DirectoryOperations.FileInfoList)
            {
                // Create a new xisf file instance for reads
                XisfFile calibrationFile = new XisfFile
                {
                    FilePath = file.FullName
                };

                mCalibrationTabValues.ProgressMax = Files.DirectoryOperations.FileInfoList.Count;
                mCalibrationTabValues.Progress += 1;
                mCalibrationTabValues.FileName = Path.GetDirectoryName(calibrationFile.FilePath) + "\n" + Path.GetFileName(calibrationFile.FilePath);
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);

                await xmlReader.ReadXisfFileHeaderKeywords(calibrationFile);

                mLibraryCalibrationFileList.Add(calibrationFile);
            }

            // There should not be any duplicate calibration files in the library but just in case...
            mLibraryCalibrationFileList = mLibraryCalibrationFileList.Distinct().ToList();

            mCalibrationTabValues.TotalFiles = mLibraryCalibrationFileList.Count;
            CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
        }

        // ******************************************************************************************************************
        // ******************************************************************************************************************

        public bool MatchTargetsWithCalibrationLibraryFrames(List<XisfFile> targetFileList)
        {
            mUnmatchedDarkTargetFileList.Clear();
            mUnmatchedFlatTargetFileList.Clear();
            mUnmatchedBiasTargetFileList.Clear();

            mUnmatchedDarkTargetFileList.AddRange(targetFileList);
            mUnmatchedFlatTargetFileList.AddRange(targetFileList);
            mUnmatchedBiasTargetFileList.AddRange(targetFileList);

            // ----------------------------------------------------------------------------------------------------------------------------------------------------

            // At this point, we have lists of target frames (lights, darks and flats) as well as a list of all Library calibration frames (darks, flats and bias) 
            // We match each target frame with a corresponding calibration frame keeping track of what calibration frame matched with what target frame:
            //    1. Find all calibration frames for the target set (Dark, Flat, Bias)
            //    2. Uniquify and consecutively number the found calibration frames 
            //    3. Assign the appropriate calibration frame number to each target

            // ----------------------------------------------------------------------------------------------------------------------------------------------------

            // DARKS

            // Match each target light frame with a corresponding DARK frame and place the match in mUniqueDarkCalibrationFileList<TargetFile, CalibrationFile>
            // Each item: mUniqueDarkCalibrationFileList.Calibration will be null if no DARK matches mUniqueDarkCalibrationFileList.Target
            // 
            // This list will eventually be copied out and placed in the local target Calibration directory
            // Targets that don't match a DARK calibration file will be flagged later

            // Create a list of all DARK calibration files that match the target frames in mDarkMatchedTargetPairList -> .Target matched with .Calibration
            bool bAllDarksMatched = MatchTargetsWithDarkCalibrationFrames(mUnmatchedDarkTargetFileList);

            // Each calibration file can match multiple targets. We only want to keep the unique calibration files
            mUniqueDarkCalibrationFileList = mDarkMatchedTargetPairList.Select(item => item.CalibrationFile).OfType<XisfFile>().Distinct().ToList();

            // Keep the calibration files in a consistent order for CDARK numbering repeatability run-to-run 
            mUniqueDarkCalibrationFileList.Sort((a, b) => a.CaptureTime.CompareTo(b.CaptureTime)); // oldest is first

            // ----------------------------------------------------------------------------------------------------------------------------------------------------

            int darkIndex = 1;
            foreach (XisfFile uniqueDarkCalibrationFile in mUniqueDarkCalibrationFileList)
            {
                if (uniqueDarkCalibrationFile == null) continue;

                foreach ((XisfFile TargetFile, XisfFile? CalibrationFile) darkMatchedTargetPair in mDarkMatchedTargetPairList)
                {
                    // For each matched dark Target/Calibration pair: When a Target .Calibration file matches  uniqueDarkCalibrationFile,
                    // assign this Target/Calibration pair and uniqueDarkCalibrationFile a unique DARK "D" number
                    if (darkMatchedTargetPair.CalibrationFile?.FilePath == uniqueDarkCalibrationFile.FilePath)
                    {
                        darkMatchedTargetPair.TargetFile.CDARK = "D" + darkIndex.ToString();

                        var target = darkMatchedTargetPair.TargetFile;
                        var dark = darkMatchedTargetPair.CalibrationFile;

                        uniqueDarkCalibrationFile.CDARK = "D" + darkIndex.ToString();
                        uniqueDarkCalibrationFile.RemoveKeyword("CFLAT"); // For WBPP
                    }
                }

                darkIndex++;
            }

            // ----------------------------------------------------------------------------------------------------------------------------------------------------

            // FLATS

            // Match each target light frame with a corresponding FLAT frame and place the match in mUniqueFlatCalibrationFileList<TargetFile, CalibrationFile>
            // Each item: mUniqueFlatCalibrationFileList.Calibration will be null if no FLAT matches mUniqueFlatCalibrationFileList.Target
            // 
            // This list will eventually be copied out and placed in the local target Calibration directory
            // Targets that don't match a FLAT calibration file will be flagged later

            // Create a list of all FLAT calibration files that match the target frames in mFlatMatchedTargetPairList --> .Target matched with .Calibration
            bool bAllFlatsMatched = MatchTargetsWithFlatCalibrationFrames(mUnmatchedFlatTargetFileList);

            // Each calibration file can match multiple targets. We only want to keep the unique calibration files
            mUniqueFlatCalibrationFileList = mFlatMatchedTargetPairList.Select(item => item.CalibrationFile).OfType<XisfFile>().Distinct().ToList();

            // Keep the calibration files in a consistent order for CFLAT numbering repeatability run-to-run 
            mUniqueFlatCalibrationFileList.Sort((a, b) => a.CaptureTime.CompareTo(b.CaptureTime)); // oldest is first

            // ----------------------------------------------------------------------------------------------------------------------------------------------------

            int flatIndex = 1;
            foreach (XisfFile uniqueFlatCalibrationFile in mUniqueFlatCalibrationFileList)
            {
                if (uniqueFlatCalibrationFile == null) continue;

                foreach ((XisfFile TargetFile, XisfFile? CalibrationFile) flatMatchedTargetPair in mFlatMatchedTargetPairList)
                {
                    // For each matched flat Target/Calibration pair: When a Target .Calibration file matches  uniqueFlatCalibrationFile,
                    // assign this Target/Calibration pair and uniqueFlatCalibrationFile a unique FLAT "F" number
                    if (flatMatchedTargetPair.CalibrationFile?.FilePath == uniqueFlatCalibrationFile.FilePath)
                    {
                        flatMatchedTargetPair.TargetFile.CFLAT = "F" + flatIndex.ToString();
                        uniqueFlatCalibrationFile.CFLAT = "F" + flatIndex.ToString();
                        uniqueFlatCalibrationFile.RemoveKeyword("CDARK"); // For WBPP
                    }
                }

                flatIndex++;
            }

            // ----------------------------------------------------------------------------------------------------------------------------------------------------

            // Send data to main UI for display

            mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
            mCalibrationTabValues.TotalUniqueFlatCalibrationFiles = mUniqueFlatCalibrationFileList.Count;
            mCalibrationTabValues.TotalUniqueDarkCalibrationFiles = mUniqueDarkCalibrationFileList.Count;
            mCalibrationTabValues.TotalUniqueBiasCalibrationFiles = 0;
            mCalibrationTabValues.TotalMatchedCalibrationFiles = mDarkCalibrationFileList.Count + mFlatCalibrationFileList.Count + mBiasCalibrationFileList.Count;

            if (bAllDarksMatched && bAllFlatsMatched)
                mCalibrationTabValues.MatchCalibrationMessage = "\r\n\r\n\r\n\r\n            Matched All Target Frames\r\n\r\n\r\n";

            CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);

            return true;
        }

        // ******************************************************************************************************************
        // ******************************************************************************************************************

        private bool MatchTargetsWithDarkCalibrationFrames(List<XisfFile> targetFrameList)
        {
            bool bAllDarksMatched = true;

            if (targetFrameList.Count == 0)
            {
                mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                mCalibrationTabValues.MatchCalibrationMessage = "\r\n\r\n\r\n            MatchCalibrationDarkFrames: No Target Frames Found\r\n\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                return false;
            }

            mDarkMatchedTargetPairList.Clear();

            // Now we add to mDarkMatchedTargetPairList for all the Dark calibration files that match the target - null if no match
            foreach (XisfFile targetFile in mUnmatchedDarkTargetFileList)
            {
                // For each target file, find the nearest matching Dark calibration file

                mDarkMatchedTargetPairList.Add((targetFile, FindNearestCalibrationFile(eFrame.DARK, targetFile, mLibraryCalibrationFileList)));

                // Check the .Calibration dictionary item that was just added for null
                if (mDarkMatchedTargetPairList.Last().CalibrationFile == null)
                    bAllDarksMatched = false;
            }

            return bAllDarksMatched;
        }

        // ******************************************************************************************************************
        // ******************************************************************************************************************

        private bool MatchTargetsWithFlatCalibrationFrames(List<XisfFile> targetFrameList)
        {
            bool bAllFlatsMatched = true;

            if (targetFrameList.Count == 0)
            {
                mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                mCalibrationTabValues.MatchCalibrationMessage = "\r\n\r\n\r\n\r\n            MatchCalibrationFlatFrames: No Target Frames Found\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                return false;
            }

            mFlatMatchedTargetPairList.Clear();

            // Now we add to mFlatMatchedTargetPairList for all the Dark calibration files that match the target - null if no match
            foreach (XisfFile targetFile in mUnmatchedFlatTargetFileList)
            {
                mFlatMatchedTargetPairList.Add((targetFile, FindNearestCalibrationFile(eFrame.FLAT, targetFile, mLibraryCalibrationFileList)));

                // Check the .Calibration dictionary item that was just added for null
                if (mFlatMatchedTargetPairList.Last().CalibrationFile == null)
                    bAllFlatsMatched = false;
            }

            return bAllFlatsMatched;
        }

        // ******************************************************************************************************************
        // ******************************************************************************************************************

        private XisfFile? FindNearestCalibrationFile(eFrame calibrationFrameMatchType, XisfFile targetFile, List<XisfFile> CalibrationLibraryFileList)
        {
            // This routine gets called twice for each target frame
            // The first time the this is used to build non-unique (meaning to find all) lists of target matching calibration files
            // The second time this is used to match unique calibration files with each target file

            // More specifically, the first time this is called, calibrationFileList contains all calibration type files found in the calibration library
            // The second time, calibrationFileList has been reduced to include only calibration files unique to the target List 

            // To cleanly reuse this code, we have to use the margin calculations when differentiating Flats from Darks from Biases. Unfortunate but not really a performance issue...

            // To Do
            // Add code to inform user of how close the match was for each appropriate match parameter (the ones with margins)

            bool bIgnoreFilter = calibrationFrameMatchType == eFrame.DARK;
            bool bIgnoreExposure = calibrationFrameMatchType is eFrame.FLAT or eFrame.BIAS;
            bool bIgnoreRotator = calibrationFrameMatchType is eFrame.DARK or eFrame.BIAS;
            bool bIgnoreFocuser = calibrationFrameMatchType is eFrame.DARK or eFrame.BIAS;

            // ******************************************************************************************************************

            // Return a list of all calibration files that match the target file camera
            List<XisfFile> CameraList = CalibrationLibraryFileList.Where(camera => camera.Camera == targetFile.Camera).ToList();
            if (CameraList.Count == 0)
            {
                mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Target Camera " + targetFile.Camera + "\r\n"
                                                              + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                return null;
            }

            // ******************************************************************************************************************

            // Refine CameraList to match the passed FrameType (DARK, FLAT, etc.)
            List<XisfFile> FrameTypeList = CameraList.Where(frameType => frameType.FrameType == calibrationFrameMatchType).ToList();
            if (FrameTypeList.Count == 0)
            {
                mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Target Frame Type " + Enum.GetName<eFrame>(calibrationFrameMatchType) + "\r\n"
                                                              + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                return null;
            }

            // ******************************************************************************************************************

            // Refine FrameTypeList to match the target file binning
            List<XisfFile> BinningList = FrameTypeList.Where(binning => binning.Binning == targetFile.Binning).ToList();
            if (BinningList.Count == 0)
            {
                mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Target Binning " + targetFile.Binning.ToString() + "x" + targetFile.Binning.ToString() + "\r\n"
                                                              + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                return null;
            }

            // ******************************************************************************************************************

            List<XisfFile> WidthList = BinningList.Where(width => width.TargetAttachmentWidth == targetFile.TargetAttachmentWidth).ToList();
            if (WidthList.Count == 0)
            {
                mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Target Width " + targetFile.TargetAttachmentWidth.ToString()
                                                                + "x" + targetFile.TargetAttachmentWidth.ToString() + "\r\n" + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                return null;
            }

            // ******************************************************************************************************************

            List<XisfFile> HeightList = WidthList.Where(height => height.TargetAttachmentHeight == targetFile.TargetAttachmentHeight).ToList();
            if (HeightList.Count == 0)
            {
                mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Target Height " + targetFile.TargetAttachmentHeight.ToString()
                                                                + "x" + targetFile.TargetAttachmentHeight.ToString() + "\r\n" + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                return null;
            }

            // ******************************************************************************************************************

            // Refine HeightList to match the target file filter
            // We ignore filters for DARKs and BIASs
            List<XisfFile> FilterList = bIgnoreFilter ? HeightList : HeightList.Where(filter => filter.FilterName == targetFile.FilterName).ToList();
            if (FilterList.Count == 0)
            {
                mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Target Filter + " + targetFile.FilterName + "\r\n"
                                                              + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                return null;
            }

            // ******************************************************************************************************************

            // Refine FilterList to match the target file gain
            List<XisfFile> GainList = FilterList.Where(gain => Math.Abs(gain.Gain - targetFile.Gain) <= GainTolerance).ToList();
            if (GainList.Count == 0)
            {
                // The tolerance match failed. Find the calibrartion file with the nearest gain to the target file
                double smallestDifference = FilterList.Min(gain => Math.Abs(gain.Gain - targetFile.Gain));

                GainList = FilterList.Where(offset => Math.Abs(offset.Offset - targetFile.Offset) <= smallestDifference).ToList();

                if (GainList.Count == 0)
                {
                    mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                    mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Target Gain " + targetFile.Gain + " Tolerance: " + GainTolerance + "\r\n"
                                                                  + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                    CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                    return null;
                }
                else
                {
                    mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                    mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Matched with Gain Offset Tolerance: " + smallestDifference + "\r\n"
                                                                  + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                    CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                }
            }

            // ******************************************************************************************************************

            // Refine GainList to match the target file offset
            List<XisfFile> OffsetList = GainList.Where(offset => Math.Abs(offset.Offset - targetFile.Offset) <= OffsetTolerance).ToList();
            if (OffsetList.Count == 0)
            {
                // The tolerance match failed. Find the calibrartion file with the nearest offset to the target file
                double smallestDifference = GainList.Min(offset => Math.Abs(offset.Offset - targetFile.Offset));

                OffsetList = GainList.Where(offset => Math.Abs(offset.Offset - targetFile.Offset) <= smallestDifference).ToList();

                if (OffsetList.Count == 0)
                {
                    mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                    mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Target Offset " + targetFile.Offset + " Tolerance: " + smallestDifference + "\r\n"
                                                                  + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                    CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                    return null;
                }
                else
                {
                    mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                    mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Matched with Target Offset Tolerance: " + smallestDifference + "\r\n"
                                                                  + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                    CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                }
            }

            // ******************************************************************************************************************

            // Refine OffsetList to match the target file focuser position
            // We ignore focuser position for DARKs and BIASs
            List<XisfFile> FocuserList = bIgnoreFocuser ? OffsetList : OffsetList.Where(focus => Math.Abs(focus.FocuserPosition - targetFile.FocuserPosition) <= FocuserTolerance).ToList();
            if (FocuserList.Count == 0) FocuserList.AddRange(OffsetList); // Deal with old Masters that don't incude the Focus position
            if (FocuserList.Count == 0)
            {
                mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Target Focuser Position " + targetFile.FocuserPosition + " Tolerance: " + FocuserTolerance + "\r\n"
                                                               + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                return null;
            }

            // ******************************************************************************************************************

            // Refine FocuserList to match the target file rotator position
            // We ignore rotator position for DARKs and BIASs
            List<XisfFile> RotatorList = bIgnoreRotator ? FocuserList : FocuserList.Where(rotator => Math.Abs(rotator.RotatorPosition - targetFile.RotatorPosition) <= RotationTolerance).ToList();
            if (RotatorList.Count == 0) RotatorList.AddRange(FocuserList); // Deal with old Masters that dont include the Rotator position
            if (RotatorList.Count == 0)
            {
                mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Target Rotator Mechanical Position " + targetFile.RotatorPosition + " Tolerance: " + RotationTolerance + "\r\n"
                                                               + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                return null;
            }

            // ******************************************************************************************************************

            // Refine RotatorList to match the target file Sensor Temperature
            List<XisfFile> TemperatureList = RotatorList.Where(temperature => Math.Abs(temperature.SensorTemperature - targetFile.SensorTemperature) <= TemperatureTolerance).ToList();
            if (TemperatureList.Count == 0)
            {
                // The tolerance match failed. Find the calibrartion file with the nearest temperature to the target file 
                double smallestDifference = RotatorList.Min(temperature => Math.Abs(temperature.SensorTemperature - targetFile.SensorTemperature));

                TemperatureList = RotatorList.Where(temperature => Math.Abs(temperature.SensorTemperature - targetFile.SensorTemperature) <= smallestDifference).ToList();

                if (TemperatureList.Count == 0)
                {
                    mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                    mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Target Sensor Temperature " + targetFile.SensorTemperature + " Tolerance: " + TemperatureTolerance + "\r\n"
                                                                   + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                    CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                    return null;
                }
                else
                {
                    mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                    mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Matched with Target Sensor Temperature Tolerance: " + smallestDifference + "\r\n"
                                                                   + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                    CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                }
            }

            // ******************************************************************************************************************

            // Refine TemperatureList to match the target file exposure time
            List<XisfFile> ExposureList = bIgnoreExposure ? TemperatureList : TemperatureList.Where(exposure => Math.Abs(exposure.ExposureSeconds - targetFile.ExposureSeconds) <= ExposureTolerance).ToList();
            if (ExposureList.Count == 0)
            {
                // The tolerance match failed. Find the calibrartion file with the nearest exposure time to the target file
                double smallestDifference = TemperatureList.Min(seconds => Math.Abs(seconds.ExposureSeconds - targetFile.ExposureSeconds));

                ExposureList = TemperatureList.Where(exposure => Math.Abs(exposure.ExposureSeconds - targetFile.ExposureSeconds) <= smallestDifference).ToList();

                if (ExposureList.Count == 0)
                {
                    mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                    mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: Exposure " + targetFile.ExposureSeconds + " Tolerance: " + smallestDifference + "\r\n"
                                                                  + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                    CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                    return null;
                }
                else
                {
                    mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                    mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Matched with Exposure Tolerance: " + smallestDifference + "\r\n"
                                                                  + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                    CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
                }
            }

            // ******************************************************************************************************************
            XisfFile? nearestFile = ExposureList.OrderBy(nearest => Math.Abs((nearest.CaptureTime - targetFile.CaptureTime).TotalSeconds)).FirstOrDefault();
            Debug.WriteLine("\nFindNearestCalibrationFile: " + calibrationFrameMatchType + "\n" + Path.GetFileName(targetFile.FilePath));
            Debug.WriteLine(nearestFile != null ? Path.GetFileName(nearestFile.FilePath) : "No files found in ExposureList.");


            if (nearestFile == null)
            {
                mCalibrationTabValues.MessageMode = eMessageMode.APPEND;
                mCalibrationTabValues.MatchCalibrationMessage = calibrationFrameMatchType + " Match Failed: No Calibration File Found\r\n"
                                                              + "  " + Path.GetFileName(targetFile.FilePath) + "\r\n\r\n";
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);
            }

            // Return the single nearest capture time calibration file
            return ExposureList.OrderBy(nearest => Math.Abs((nearest.CaptureTime - targetFile.CaptureTime).TotalSeconds)).FirstOrDefault();
        }

        // ******************************************************************************************************************
        // ******************************************************************************************************************

        public async Task<bool> CreateTargetCalibrationDirectoryAsync(List<XisfFile> targetFileList)
        {
            string targetCalibrationDirectory = SetTargetCalibrationFileDirectories(targetFileList[0].FilePath);

            mCalibrationTabValues.Progress = 0;
            mCalibrationTabValues.ProgressMax = mUniqueDarkCalibrationFileList.Count + mUniqueFlatCalibrationFileList.Count + mBiasCalibrationFileList.Count;

            foreach (XisfFile uniqueDarkCalibrationFile in mUniqueDarkCalibrationFileList)
            {
                // Send a message to the UI
                mCalibrationTabValues.Progress += 1;
                mCalibrationTabValues.FileName = targetCalibrationDirectory + "\n" + Path.GetFileName(uniqueDarkCalibrationFile.FilePath);
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);

                // Construct destination filePath
                string destinationCalibrationFilePath = targetCalibrationDirectory + @"\" + uniqueDarkCalibrationFile.CDARK + " " + Path.GetFileName(uniqueDarkCalibrationFile.FilePath);

                // Save the current KeywordUpdateMode and set it to FORCE
                eKeywordUpdateMode keywordUpdateMode = uniqueDarkCalibrationFile.KeywordUpdateMode;
                uniqueDarkCalibrationFile.KeywordUpdateMode = eKeywordUpdateMode.FORCE;

                // Update the copied calibration file with uniqueDarkCalibrationFile Keywords
                await mXisfFileUpdate.UpdateFileAsync(uniqueDarkCalibrationFile, destinationCalibrationFilePath);

                // Restore the original KeywordUpdateMode
                uniqueDarkCalibrationFile.KeywordUpdateMode = keywordUpdateMode;
            }

            foreach (XisfFile uniqueFlatCalibrationFile in mUniqueFlatCalibrationFileList)
            {
                // Send a message to the UI
                mCalibrationTabValues.Progress += 1;
                mCalibrationTabValues.FileName = targetCalibrationDirectory + "\n" + Path.GetFileName(uniqueFlatCalibrationFile.FilePath);
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);

                // Construct destination filePath
                string destinationCalibrationFilePath = targetCalibrationDirectory + @"\" + uniqueFlatCalibrationFile.CFLAT + " " + Path.GetFileName(uniqueFlatCalibrationFile.FilePath);

                // Save the current KeywordUpdateMode and set it to FORCE
                eKeywordUpdateMode keywordUpdateMode = uniqueFlatCalibrationFile.KeywordUpdateMode;
                uniqueFlatCalibrationFile.KeywordUpdateMode = eKeywordUpdateMode.FORCE;

                // Update the copied calibration file with uniqueFlatCalibrationFile Keywords
                await mXisfFileUpdate.UpdateFileAsync(uniqueFlatCalibrationFile, destinationCalibrationFilePath);

                // Restore the original KeywordUpdateMode
                uniqueFlatCalibrationFile.KeywordUpdateMode = keywordUpdateMode;
            }

            foreach (XisfFile uniqueBiasCalibrationFile in mUniqueBiasCalibrationFileList)
            {
                // Send a message to the UI
                mCalibrationTabValues.Progress += 1;
                mCalibrationTabValues.FileName = targetCalibrationDirectory + "\n" + Path.GetFileName(uniqueBiasCalibrationFile.FilePath);
                CalibrationTabPageEvent.TransmitData(mCalibrationTabValues);

                // Construct destination filePath
                string destinationCalibrationFilePath = targetCalibrationDirectory + @"\" + Path.GetFileName(uniqueBiasCalibrationFile.FilePath);

                // Save the current KeywordUpdateMode and set it to FORCE
                eKeywordUpdateMode keywordUpdateMode = uniqueBiasCalibrationFile.KeywordUpdateMode;
                uniqueBiasCalibrationFile.KeywordUpdateMode = eKeywordUpdateMode.FORCE;

                // Update the copied calibration file with uniqueBiasCalibrationFile Keywords
                await mXisfFileUpdate.UpdateFileAsync(uniqueBiasCalibrationFile, destinationCalibrationFilePath);

                // Restore the original KeywordUpdateMode
                uniqueBiasCalibrationFile.KeywordUpdateMode = keywordUpdateMode;
            }

            return true;
        }

        // ******************************************************************************************************************
        // ******************************************************************************************************************

        public static string SetTargetCalibrationFileDirectories(string targetFilePath)
        {
            // Create and use a Calibration Directory for the complete project
            // For Calibration files, there is no distiction between an individual Target and a Mosaic
            string targetCalibrationDirectory = Path.GetDirectoryName(targetFilePath) ?? string.Empty;

            // Can we find a "Captures" directory?
            if (targetCalibrationDirectory.Contains(@"Captures\"))
                // Place the Calibration directory under "Captures"
                targetCalibrationDirectory = string.Concat(targetCalibrationDirectory.AsSpan(0, targetCalibrationDirectory.IndexOf("Captures")), @"Captures\Calibration");
            else
                // No - so set just add "Calibration" under the target path
                targetCalibrationDirectory = Path.GetFullPath(Path.Combine(targetCalibrationDirectory, @"..") + @"\Calibration");

            return targetCalibrationDirectory;
        }

        // ******************************************************************************************************************
        // ******************************************************************************************************************

        public int FindBackgroundPedestal(List<XisfFile> targetFileList, decimal minBackground)
        {
            int pedestal = 0;

            foreach ((XisfFile TargetFile, XisfFile? CalibrationFile) darkMatchedTargetPair in mDarkMatchedTargetPairList)
            {
                var target = darkMatchedTargetPair.TargetFile;
                var dark = darkMatchedTargetPair.CalibrationFile;

            }


            return pedestal;
        }

        // ******************************************************************************************************************
        // ******************************************************************************************************************

    }
}