using System.Text.RegularExpressions;
using XisfFileManager.Globals;


namespace XisfFileManager
{
    public class KeywordList
    {
        private Regex onlyNumerics = new Regex(@"^[\+\-]?\d*\.?[Ee]?[\+\-]?\d*$", RegexOptions.Compiled);
        public List<Keyword> mKeywordList { get; set; }

        // ----------------------------------------------------------------------------------------------------------

        public KeywordList()
        {
            mKeywordList = new List<Keyword>();
        }

        // ----------------------------------------------------------------------------------------------------------

        public void Clear()
        {
            mKeywordList.Clear();
        }

        // ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Normalizes capture time by converting DATE-OBS (UTC) to DATE-LOC (local time).
        /// Called during file update, not during property read.
        /// </summary>
        public void NormalizeCaptureTime()
        {
            // If DATE-LOC already exists, nothing to do
            if (GetKeywordValue("DATE-LOC") != string.Empty)
                return;

            string value = GetKeywordValue("DATE-OBS");
            if (value == string.Empty)
                return;

            try
            {
                // Convert UTC to Local Time
                DateTime dateTimeUtc = DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                dateTimeUtc = DateTime.SpecifyKind(dateTimeUtc, DateTimeKind.Utc);
                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                DateTime dateTimeLocal = TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, localTimeZone);

                AddKeyword("DATE-LOC", dateTimeLocal.ToString("yyyy-MM-ddTHH:mm:ss.fff"), "Local capture time");
            }
            catch
            {
                // If parsing fails, don't add the keyword
            }
        }

        // ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Normalizes exposure keyword by converting the legacy EXPOSURE to standard EXPTIME.
        /// Called during file update, not during property read.
        /// </summary>
        public void NormalizeExposure()
        {
            // If the standard EXPTIME already exists, just clean up the legacy EXPOSURE if present
            string exptimeValue = GetKeywordValue("EXPTIME");
            if (exptimeValue != string.Empty)
            {
                RemoveKeyword("EXPOSURE");
                return;
            }

            // Convert legacy EXPOSURE to standard EXPTIME
            string exposureValue = GetKeywordValue("EXPOSURE");
            if (exposureValue != string.Empty && double.TryParse(exposureValue, out double exposure))
            {
                // Add the standard EXPTIME keyword
                AddKeyword("EXPTIME", exposure.ToString(), "[s] Imaging Camera Exposure Time");

                // Purge the non-standard keyword
                RemoveKeyword("EXPOSURE");
            }
        }

        // ----------------------------------------------------------------------------------------------------------

        public static Keyword NewKeyword(string sName, string oValue, string sComment)
        {
            Keyword newKeyword = new Keyword
            {
                Name = sName,
                Value = oValue,
                Comment = sComment
            };

            return newKeyword;
        }

        // ----------------------------------------------------------------------------------------------------------

        public Keyword? GetKeyword(string sName)
        {
            Keyword? node = mKeywordList.Find(i => i.Name == sName);
            if (node == null)
                return null;

            return node;
        }

        // ----------------------------------------------------------------------------------------------------------

        public string GetKeywordValue(string sName)
        {
            Keyword? node = mKeywordList.Find(i => i.Name == sName);
            if (node == null)
                return string.Empty;

            return node.Value;
        }

        // ----------------------------------------------------------------------------------------------------------

        public string GetKeywordComment(string sName)
        {
            Keyword? node = mKeywordList.Find(i => i.Name == sName);
            if (node == null)
                return string.Empty;

            return node.Comment;
        }

        // ----------------------------------------------------------------------------------------------------------

        public void RemoveKeyword(string sName)
        {
            mKeywordList.RemoveAll(i => i.Name.Equals(sName));
        }

        // ----------------------------------------------------------------------------------------------------------

        public void RemoveKeyword(string sName, object oValue)
        {
            mKeywordList.RemoveAll(i => i.Name.Equals(sName) && i.Value.Equals(oValue));
        }

        // ----------------------------------------------------------------------------------------------------------

