using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Astronomy.XISF.Compression;

namespace XisfFileManager.Files.XML
{
    internal sealed class Xml
    {
        // Compiled Regex Match Expressions

        // Remove the ProcessingHistory block
        private static readonly Regex _historyRx = new Regex(@"\<Property.*?;\</Property\>", RegexOptions.Singleline | RegexOptions.Compiled);

        //Find each <Image…>…</Image> block
        private static readonly Regex _imageBlockRx = new Regex(@"(<Image.*?>)(.*?)(</Image>)", RegexOptions.Singleline | RegexOptions.Compiled);

        // Remove any <Property…></Property> or self-closing <Property…/> inside an image
        private static readonly Regex _propertyInsideImageRx = new Regex(@"\<Property.*?(?:/Property\>|/>)", RegexOptions.Singleline | RegexOptions.Compiled);

        public static readonly Regex XisfFile_XmlVersionBlockRx = new Regex(
                    @"<\?xml[\s\S]*?\?>",
                    RegexOptions.Singleline
                  | RegexOptions.Compiled
                );

        public static readonly Regex XisfFile_CommentBlockRx = new Regex(
                            @"<!--[\s\S]*?-->",
                            RegexOptions.Singleline
                          | RegexOptions.Compiled
                        );

        public static readonly Regex XisfFile_XisfBlockRx = new Regex(
                            @"<xisf[\s\S]*?xisf>",
                            RegexOptions.Singleline
                          | RegexOptions.Compiled
                        );

        public static readonly Regex XisfFile_MetadataBlockRx = new Regex(
                            @"<Metadata[\s\S]*?Metadata>",
                            RegexOptions.Singleline
                          | RegexOptions.Compiled
                        );

        public static readonly Regex IntegrationImageRx = new Regex(
                @"<Image\b           # start an <Image tag
                  [^>]*\bid=""integration""  # with id=""integration""
                  [^>]*?             # other attrs, non-greedy
                  (?:                # then either
                    /\>              #  self-close it
                  |                  # or
                    \>               #  open tag close
                    (?:[\s\S]*?)     #  lazy-grab any content
                    </Image>         #  until the closing tag
                  )",
                    RegexOptions.IgnorePatternWhitespace
                  | RegexOptions.Singleline
                  | RegexOptions.Compiled
                );

        public static readonly Regex RemoveProcessingHistoryRx = new Regex(
                @"<Property\b
                   [^>]*\bid=""PixInsight:ProcessingHistory""
                   [^>]*(?:/\>|>(?:[\s\S]*?)</Property>)",
                RegexOptions.Singleline
              | RegexOptions.IgnorePatternWhitespace
              | RegexOptions.Compiled
            );

        public static readonly Regex RemoveAllPropertyRx = new Regex(
                @"<Property\b          # start a Property tag
                   [^>]*?              # any attrs, non-greedy
                   (?:                 # then either
                     /\>               #  a self-closing tag
                   |                   # or
                     \>                #  close start-tag
                     (?:[\s\S]*?)      #  lazily grab inner content
                     </Property>       #  until the end-tag
                   )",
                RegexOptions.Singleline
              | RegexOptions.IgnorePatternWhitespace
              | RegexOptions.Compiled
            );

        // ***********************************************************************************
        // ***********************************************************************************