        public void AddKeyword(string sName, string oValue, string sComment = "XISF File Manager")
        {
            mKeywordList.RemoveAll(i => i.Name.Equals(sName));

            Keyword newKeyword = NewKeyword(sName, oValue, sComment);

            mKeywordList.Add(newKeyword);
        }

        // ----------------------------------------------------------------------------------------------------------

        public void AddKeywordKeepDuplicates(Keyword newKeyword)
        {
            mKeywordList.Add(newKeyword);
        }

        // ----------------------------------------------------------------------------------------------------------

        public void AddKeywordKeepDuplicates(string sName, string oValue, string sComment = "XISF File Manager")
        {
            Keyword newKeyword = NewKeyword(sName, oValue, sComment);

            mKeywordList.Add(newKeyword);
        }

        // *********************************************************************************************************

        public double Airmass
        {
            get
            {
                string value = GetKeywordValue("AIRMASS");
                if (value == string.Empty)
                    return -1.0;

                return Convert.ToDouble(value);
            }
            set { AddKeyword("AIRMASS", value.ToString("F3"), "[#] Line-of-sight Atmospheres"); }
        }

        // *********************************************************************************************************

        public double AmbientTemperature
        {
            get
            {
                string value = GetKeywordValue("AMBTEMP");
                if (value == string.Empty)
                {
                    value = GetKeywordValue("AMB-TEMP");
                    if (value == string.Empty)
                        return -273.0;
                }

                return Convert.ToDouble(value);
            }
            set { AddKeyword("AMBTEMP", value.ToString("F1"), "[degC] Local Temerature from Open Weather API"); }
        }

        // *********************************************************************************************************

        public double ApertureDiameter
        {
            get
            {
                string value = GetKeywordValue("APTDIA");
                if (value == string.Empty)
                    return -1.0;

                double diameter = Convert.ToDouble(value);
                return diameter;
            }
            set { AddKeyword("APTDIA", value.ToString("F1"), "[mm] OTA Aperture Diameter"); }
        }
        
        // *********************************************************************************************************

        public int Binning
        {
            get
            {
                string value = GetKeywordValue("XBINNING");
                if (value == string.Empty)
                    return -1;

                return Convert.ToInt32(value);
            }
            set
            {
                AddKeyword("XBINNING", value.ToString(), "[#] Camera Binning");
                AddKeyword("YBINNING", value.ToString(), "[#] Camera Binning");
            }
        }

        // *********************************************************************************************************

        public string Camera
        {
            get { return GetKeywordValue("INSTRUME"); }
            set { AddKeyword("INSTRUME", value, "[name] Imaging Camera"); }
        }

        // *********************************************************************************************************

        public DateTime CaptureTime
        {
            get
            {
                // Try DATE-LOC first (already in local time)
                string value = GetKeywordValue("DATE-LOC");
                if (value != string.Empty)
                {
                    if (DateTime.TryParse(value, out DateTime localTime))
                        return localTime;
                }

                // Fall back to DATE-OBS (UTC) and convert to local for display (read-only, no keyword modification)
                value = GetKeywordValue("DATE-OBS");
                if (value != string.Empty)
                {
                    try
                    {
                        DateTime dateTimeUtc = DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                        dateTimeUtc = DateTime.SpecifyKind(dateTimeUtc, DateTimeKind.Utc);
                        return TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, TimeZoneInfo.Local);
                    }
                    catch
                    {
                        return DateTime.MinValue;
                    }
                }

                return DateTime.MinValue;
            }
            set { AddKeyword("DATE-LOC", value.ToString("yyyy-MM-ddTHH:mm:ss.fff"), "Local capture time"); }
        }

        // *********************************************************************************************************

        public string CBIAS
        {
            get { return GetKeywordValue("CBIAS"); }
            set { AddKeyword("CBIAS", value.ToString(), "[#] PixInsight WBPP PreProcessing Group Keyword"); }
        }

        // *********************************************************************************************************

        public string CDARK
        {
            get { return GetKeywordValue("CDARK"); }
            set { AddKeyword("CDARK", value.ToString(), "[#] PixInsight WBPP PreProcessing Group Keyword"); }
        }

        // *********************************************************************************************************

        public string CFLAT
        {
            get { return GetKeywordValue("CFLAT"); }
            set { AddKeyword("CFLAT", value.ToString(), "[#] PixInsight WBPP PreProcessing Group Keyword"); }
        }

        // *********************************************************************************************************

        public string CPANEL
        {
            get { return GetKeywordValue("CPANEL"); }
            set { AddKeyword("CPANEL", value.ToString(), "[name] PixInsight WBPP PostProcessing Group Keyword"); }
        }

        // *********************************************************************************************************

        public string CSTARS
        {
            get { return GetKeywordValue("CSTARS"); }
            set { AddKeyword("CSTARS", value.ToString(), "[name] PixInsight WBPP PostProcessing Group Keyword"); }
        }

        // *********************************************************************************************************

        public string CREJECT
        {
            get { return GetKeywordValue("CREJECT"); }
            set { AddKeyword("CREJECT", value.ToString(), "[name] PixInsight WBPP PostProcessing Group Keyword"); }
        }

        // *********************************************************************************************************

        /// <summary>
        /// True when the file carries an exposure keyword at all (standard EXPTIME or legacy
        /// EXPOSURE). Needed because the ExposureSeconds getter returns 0.0 for a missing keyword,
        /// which is indistinguishable from a genuine 0-second bias frame.
        /// </summary>
        public bool HasExposure =>
            GetKeywordValue("EXPTIME") != string.Empty || GetKeywordValue("EXPOSURE") != string.Empty;

        public double ExposureSeconds
        {
            get
            {
                // Standard EXPTIME first (read-only, no keyword modification)
                string value = GetKeywordValue("EXPTIME");
                if (value != string.Empty && double.TryParse(value, out double exposure))
                    return exposure;

                // Fall back to legacy EXPOSURE (files not yet normalized)
                value = GetKeywordValue("EXPOSURE");
                if (value != string.Empty && double.TryParse(value, out exposure))
                    return exposure;

                return 0.0;
            }
            set
            {
                // Write the standard keyword; drop any legacy EXPOSURE so it can't shadow the new value
                RemoveKeyword("EXPOSURE");
                AddKeyword("EXPTIME", value.ToString(), "[s] Imaging Camera Exposure Time");
            }
        }

        // *********************************************************************************************************

        public string FilterName
        {
            get { return GetKeywordValue("FILTER"); }
            set
            {
                string filterName = value.ToUpper() switch
                {
                    _ when value.StartsWith('L') => "L",
                    _ when value.StartsWith('H') => "H",
                    _ when value.StartsWith('O') => "O",
                    _ when value.StartsWith('S') && value != "Shutter" => "S",
                    _ when value.StartsWith('R') => "R",
                    _ when value.StartsWith('G') => "G",
                    _ when value.StartsWith('B') => "B",
                    _ when value.Equals("Shutter") => "Shutter",
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(filterName))
                {
                    string description = filterName switch
                    {
                        "L" => "Astrodon 1.25 via Starlight Xpress USB 7 Position Wheel",
                        "R" => "Astrodon 1.25 via Starlight Xpress USB 7 Position Wheel",
                        "G" => "Astrodon 1.25 via Starlight Xpress USB 7 Position Wheel",
                        "B" => "Astrodon 1.25 via Starlight Xpress USB 7 Position Wheel",
                        "H" => "Astrodon E-Series 1.25 via Starlight Xpress USB 7 Position Wheel",
                        "O" => "Astrodon E-Series 1.25 via Starlight Xpress USB 7 Position Wheel",
                        "S" => "Astrodon E-Series 1.25 via Starlight Xpress USB 7 Position Wheel",
                        "Shutter" => "Opaque 1.25 via Starlight Xpress USB 7 Position Wheel",
                        _ => string.Empty
                    };

                    AddKeyword("FILTER", filterName, description);
                }
            }
        }

        // *********************************************************************************************************

        public double FocalLength
        {
            get
            {
                string value = GetKeywordValue("FOCALLEN");
                if (value == string.Empty)
                    return -1.0;

                double length = Convert.ToDouble(value);
                return length;
            }
            set { AddKeyword("FOCALLEN", value.ToString("F1"), "[mm] OTA Focal length"); }
        }

        public double FocalRatio        
        {
            get
            {
                string value = GetKeywordValue("FOCRATIO");
                if (value == string.Empty) 
                    return -1.0;

                double focalratio = Convert.ToDouble(value);
                return focalratio;
            }

            set { AddKeyword("FOCRATIO", value.ToString("F3"), "[ratio] FocalRatio = FocalLength / ApertureDiameter"); }
        }

        // *********************************************************************************************************

        public int FocuserPosition
        {
            get
            {
                string value = GetKeywordValue("FOCPOS");
                if (value == string.Empty)
                    return 0;

                return Convert.ToInt32(value);
            }
            set { AddKeyword("FOCPOS", value.ToString(), "[um] NiteCrawler Position - 94580 Steps, 0.2667 um/Step"); }
        }

        // *********************************************************************************************************

        public double FocuserTemperature
        {
            get
            {
                string value = GetKeywordValue("FOCTEMP");
                if (value == string.Empty) return -273.0;

                return Convert.ToDouble(value);
            }
            set { AddKeyword("FOCTEMP", value.ToString(), "[degC] NightCrawler Focuser Temperature"); }
        }

        // *********************************************************************************************************

        public eFrame FrameType
        {
            get
            {
                string value = GetKeywordValue("IMAGETYP");
                if (value == string.Empty)
                    return eFrame.EMPTY;

                if (value.Contains("light", StringComparison.CurrentCultureIgnoreCase)) return eFrame.LIGHT;
                if (value.Contains("dark", StringComparison.CurrentCultureIgnoreCase)) return eFrame.DARK;
                if (value.Contains("flat", StringComparison.CurrentCultureIgnoreCase)) return eFrame.FLAT;
                if (value.Contains("bias", StringComparison.CurrentCultureIgnoreCase)) return eFrame.BIAS;

                return eFrame.EMPTY;
            }
            set
            {
                switch (value)
                {
                    case eFrame.LIGHT:
                        AddKeyword("IMAGETYP", "Light", "Type of frame capture");
                        break;
                    case eFrame.DARK:
                        AddKeyword("IMAGETYP", "Dark", "Type of frame capture");
                        break;
                    case eFrame.FLAT:
                        AddKeyword("IMAGETYP", "Flat", "Type of frame capture");
                        break;
                    case eFrame.BIAS:
                        AddKeyword("IMAGETYP", "Bias", "Type of frame capture");
                        break;
                    default:
                        AddKeyword("IMAGETYP", "", "Type of frame capture");
                        break;
                }
            }
        }

        // *********************************************************************************************************

        public int Gain
        {
            get
            {
                string value = GetKeywordValue("GAIN");
                if (value == string.Empty)
                    return -1;

                int gain = Convert.ToInt32(value);
                return gain;
            }
            set
            {
                AddKeyword("GAIN", value.ToString(), "[#] Imaging Camera Gain");
                SetEGain();
            }
        }

        // *********************************************************************************************************

        public string MSTRALG
        {
            get { return GetKeywordValue("MSTRALG"); }
            set { AddKeyword("MSTRALG", value, "[name] Master Frame Rejection Algorithm"); }
        }

        // *********************************************************************************************************

        public int MSTRFRMS
        {
            get
            {
                string value = GetKeywordValue("MSTRFRMS");
                if (value == string.Empty)
                    return -1;

                return Convert.ToInt32(value);
            }
            set { AddKeyword("MSTRFRMS", value.ToString(), "[#] Master Frame Integration Total"); }
        }

        // *********************************************************************************************************

        public int Offset
        {
            get
            {
                string value = GetKeywordValue("OFFSET");
                if (value == string.Empty)
                    return -1;

                int offset = Convert.ToInt32(value);
                return offset;
            }
            set
            {
                if (Camera.Contains("183"))
                {
                    AddKeyword("OFFSET", value.ToString(), "[#] ADU Offset divided by 5");
                    return;
                }

                if (Camera.Contains("533"))
                {
                    AddKeyword("OFFSET", value.ToString(), "[#] ADU Offset divided by 40");
                    return;
                }

                if (Camera.Contains("178"))
                {
                    AddKeyword("OFFSET", value.ToString(), "[#] ADU Offset divided by 18.33");
                    return;
                }

                if (Camera.Contains("144"))
                {
                    RemoveKeyword("OFFSET");
                    return;
                }
            }
        }

        // *********************************************************************************************************

        public double PixelSize
        {
            get
            {
                string value = GetKeywordValue("XPIXSZ");
                if (value == string.Empty)
                    return -1;

                return Convert.ToDouble(value);
            }
            set
            {
                AddKeyword("XPIXSZ", value.ToString(), "[um] Sensor Photosite Width Microns");
                AddKeyword("YPIXSZ", value.ToString(), "[um] Sensor Photosite Height Microns");
            }
        }

        // *********************************************************************************************************

        public double RotatorPosition
        {
            get
            {
                string value = GetKeywordValue("POSANGLE");
                if (value == string.Empty)
                    return double.MinValue;

                return Convert.ToDouble(value);
            }
            set { AddKeyword("POSANGLE", value.ToString(), "[degrees] NightCrawler Mechanical Position 0.001 deg/Step"); }
        }

        // *********************************************************************************************************

        public double RotatorSkyAngle
        {
            get
            {
                string value = GetKeywordValue("OBJCTROT");
                if (value == string.Empty)
                    return double.MinValue;

                return Convert.ToDouble(value);
            }
            set { AddKeyword("OBJCTROT", value.ToString(), "[degrees] Image Sky Angle at Frame Center"); }
        }

        // *********************************************************************************************************

        private void SetEGain()
        {
            // Use graphs found on manufacturer website

            double egain = -1.0;
            double gain = Gain;
            string camera = Camera;

            if (camera == "Z183")
                egain = 3.6059 * Math.Exp(-0.011 * gain);

            if (camera == "Z533")
                egain = (-7e-13 * Math.Pow(gain, 5)) + (1e-9 * Math.Pow(gain, 4)) - (6e-7 * Math.Pow(gain, 3)) + (0.0002 * Math.Pow(gain, 2)) - (0.0356 * gain) + 3.1338;

            if (camera == "Q178")
            {
                if (gain < 4.0)
                    egain = 2.6;
                else
                    egain = 3.8018 * Math.Exp(-0.0117 * gain);
            }

            if (camera == "A144")
                egain = 0.37;

            AddKeyword("EGAIN", egain.ToString(), "[#] Electrons per ADU per manufacturer graphs");
        }

        // *********************************************************************************************************

        public double SensorTemperature
        {
            get
            {
                string value = GetKeywordValue("CCD-TEMP");
                if (value == string.Empty)
                    return -273;

                return Convert.ToDouble(value);
            }
            set { AddKeyword("CCD-TEMP", value.ToString("F1"), "[degC] Imaging Camera Sensor Temperature"); }
        }

        // *********************************************************************************************************

        public double SensorTemperatureSetPoint
        {
            get
            {
                string value = GetKeywordValue("SET-TEMP");
                if (value == string.Empty)
                    return -273.0;

                double temperature = Convert.ToDouble(value);
                return temperature;
            }
            set { AddKeyword("SET-TEMP", value.ToString("F1"), "[degC] Imaging Camera Temperature Set Point"); }
        }

        // *********************************************************************************************************

        public string TargetName
        {
            get { return GetKeywordValue("OBJECT"); }
            set
            {
                if (value.Equals("Master"))
                    AddKeyword("OBJECT", "Master", "[name] Master Calibration Frame");
                else
                    AddKeyword("OBJECT", value, "[name] Target Object Name");
            }
        }

        // *********************************************************************************************************

        public string Telescope
        {
            get { return GetKeywordValue("TELESCOP"); }
            set { AddKeyword("TELESCOP", value.ToString(), "[name] Imaging OTA"); }
        }

        // *********************************************************************************************************

        public List<string> WeightKeyword
        {
            get
            {
                List<string> wList = new List<string>();

                string value = GetKeywordValue("SSWEIGHT");
                if (value != string.Empty)
                    wList.Add("SSWEIGHT");

                value = GetKeywordValue("NWEIGHT");
                if (value != string.Empty)
                    wList.Add("NWEIGHT");

                value = GetKeywordValue("W_SNR");
                if (value != string.Empty)
                    wList.Add("W_SNR");

                value = GetKeywordValue("W_FWHM");
                if (value != string.Empty)
                    wList.Add("W_FWHM");

                value = GetKeywordValue("W_ECC");
                if (value != string.Empty)
                    wList.Add("W_ECC");

                value = GetKeywordValue("W_PSFSNR");
                if (value != string.Empty)
                    wList.Add("W_PSFSNR");

                value = GetKeywordValue("W_PSFS");
                if (value != string.Empty)
                    wList.Add("W_PSFS");

                return wList;
            }
        }

        // *********************************************************************************************************
        // *********************************************************************************************************

        public void SetMasterFrameKeywords()
        {
            // In PixInsight 1.9.2+ , these parameters get moved to within multiple <Property> sections

            // <Property id="PixInsight:ProcessingHistory"
            // ...
            // &lt;parameter id=&quot;numberOfImages&quot; value=&quot;33&quot;/&gt;
            // &lt; parameter id = &quot; rejection & quot; value = &quot; WinsorizedSigmaClip & quot;/ &gt;
            // ...
            // </Property>


            // This code is used in older versions of PixInsight and WBPP
            List<Keyword> keywords = new List<Keyword>(mKeywordList);

            foreach (Keyword node in keywords)
            {
                if (node.Comment.Contains("numberOfImages:"))
                {
                    // Capture all the consecutive digits at the end of the string.
                    string totalFrames = Regex.Match(node.Comment, @"\d+$").Value;

                    AddKeyword("MSTRFRMS", totalFrames, "Number of Integrated SubFrames");
                }

                if (node.Comment.Contains("ImageIntegration.pixelRejection:", StringComparison.OrdinalIgnoreCase))
                {
                    if (node.Comment.Contains("Linear"))
                    {
                        AddKeyword("MSTRALG", "LFC", "Linear Fit Clipping");
                    }
                    
                    else if (node.Comment.Contains("Studentized"))
                    {
                        AddKeyword("MSTRALG", "ESD", "Generalized Extreme Studentized Deviate Clipping");
                    }

                    else if (node.Comment.Contains("Winsor"))
                    {
                        AddKeyword("MSTRALG", "WSC", "Winsorized Sigma Clipping");
                    }

                    else if (node.Comment.Contains("Sigma"))
                    {
                        AddKeyword("MSTRALG", "SC", "Sigma Clipping");
                    }
                }
            }
        }
        /// <summary>
        /// Stamps a measured plate solution (astap-plate-solve, 2026-08-06): pointing + measured sky
        /// angle + the full standard WCS so any future software can read the solution. High-tier
        /// programmatic surface — the one place solver results become keywords. AddKeyword replaces,
        /// so solved values always win over planned ones (disk is truth). None of these names appear
        /// in RemoveUnwantedKeywords (verified 2026-08-06) — keep it that way.
        /// </summary>
        public void SetPlateSolution(Solver.SolveResult solution)
        {
            System.Globalization.CultureInfo inv = System.Globalization.CultureInfo.InvariantCulture;

            AddKeyword("RA", solution.RaDegrees.ToString("0.######", inv), "[deg] Solved right ascension of frame center (ASTAP)");
            AddKeyword("DEC", solution.DecDegrees.ToString("0.######", inv), "[deg] Solved declination of frame center (ASTAP)");
            RotatorSkyAngle = solution.PositionAngleDegrees;   // OBJCTROT — measured, replaces planned

            AddKeyword("CTYPE1", "RA---TAN", "Solved WCS projection (ASTAP)");
            AddKeyword("CTYPE2", "DEC--TAN", "Solved WCS projection (ASTAP)");
            AddKeyword("EQUINOX", "2000.0", "[yr] Equinox of solved coordinates");
            AddKeyword("CRVAL1", solution.Crval1, "[deg] Solved WCS RA of reference pixel (ASTAP)");
            AddKeyword("CRVAL2", solution.Crval2, "[deg] Solved WCS DEC of reference pixel (ASTAP)");
            AddKeyword("CRPIX1", solution.Crpix1, "[px] Solved WCS reference pixel X (frame center)");
            AddKeyword("CRPIX2", solution.Crpix2, "[px] Solved WCS reference pixel Y (frame center)");
            AddKeyword("CD1_1", solution.Cd1_1, "Solved WCS CD matrix (ASTAP)");
            AddKeyword("CD1_2", solution.Cd1_2, "Solved WCS CD matrix (ASTAP)");
            AddKeyword("CD2_1", solution.Cd2_1, "Solved WCS CD matrix (ASTAP)");
            AddKeyword("CD2_2", solution.Cd2_2, "Solved WCS CD matrix (ASTAP)");
            if (solution.Crota1 != string.Empty)
                AddKeyword("CROTA1", solution.Crota1, "[deg] Solved WCS image twist of X axis (ASTAP)");
            if (solution.Crota2 != string.Empty)
                AddKeyword("CROTA2", solution.Crota2, "[deg] Solved WCS image twist of Y axis (ASTAP)");
        }

        // *********************************************************************************************************

        public void RemoveUnwantedKeywords()
        {
            RemoveKeyword("ALT-OBS");
            RemoveKeyword("AOCAMBT");
            RemoveKeyword("COMMENT");
            RemoveKeyword("Camera");
            RemoveKeyword("CREJECT");
            RemoveKeyword("DATE-END");
            RemoveKeyword("DBLKEYWD");
            RemoveKeyword("FILENAME");
            RemoveKeyword("HISTORY");
            RemoveKeyword("INTKEYWD");
            RemoveKeyword("LAT-OBS");
            RemoveKeyword("LONG-OBS");
            RemoveKeyword("NOISE00");
            RemoveKeyword("NOISEA00");
            RemoveKeyword("NOISEH00");
            RemoveKeyword("NOISEL00");
            RemoveKeyword("NUM-FRMS");
            RemoveKeyword("OBJCTDEC");
            RemoveKeyword("OBJCTRA");
            RemoveKeyword("OBSGEO-B");
            RemoveKeyword("OBSGEO-H");
            RemoveKeyword("OBSGEO-L");
            RemoveKeyword("PROTECT");
            RemoveKeyword("PSFFLP00");
            RemoveKeyword("PSFFLX00");
            RemoveKeyword("PSFMFL00");
            RemoveKeyword("PSFMFP00");
            RemoveKeyword("PSFMST00");
            RemoveKeyword("PSFNST00");
            RemoveKeyword("PSFSGL00");
            RemoveKeyword("PSFSGN00");
            RemoveKeyword("PSFSGP00");
            RemoveKeyword("PSFSGTYP");
            RemoveKeyword("Protected");
            RemoveKeyword("RADESYS");
            RemoveKeyword("REJECTIO");
            RemoveKeyword("REJECTION");
            RemoveKeyword("RESOUNIT");
            RemoveKeyword("RJCT-ALG");
            RemoveKeyword("Rejection");
            RemoveKeyword("STRKEYWD");
            RemoveKeyword("TOTALFRA");
            RemoveKeyword("TOTALFRAMES");
            RemoveKeyword("XBAYROFF");
            RemoveKeyword("XORGSUBF");
            RemoveKeyword("YBAYROFF");
            RemoveKeyword("YORGSUBF");
        }

        // #########################################################################################################
        // #########################################################################################################

    }
}