        // Constructor
        public static (string FixedXml, bool Modified) FixXisfXml(string xmlString)
        {
            // Remove any bogus characters, extra single quotes and anything after the closing </xisf>.
            // Data after </xisf> should just be image pixel data. We are reading values from and changing xml text so no pixel data is needed. 
            // Clearing data after </xisf> will also clear any alignment padding before image pixels. 

            bool modified = false;
            string step;

            // Step 1: Remove all non-ASCII characters
            step = new string(xmlString.Where(c => c < 128).ToArray());
            if (!string.Equals(step, xmlString, StringComparison.Ordinal))
            {
                modified = true;
            }
            xmlString = step;

            // Step 2: Remove single quotes inside FITS Keywords
            step = xmlString.Replace("'", "");
            if (!string.Equals(step, xmlString, StringComparison.Ordinal))
            {
                //modified = true;
            }
            xmlString = step;
            
            // Step 3: Remove anything after </xisf> in xmlString (pixel data)
            int endIndex = xmlString.IndexOf("</xisf>") + "</xisf>".Length;
            if (endIndex > 0)
            {
                if (endIndex < xmlString.Length)
                {
                    modified = true;
                }
                step = xmlString.Substring(0, endIndex);
                xmlString = step;
            }
            else
            {
                throw new XisfXmlException("Closing </xisf> tag not found in the image XML string.");
            }
          
            return (xmlString, modified);
        }

        // ***********************************************************************************
        // ***********************************************************************************

        public static (string XmlString, bool Modified) ValidateXisfXml(string xmlString)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            bool modified = false;

            while (!string.IsNullOrEmpty(xmlString))
            {
                using (var reader = XmlReader.Create(new StringReader(xmlString), settings))
                {
                    try
                    {
                        // If this succeeds, the XML is valid
                        while (reader.Read()) { }
                        return (xmlString, modified);
                    }
                    catch (XmlSchemaValidationException ex)
                    {
                        throw new XisfXmlException(
                            $"Xml.cs ValidateXisfXml() - XML schema validation error: {ex.Message}"
                        );
                    }
                    catch (XmlException ex)
                    {
                        // Locate the junk between tags and remove it
                        int errorPos = ex.LinePosition;
                        int startIndex = xmlString.LastIndexOf('>', errorPos) + 1;
                        int endIndex = xmlString.IndexOf('<', errorPos);

                        if (startIndex >= 0 && endIndex >= 0 && endIndex > startIndex)
                        {
                            xmlString = xmlString.Remove(startIndex, endIndex - startIndex);
                            modified = true;
                            // Loop again to re-validate the trimmed XML
                        }
                        else
                        {
                            throw new XisfXmlException(
                                $"Xml.cs ValidateXisfXml() - XML parsing " +
                                $"error at line {ex.LineNumber}, position {ex.LinePosition}: {ex.Message}"
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new XisfXmlException(
                            $"Xml.cs ValidateXisfXml() - Unexpected error: {ex.Message}"
                        );
                    }
                }
            }

            // If we ever exit the loop without returning, something’s wrong
            throw new XisfXmlException("Xml.cs ValidateXisfXml() - Unexpected empty XML.");
        }


        // ***********************************************************************************
        /// <summary>
        /// Removes all `<Image>` elements whose @id matches any of the supplied IDs.
        /// </summary>
        /// <param name="xmlString">A well-formed XML document containing `<Image>` elements.</param>
        /// <param name="imageIds">One or more id values to remove (e.g. "rejection_low", "rejection_high").</param>
        /// <returns>
        /// A tuple of
        ///  • CleanedXml: the XML string with those `<Image>` elements stripped out  
        ///  • Modified: true if any `<Image>` was removed  
        /// </returns>
        public static (string CleanedXml, bool Modified) RemoveImage_ById(
            string xmlString,
            params string[] imageIds
        )
        {
            string before = xmlString;

            var doc = XDocument.Parse(xmlString);

            // Find all <Image> nodes whose @id is in the imageIds array
            var imagesToRemove = doc
                .Descendants()
                .Where(e => e.Name.LocalName == "Image"
                         && imageIds.Contains((string?)e.Attribute("id")))
                .ToList();

            // Remove them
            foreach (var img in imagesToRemove)
                img.Remove();

            string cleaned = doc.ToString(SaveOptions.DisableFormatting);
            bool modified = !string.Equals(before, cleaned, StringComparison.Ordinal);

            return (cleaned, modified);
        }

        // ***********************************************************************************
        // ***********************************************************************************

        /// <summary>
        /// Strips out any `<Property id="PixInsight:ProcessingHistory">…</Property>`
        /// (or `<Property …/>`) blocks nested inside the given `<Image id="…">…</Image>` element.
        /// If no `<Image id="imageId">` exists, the first `<Image>…</Image>` block is used.
        /// </summary>
        /// <param name="xmlInput">The full XML document.</param>
        /// <param name="imageId">The value of the Image/@id to clean (e.g. "integration").</param>
        /// <returns>
        /// A tuple of
        ///  • CleanedXml: the document with history properties removed  
        ///  • Modified: true if anything was stripped out  
        /// </returns>
        public static (string CleanedXml, bool Modified) RemoveImageProcessingHistory_ById(
            string xmlInput,
            string imageId
        )
        {
            string before = xmlInput;

            // Specific <Image id="…">…</Image>
            string specificPattern = $@"
        (<Image\b
           [^>]*\bid=""{Regex.Escape(imageId)}""
           [^>]*>)           # the start-tag with matching id
        ([\s\S]*?)          # the inner XML
        (</Image>)          # the closing tag
    ";

            // Fallback: first <Image…>…</Image>
            const string genericPattern = @"
        (<Image\b[^>]*>)    # any start-tag
        ([\s\S]*?)          # the inner XML
        (</Image>)          # the closing tag
    ";

            var opts = RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace;
            bool hasSpecific = Regex.IsMatch(xmlInput, specificPattern, opts);
            string chosenPattern = hasSpecific ? specificPattern : genericPattern;

            // Only replace the first matched block
            var blockRx = new Regex(chosenPattern, opts);
            string cleaned = blockRx.Replace(xmlInput, m =>
            {
                var openTag = m.Groups[1].Value;
                var innerXml = m.Groups[2].Value;
                var closeTag = m.Groups[3].Value;

                // Remove history properties inside that block
                string cleanedInner = RemoveProcessingHistoryRx.Replace(innerXml, "");

                return openTag + cleanedInner + closeTag;
            }, count: 1);

            bool modified = !string.Equals(before, cleaned, StringComparison.Ordinal);
            return (cleaned, modified);
        }


        // ***********************************************************************************
        // ***********************************************************************************

        /// <summary>
        /// Removes *all* <Property…>…</Property> (and self-closing)
        /// children from a targeted <Image> block.
        /// If no <Image id="imageId"> exists, uses the first <Image>…</Image> in the doc.
        /// </summary>
        /// <param name="xmlInput">Complete XML document text.</param>
        /// <param name="imageId">The Image/@id whose Property children to strip.</param>
        /// <returns>
        /// A tuple:
        ///  • CleanedXml: the XML with those <Property> elements removed  
        ///  • Modified: true if any were removed  
        /// </returns>
        public static (string CleanedXml, bool Modified) RemoveImageProperties_ById(
            string xmlInput,
            string imageId
        )
        {
            string before = xmlInput;

            // Pattern for a specific <Image id="…">…</Image>
            string specificPattern = $@"
        (<Image\b
           [^>]*\bid=""{Regex.Escape(imageId)}""
           [^>]*>)
        ([\s\S]*?)
        (</Image>)
    ";

            // Generic pattern for the FIRST <Image…>…</Image> block
            const string genericPattern = @"
        (<Image\b[^>]*>)
        ([\s\S]*?)
        (</Image>)
    ";

            // Decide which pattern to use
            var opts = RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace;
            bool hasSpecific = Regex.IsMatch(xmlInput, specificPattern, opts);
            string chosenPattern = hasSpecific ? specificPattern : genericPattern;

            // Only strip inside that one block
            var regex = new Regex(chosenPattern, opts);
            string cleaned = regex.Replace(xmlInput, m =>
            {
                var openTag = m.Groups[1].Value;
                var inner = m.Groups[2].Value;
                var closeTag = m.Groups[3].Value;

                // strip every <Property…>…</Property> or self-closing <Property…/>
                string cleanedInner = RemoveAllPropertyRx.Replace(inner, "");

                return openTag + cleanedInner + closeTag;
            }, count: 1); // only replace the first match

            bool modified = !string.Equals(before, cleaned, StringComparison.Ordinal);
            return (cleaned, modified);
        }


        // ***********************************************************************************
        // ***********************************************************************************

        /// <summary>
        /// Extracts the attachment start/length and image width/height
        /// from the `<Image id="…">` element in the given XML.
        /// If no `<Image id="imageId">` exists, it falls back to the first `<Image>…</Image>` block.
        /// </summary>
        /// <param name="xmlString">Complete XML text containing one or more `<Image>` elements.</param>
        /// <param name="imageId">The Image/@id to look up (e.g. "integration").</param>
        /// <returns>
        /// A tuple of
        ///  • TargetAttachmentStart  
        ///  • TargetAttachmentLength  
        ///  • TargetAttachmentWidth  
        ///  • TargetAttachmentHeight  
        /// </returns>
        public static (int TargetAttachmentStart,
                       int TargetAttachmentLength,
                       int TargetAttachmentWidth,
                       int TargetAttachmentHeight)
        GetImageExtents(string xmlString, string imageId)
        {
            var doc = XDocument.Parse(xmlString);

            // Try the specific id first, otherwise pick the first <Image> element
            var img = doc
                .Descendants()
                .FirstOrDefault(e =>
                    e.Name.LocalName == "Image" &&
                    (string?)e.Attribute("id") == imageId
                )
                ?? doc
                    .Descendants()
                    .FirstOrDefault(e => e.Name.LocalName == "Image");

            if (img == null)
                throw new ArgumentException(
                    "No <Image> element found in the document.",
                    nameof(xmlString)
                );

            int width = 0, height = 0, start = 0, length = 0;

            // Parse geometry="W:H:..."
            var geom = img.Attribute("geometry")?.Value;
            if (!string.IsNullOrEmpty(geom))
            {
                var parts = geom.Split(':');
                if (parts.Length >= 2)
                {
                    width = Convert.ToInt32(parts[0]);
                    height = Convert.ToInt32(parts[1]);
                }
            }

            // Parse location="attachment:start:length"
            var loc = img.Attribute("location")?.Value;
            if (!string.IsNullOrEmpty(loc))
            {
                var parts = loc.Split(':');
                if (parts.Length >= 3)
                {
                    start = Convert.ToInt32(parts[1]);
                    length = Convert.ToInt32(parts[2]);
                }
            }

            return (start, length, width, height);
        }

        // ***********************************************************************************
        // ***********************************************************************************

        /// <summary>
        /// Reads the main <Image> element's sample format and existing compression/checksum, and derives the
        /// byte-shuffle item size (bytes-per-sample). Used to decide whether a block is already compressed and,
        /// when it is not, how to shuffle it before zlib compression.
        /// </summary>
        /// <param name="xmlString">Full XML header text.</param>
        /// <param name="imageId">The Image/@id to prefer (e.g. "integration"); falls back to the first Image.</param>
        /// <returns>
        /// SampleFormat (e.g. "UInt16"; "" if absent), ItemSize (bytes per sample for shuffling; ≥ 1),
        /// and the parsed existing compression/checksum (Codec == None when the block is uncompressed).
        /// </returns>
        public static (string SampleFormat, int ItemSize, BlockCompressionInfo Compression)
        GetImageBlockInfo(string xmlString, string imageId)
        {
            var doc = XDocument.Parse(xmlString);

            var img = doc
                .Descendants()
                .FirstOrDefault(e =>
                    e.Name.LocalName == "Image" &&
                    (string?)e.Attribute("id") == imageId)
                ?? doc
                    .Descendants()
                    .FirstOrDefault(e => e.Name.LocalName == "Image");

            string sampleFormat = img?.Attribute("sampleFormat")?.Value ?? string.Empty;
            BlockCompressionInfo compression = BlockCompressionInfo.Parse(
                img?.Attribute("compression")?.Value,
                img?.Attribute("checksum")?.Value);

            int itemSize;
            if (compression.IsCompressed)
            {
                // Already compressed: the shuffle item size is recorded in the compression attribute.
                itemSize = compression.ItemSize;
            }
            else
            {
                // Uncompressed: prefer the canonical sampleFormat, else derive bytes/sample from geometry
                // and the (uncompressed) block length. Shuffle round-trips for any recorded itemSize, so a
                // wrong value would only cost ratio, never correctness.
                itemSize = BytesPerSample(sampleFormat);
                if (itemSize <= 0)
                    itemSize = DeriveItemSizeFromGeometry(img);
                if (itemSize <= 0)
                    itemSize = 1;
            }

            return (sampleFormat, itemSize, compression);
        }

        /// <summary>Bytes per sample for an XISF sampleFormat token, or 0 if unknown.</summary>
        private static int BytesPerSample(string sampleFormat) => sampleFormat switch
        {
            "UInt8" => 1,
            "UInt16" => 2,
            "UInt32" => 4,
            "UInt64" => 8,
            "Float32" => 4,
            "Float64" => 8,
            "Complex32" => 8,
            "Complex64" => 16,
            _ => 0
        };

        /// <summary>
        /// Derive bytes-per-sample for an uncompressed image as blockLength / (width × height × channels),
        /// where channels is the third geometry component (default 1). Returns 0 when it cannot be derived.
        /// </summary>
        private static int DeriveItemSizeFromGeometry(XElement? img)
        {
            if (img == null)
                return 0;

            var geom = img.Attribute("geometry")?.Value;
            var loc = img.Attribute("location")?.Value;
            if (string.IsNullOrEmpty(geom) || string.IsNullOrEmpty(loc))
                return 0;

            string[] g = geom.Split(':');
            if (g.Length < 2)
                return 0;

            string[] l = loc.Split(':');
            if (l.Length < 3)
                return 0;

            if (!long.TryParse(g[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out long width) ||
                !long.TryParse(g[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out long height) ||
                !long.TryParse(l[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out long length))
                return 0;

            long channels = 1;
            if (g.Length >= 3)
                long.TryParse(g[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out channels);
            if (channels < 1)
                channels = 1;

            long denom = width * height * channels;
            if (denom <= 0 || length <= 0 || length % denom != 0)
                return 0;

            return (int)(length / denom);
        }

        // ***********************************************************************************
        // ***********************************************************************************

        /// <summary>
        /// Reads the “numberOfImages” and “rejection” parameters from
        /// the PixInsight:ProcessingHistory Property inside the specified <Image>.
        /// If no matching <Image id="…"> exists, returns (-1, "N/A").
        /// </summary>
        /// <param name="xmlString">Full XML document text.</param>
        /// <param name="imageId">The Image/@id to look up (e.g. "integration").</param>
        /// <returns>
        /// A tuple of
        ///  • MasterFrames: the parsed numberOfImages (–1 if not found)  
        ///  • MasterAlgo:    the rejection algorithm ( "N/A" if not found )  
        /// </returns>
        public static (int MasterFrames, string MasterAlgo) GetMasterFrameKeywords(
            string xmlString,
            string imageId
        )
        {
            var doc = XDocument.Parse(xmlString);

            // 1) Find the target <Image> by id
            var img = doc
                .Descendants()
                .FirstOrDefault(e =>
                    e.Name.LocalName == "Image" &&
                    (string?)e.Attribute("id") == imageId
                );

            // 2) If none, bail out immediately
            if (img == null)
                return (-1, "N/A");

            int masterFrames = 0;
            string? masterAlgo = null;

            // 3) Locate the <Property id="PixInsight:ProcessingHistory"> under that <Image>
            var historyProps = img
                .Elements()
                .Where(e =>
                    e.Name.LocalName == "Property" &&
                    (string?)e.Attribute("id") == "PixInsight:ProcessingHistory"
                );

            // 4) For each such Property, parse its inner XML to pull out <parameter> tags
            foreach (var prop in historyProps)
            {
                // raw text of the <Property>…</Property> node
                string raw = prop.Value;

                // strip any embedded XML declaration
                raw = Regex.Replace(raw, @"<\?xml[\s\S]*?\?>", "");

                // wrap and parse so we can query <parameter> elements
                var wrapper = XElement.Parse("<root>" + raw + "</root>");

                foreach (var param in wrapper.Descendants()
                                             .Where(x => x.Name.LocalName == "parameter"))
                {
                    var pid = (string?)param.Attribute("id");
                    var pval = (string?)param.Attribute("value");

                    if (pid == "numberOfImages" &&
                        int.TryParse(pval, out var n))
                    {
                        masterFrames = n;
                    }

                    if (pid == "rejection" && pval != null)
                    {
                        // apply string-based mapping
                        if (pval.Contains("Linear", StringComparison.OrdinalIgnoreCase))
                            masterAlgo = "LFC";        // Linear Fit Clipping
                        else if (pval.Contains("Studentized", StringComparison.OrdinalIgnoreCase))
                            masterAlgo = "ESD";        // Extreme Studentized Deviate
                        else if (pval.Contains("Winsor", StringComparison.OrdinalIgnoreCase))
                            masterAlgo = "WSC";        // Winsorized Sigma Clipping
                        else if (pval.Contains("Sigma", StringComparison.OrdinalIgnoreCase))
                            masterAlgo = "SC";         // Sigma Clipping
                        else
                            masterAlgo = pval;         // fallback to raw value
                    }
                }
            }

            // 5) Default if missing
            if (masterAlgo == null)
            {
                masterFrames = 0;
                masterAlgo = "N/A";
            }

            return (masterFrames, masterAlgo);
        }



        // ***********************************************************************************
        // ***********************************************************************************

        public static string RemoveNonEvenPairs(string input, string openString, string closeString)
        {
            string pattern = $@"{Regex.Escape(openString)}[^{Regex.Escape(openString + closeString)}]*{Regex.Escape(closeString)}";
            MatchCollection matches = Regex.Matches(input, pattern);

            var result = new System.Text.StringBuilder();

            foreach (Match match in matches)
            {
                if (HasEvenPairs(match.Value, openString, closeString))
                {
                    result.Append(match.Value);
                }
            }

            return result.ToString();
        }

        // ***********************************************************************************
        // ***********************************************************************************

        public static bool HasEvenPairs(string input, string openString, string closeString)
        {
            int openCount = Regex.Count(input, Regex.Escape(openString));
            int closeCount = Regex.Count(input, Regex.Escape(closeString));

            return openCount == closeCount;
        }

        // ***********************************************************************************
        // ***********************************************************************************

        public static bool ContainsNonAsciiOrInvalidChars(string input, out char firstInvalidChar)
        {
            // Check for non-ASCII characters
            bool containsNonAscii = Regex.IsMatch(input, @"[^\x00-\x7F]");

            // Check for characters other than the allowed set
            Match match = Regex.Match(input, @"[^A-Za-z0-9+\-./:()_ .<>="",*%]");
            if (match.Success)
            {
                firstInvalidChar = match.Value[0];
                return true;
            }

            firstInvalidChar = '\0';

            return containsNonAscii;
        }

        // ***********************************************************************************
        // ***********************************************************************************
    }

    public class XisfXmlException : Exception
    {
        public XisfXmlException(string message) : base(message) { }
    }
}
