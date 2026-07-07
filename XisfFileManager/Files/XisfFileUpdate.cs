using System.Text;
using System.Xml;
using XisfFileManager.Configuration;
using XisfFileManager.Files.Compression;
using XisfFileManager.Files.XML;
using XisfFileManager.Globals;
using XisfFileManager.Helpers;

namespace XisfFileManager.Files
{
    public class XisfFileUpdate
    {
        private Buffer mBuffer = new();
        private List<Buffer> mBufferList = new();

        /// <summary>Outcome of the most recent <see cref="UpdateFileAsync"/> call, for status reporting.</summary>
        public eUpdateOutcome LastUpdateOutcome { get; private set; }


        /// <summary>
        /// Extracts a substring from the source string that is between the specified start and end tokens.
        /// </summary>
        /// <param name="source">The source string to extract the value from.</param>
        /// <param name="startToken">The token that marks the beginning of the value to extract.</param>
        /// <param name="endToken">The token that marks the end of the value to extract.</param>
        /// <returns>The extracted value if found; otherwise, an empty string.</returns>
        private static string ExtractValue(string source, string startToken, string endToken)
        {
            int start = source.IndexOf(startToken);
            if (start == -1) return "";

            start += startToken.Length; // Move the start index to the end of the startToken
            if (start >= source.Length) return "";

            int end = source.IndexOf(endToken, start);
            if (end == -1) return "";

            return source.Substring(start, end - start).Trim();
        }


        /// <summary>
        /// Extracts a substring from the source string that is between the specified start and end tokens,
        /// and occurs after the preToken.
        /// </summary>
        /// <param name="source">The source string to extract the value from.</param>
        /// <param name="preToken">The token that must precede the startToken in the source string.</param>
        /// <param name="startToken">The token that marks the beginning of the value to extract.</param>
        /// <param name="endToken">The token that marks the end of the value to extract.</param>
        /// <returns>The extracted value if found; otherwise, an empty string.</returns>
        private static string ExtractValue(string source, string preToken, string startToken, string endToken)
        {
            int preStart = source.IndexOf(preToken);
            if (preStart == -1) return "";

            // Adjust the starting position to search for startToken and endToken after preToken
            string adjustedSource = source.Substring(preStart + preToken.Length);

            int start = adjustedSource.IndexOf(startToken);
            if (start == -1) return "";

            int end = adjustedSource.IndexOf(endToken, start + startToken.Length);
            if (end == -1) return "";

            return adjustedSource.Substring(start + startToken.Length, end - (start + startToken.Length));
        }


        // ##############################################################################################################################################
        // ##############################################################################################################################################
        /// <summary>
        /// Updates the specified XISF file and writes the changes to a destination path.
        /// The method checks if the file is locked, validates the keyword update mode,
        /// and processes the file's XML content, updating keywords and attachments as needed.
        /// </summary>
        /// <param name="xFile">The XISF file to update.</param>
        /// <param name="destinationPath">The destination path to write the updated file.</param>
        /// <returns>True if the file is updated successfully; otherwise, false.</returns>
        public async Task<bool> UpdateFileAsync(XisfFile xFile, string destinationPath, Action<string>? onWriting = null)
        {
            // PROTECT is enforced here, not at call sites: a protected file is never written, no matter
            // which feature asks. Deliberate write paths (Calibration, FluxDensity) set FORCE first.
            if (xFile.KeywordUpdateMode == eKeywordUpdateMode.PROTECT)
            {
                LastUpdateOutcome = eUpdateOutcome.Protected;
                return true;
            }

            int delay = 0;
            while (FileHelpers.IsFileLocked(xFile.FilePath) && delay < 100)
            {
                delay++;
                await Task.Delay(10);
            }

            if (delay == 100)
            {
                MessageBox.Show("File is locked", xFile.FilePath, MessageBoxButtons.OK);
                LastUpdateOutcome = eUpdateOutcome.Failed;
                return false;
            }

            // Normalize keywords before writing (converts CREATOR->SWCREATE, DATE-OBS->DATE-LOC, EXPOSURE->EXPTIME)
            xFile.NormalizeKeywords();

            // "Save if needed": skip only when nothing would change. In UPDATE_NEW mode that means the keywords
            // already match the on-disk XML AND the image block is already compressed. A keyword change OR an
            // uncompressed block (compression needed) both require a rewrite. FORCE always writes.
            if (xFile.KeywordUpdateMode == eKeywordUpdateMode.UPDATE_NEW && KeywordsMatchXml(xFile) && xFile.IsImageCompressed)
            {
                LastUpdateOutcome = eUpdateOutcome.Skipped;
                return true;
            }

            // Committed to writing this file (keyword change and/or compression). Report it now — before the
            // potentially slow compress + write — so the UI shows the file that is actually being written.
            onWriting?.Invoke(destinationPath);

            int xisfStart;
            int xisfEnd;

            byte[] binaryFileData = new byte[XisfConstants.MaxFileReadBytes];
            mBufferList = new List<Buffer>();

            try
            {
                xFile.bModified = false;
               
                using (Stream stream = new FileStream(xFile.FilePath, FileMode.Open))
                {
                    bool modified;
                    // *******************************************************************************************************************************
                    // *******************************************************************************************************************************

                    // Read entire XISF file (up to 1 GB) into rawFileData and create an xml document
                    BinaryReader br = new BinaryReader(stream);
                    binaryFileData = br.ReadBytes(XisfConstants.MaxFileReadBytes);
                    br.Close();

                    // *******************************************************************************************************************************
                    // *******************************************************************************************************************************

                    xisfStart = BinaryFind(binaryFileData, "<xisf "); // returns the position of '<'
                    // NOTE: This value will change AFTER Keyword Replacement a few lines below
                    xisfEnd = BinaryFind(binaryFileData, "</xisf>") + "</xisf>".Length;  // returns the position immediately after '>'                

                    // Convert <xisf to /xisf> to a string and then parse string as xml into a new doc
                    string xmlString = Encoding.UTF8.GetString(binaryFileData, xisfStart, xisfEnd);

                    // ******************************************************************************************
                    // Clean up and validate xmlString

                    // This should not be needed once all xisf files have been passed throught this application
                    (xmlString, modified) = Xml.FixXisfXml(xmlString);
                    xFile.bModified |= modified;

                    // This should not be needed once all xisf files have been passed throught this application
                    (xmlString, modified) = Xml.ValidateXisfXml(xmlString);
                    xFile.bModified |= modified;

                    // *******************************************************************************************************************************
                    // *******************************************************************************************************************************

                    // Remove all descendants from xFile.mXDoc that contain an "attachment" attribute that do not match the criteria specified by imageNode (our main image)

                    // Remove rejection image blocks
                    (xmlString, modified) = Xml.RemoveImage_ById(xmlString, "rejection_high");
                    xFile.bModified |= modified;

                    (xmlString, modified) = Xml.RemoveImage_ById(xmlString, "rejection_low");
                    xFile.bModified |= modified;

                    (xmlString, modified) = Xml.RemoveImageProperties_ById(xmlString, "integration"); // Includes ProcessHistory
                    xFile.bModified |= modified;

                    // ******************************************************************************************

                    // Create an Xml Document from the xmlString with the proper namespace
                    XmlDocument xmlDoc = new XmlDocument();
                    XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
                    namespaceManager.AddNamespace("ns", "http://www.pixinsight.com/xisf");

                    // The new document does not include the XmLVersion and PixInsight comment section - Add them
                    xmlDoc.LoadXml(xFile.XmlVersionText + xFile.XmlCommentText + xmlString);

                    
                    RemoveUnwantedAttachments(xmlDoc);

                    // *******************************************************************************************************************************
                    // *******************************************************************************************************************************

                    // A list of unsed FITSKeywords will be removed. This method call should be able to be removed in the future.

                    xFile.KeywordList.RemoveUnwantedKeywords();

                    // Replace all existing FITSKeywords with FITSKeywords from our list (mFile.KeywordList)

                    ReplaceAllFitsKeywords(xmlDoc, xFile);

                    // *******************************************************************************************************************************
                    // *******************************************************************************************************************************

                    // Compress the image data block (zlib+sh + SHA-1) unless it is already compressed.
                    // Already-compressed blocks (any codec) are copied verbatim with their existing attributes.
                    bool compressNow = !xFile.IsImageCompressed;
                    BlockCompressionResult compressionResult = default;
                    if (compressNow)
                    {
                        byte[] rawImageBlock = new byte[xFile.TargetAttachmentLength];
                        Array.Copy(binaryFileData, xFile.TargetAttachmentStart, rawImageBlock, 0, xFile.TargetAttachmentLength);

                        compressionResult = XisfBlockCompression.Compress(rawImageBlock, xFile.ItemSize);

                        // Add compression/checksum to the <Image> before location/padding converge against the final header.
                        ApplyCompressionAttributes(xmlDoc, compressionResult.Info);
                    }

                    // *******************************************************************************************************************************
                    // *******************************************************************************************************************************

                    // We need to set the start address and length of the image attachement due to any changes in the <xisf> to </xisf> length

                    if (xFile.BlockAlignmentSize != -1)
                    {
                        int possibleBogusImageAttachmentLocation = FindExistingImageStartingLocation(binaryFileData);
                        if (possibleBogusImageAttachmentLocation % xFile.BlockAlignmentSize != 0)
                        {
                            if ((possibleBogusImageAttachmentLocation - 1) % xFile.BlockAlignmentSize != 0)
                                MessageBox.Show(Path.GetFileName(xFile.FilePath), "Image Attachment Start Location was not Block Aligned. Continuing...");
                        }
                    }
                    // When compressing, the stored block size becomes the compressed size; otherwise leave it unchanged.
                    xFile.TargetAttachmentPadding = SetImageAttachmentLocation(xmlDoc, xFile,
                        compressNow ? compressionResult.CompressedBytes.Length : (int?)null);

                    // *******************************************************************************************************************************
                    // *******************************************************************************************************************************

                    // "XISF0100" is the XISF Signature. The length of the xml section is stored in the 8th and 9th bytes of the signature
                    // Assumes that xmlLength is less than 65536 bytes
                    //                                                                X     I     S     F     0     1     0     0     0     0     0     0     0     0     0     0
                    byte[] xisfSignature = new byte[XisfConstants.SignatureSize] { 0x58, 0x49, 0x53, 0x46, 0x30, 0x31, 0x30, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    // Set the new length of the new <xisf to /xisf> section
                    int xmlLength = xmlDoc.OuterXml.Length;

                    // Change Endianess
                    // Find the length of the <xisf>...</xisf> section
                    xisfSignature[8] = (byte)(xmlLength & 0xFF); // Least significant byte
                    xisfSignature[9] = (byte)((xmlLength >> 8) & 0xFF); // Most significant byte

                    // *******************************************************************************************************************************
                    // *******************************************************************************************************************************

                    // Create a Buffer List. The list will contain the XISF Signature and length, the xml, and the image data
                    // This ordered List sets up pointers to the data to by written, its type, and length

                    mBufferList.Clear();

                    // XISF Signature and length
                    mBuffer = new Buffer
                    {
                        Type = eBufferData.BINARY,
                        BinaryDataStart = 0,
                        BinaryByteLength = XisfConstants.SignatureSize,
                        BinaryData = xisfSignature
                    };
                    mBufferList.Add(mBuffer);

                    // Copy "<?xml" to  "/Xisf>"
                    mBuffer = new Buffer
                    {
                        Type = eBufferData.ASCII,
                        AsciiData = xmlDoc.OuterXml
                    };
                    mBufferList.Add(mBuffer);

                    // Main Image
                    // In OUTPUT file, pad from current position to the start of image data
                    mBuffer = new Buffer
                    {
                        Type = eBufferData.ZEROS,
                        BinaryByteLength = xFile.TargetAttachmentPadding
                    };
                    mBufferList.Add(mBuffer);

                    // Main Image data, written after the padding:
                    //  - compressed now  -> the freshly compressed (zlib+sh) bytes
                    //  - already compressed -> a verbatim copy of the source block from binaryFileData
                    if (compressNow)
                    {
                        mBuffer = new Buffer
                        {
                            Type = eBufferData.BINARY,
                            BinaryDataStart = 0,
                            BinaryByteLength = compressionResult.CompressedBytes.Length,
                            BinaryData = compressionResult.CompressedBytes
                        };
                    }
                    else
                    {
                        mBuffer = new Buffer
                        {
                            Type = eBufferData.BINARY,
                            BinaryDataStart = xFile.TargetAttachmentStart,
                            BinaryByteLength = xFile.TargetAttachmentLength,
                            BinaryData = binaryFileData
                        };
                    }
                    mBufferList.Add(mBuffer);

                    // Ignore any other attachments (e.g. Thumbnail, High Rejection, Low Rejection, etc,) in the input file

                    // *******************************************************************************************************************************
                    // *******************************************************************************************************************************

                    // Now that the mBuffer List is done, use it to write a new/updated XISF File
                    bool bStatus = await WriteBinaryFileAsync(destinationPath);
                    if (bStatus == false)
                    {
                        LastUpdateOutcome = eUpdateOutcome.Failed;
                        return false;
                    }

                    LastUpdateOutcome = compressNow ? eUpdateOutcome.Compressed : eUpdateOutcome.AlreadyCompressed;
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Update Write File Failed: {ex.Message}", xFile.FilePath, MessageBoxButtons.OK);
                LastUpdateOutcome = eUpdateOutcome.Failed;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error updating file: {ex.Message}", xFile.FilePath, MessageBoxButtons.OK);
                LastUpdateOutcome = eUpdateOutcome.Failed;
                return false;
            }

            return true;
        }

        // ##############################################################################################################################################
        // ##############################################################################################################################################

        /// <summary>
        /// Calculates the padding needed to align the given length to the specified alignment.
        /// </summary>
        /// <param name="length">The initial length that needs to be aligned.</param>
        /// <param name="alignment">The alignment boundary to align to.</param>
        /// <returns>The number of padding bytes needed to align the length to the specified alignment.</returns>
        private static int GetNewPadding(int length, int alignment)
        {
            // Check if the length is already aligned
            if (length % alignment == 0)
                return 0;

            // Calculate the next value that is evenly divisible by alignment
            int nextDivisible = length + (alignment - length % alignment);

            // Return the difference between the next divisible value and the current length
            return nextDivisible - length;
        }


        // ****************************************************************************************************
        // ****************************************************************************************************

        /// <summary>
        /// Removes unwanted attachments from the XML document.
        /// This method removes all <Image> elements with an id attribute not equal to 'integration' as well as
        /// <Thumbnail>, <ICCProfile>, and <DisplayFunction> elements.
        /// </summary>
        /// <param name="document">The XML document to modify.</param>
        private static void RemoveUnwantedAttachments(XmlDocument document)
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(document.NameTable);
            string namespaceUri = document.DocumentElement?.NamespaceURI ?? string.Empty;
            nsManager.AddNamespace("ns", namespaceUri);

            // Remove <Thumbnail> element if it exists
            var thumbnailNode = document.SelectSingleNode("//ns:Thumbnail", nsManager);
            thumbnailNode?.ParentNode?.RemoveChild(thumbnailNode);

            // Remove <ICCProfile> element if it exists
            var iccProfileNode = document.SelectSingleNode("//ns:ICCProfile", nsManager);
            iccProfileNode?.ParentNode?.RemoveChild(iccProfileNode);

            // Remove <DisplayFunction> element if it exists
            var displayFunctionNode = document.SelectSingleNode("//ns:DisplayFunction", nsManager);
            displayFunctionNode?.ParentNode?.RemoveChild(displayFunctionNode);
        }


        // ****************************************************************************************************
        // ****************************************************************************************************

        /// <summary>
        /// Finds the starting location of existing image data in the binary file data.
        /// It identifies the end of the XML section, then skips any padding (runs of 0's) after the </xisf> tag,
        /// returning the location of the first non-zero byte after the </xisf> tag.
        /// </summary>
        /// <param name="binaryFileData">The binary file data to search.</param>
        /// <returns>The location of the first non-zero byte after the </xisf> tag.</returns>
        public static int FindExistingImageStartingLocation(byte[] binaryFileData)
        {
            // Find the end of the </xisf> tag in the binary file data
            int xmlEndLocation = BinaryFind(binaryFileData, "</xisf>") + "</xisf>".Length;

            // Walk through any padding (runs of 0's) after the </xisf> tag
            int padding = 0;
            for (int i = xmlEndLocation; i < binaryFileData.Length; i++)
            {
                if (binaryFileData[i] == 0)
                {
                    padding++;
                }
                else
                {
                    break;
                }
            }

            // Return the location of the first non-zero byte after the </xisf> tag
            return xmlEndLocation + padding;
        }


        // ****************************************************************************************************
        // ****************************************************************************************************

        /// <summary>
        /// Sets the image attachment location in the XML document for an XISF file.
        /// This method updates the location attribute of the first Image node and calculates the necessary padding.
        /// </summary>
        /// <param name="document">The XML document to modify.</param>
        /// <param name="xFile">The XISF file containing block alignment size.</param>
        /// <param name="newImageSize">When set, also update the location size field (used after compression
        /// changes the stored block size); when null the existing size is preserved.</param>
        /// <returns>The calculated padding required for the new starting address.</returns>
        private static int SetImageAttachmentLocation(XmlDocument document, XisfFile xFile, int? newImageSize = null)
        {
            int documentLengthBeforeNewStartingAddress;
            int documentLengthAfterNewStartingAddress;
            int newPadding = -1;
            int newStartingAddress;

            XmlNamespaceManager nsManager = new XmlNamespaceManager(document.NameTable);
            string namespaceUri = document.DocumentElement?.NamespaceURI ?? string.Empty;
            nsManager.AddNamespace("ns", namespaceUri);

            // Find the first (but not only for masters) (ns + "Image") descendant
            XmlNode? imageNode = document.SelectSingleNode("//ns:Image", nsManager);

            if (imageNode == null)
                return -1;

            // Iterate until the XML document length does not change
            do
            {
                documentLengthBeforeNewStartingAddress = document.OuterXml.Length;

                // Include the XISF Signature. No comment section in the padding calculation
                newPadding = GetNewPadding(documentLengthBeforeNewStartingAddress + XisfConstants.SignatureSize, xFile.BlockAlignmentSize);

                // The starting address is the address in the new file (including XISF Signature); (not just in the XML document section)
                newStartingAddress = documentLengthBeforeNewStartingAddress + XisfConstants.SignatureSize + newPadding;

                // Update imageNode with the new starting address and can change the size of the XML document
                // 1. This can push the image data to a new location - new start address
                // 2. This can change the padding - new padding
                // If the document length changes, we need to iterate until it does not change (padding will change with each iteration)
                XmlAttribute? locationAttribute = imageNode?.Attributes?["location"];
                if (locationAttribute != null)
                {
                    string[] locationParts = locationAttribute.Value.Split(':');
                    locationParts[1] = newStartingAddress.ToString();
                    if (newImageSize.HasValue && locationParts.Length >= 3)
                        locationParts[2] = newImageSize.Value.ToString();
                    locationAttribute.Value = string.Join(":", locationParts);
                }

                documentLengthAfterNewStartingAddress = document.OuterXml.Length;
            }
            while (documentLengthAfterNewStartingAddress != documentLengthBeforeNewStartingAddress);

            return newPadding;
        }


        // ****************************************************************************************************
        // ****************************************************************************************************

        /// <summary>
        /// Sets the <c>compression</c> and <c>checksum</c> attributes on the main <Image> element so the written
        /// header describes the compressed block. Called before <see cref="SetImageAttachmentLocation"/> so the
        /// location size and padding converge against the final header length.
        /// </summary>
        private static void ApplyCompressionAttributes(XmlDocument document, BlockCompressionInfo info)
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(document.NameTable);
            string namespaceUri = document.DocumentElement?.NamespaceURI ?? string.Empty;
            nsManager.AddNamespace("ns", namespaceUri);

            XmlNode? imageNode = document.SelectSingleNode("//ns:Image", nsManager);
            if (imageNode?.Attributes == null)
                return;

            SetOrRemoveAttribute(document, imageNode, "compression", info.ToCompressionAttribute());
            SetOrRemoveAttribute(document, imageNode, "checksum", info.ToChecksumAttribute());
        }

        /// <summary>Sets a node attribute to the given value, or removes it when the value is null.</summary>
        private static void SetOrRemoveAttribute(XmlDocument document, XmlNode node, string name, string? value)
        {
            if (node.Attributes == null)
                return;

            if (value == null)
            {
                node.Attributes.RemoveNamedItem(name);
                return;
            }

            XmlAttribute attribute = node.Attributes[name] ?? node.Attributes.Append(document.CreateAttribute(name));
            attribute.Value = value;
        }


        // ****************************************************************************************************
        // ****************************************************************************************************

        /// <summary>
        /// Replaces all FITS keywords in the given XML document with the keywords from the XISF file.
        /// The method removes existing FITSKeyword elements and adds new ones from the XISF file's keyword list.
        /// </summary>
        /// <param name="document">The XML document to modify.</param>
        /// <param name="xFile">The XISF file containing the new keywords.</param>
        private static void ReplaceAllFitsKeywords(XmlDocument document, XisfFile xFile)
        {
            // Remove all existing FITSKeyword elements
            var nodeList = document.GetElementsByTagName("FITSKeyword").Cast<XmlNode>().ToList();
            nodeList.ForEach(node => node.ParentNode?.RemoveChild(node));

            // Find all <Image> elements in the document
            var imageNodes = document.GetElementsByTagName("Image").Cast<XmlNode>().ToList();

            // Alphabetize the keyword list from the XISF file
            var sortedKeywords = xFile.KeywordList.mKeywordList.OrderBy(p => p.Name).ToList();

            // Add each keyword as a new FITSKeyword element under each <Image> node
            imageNodes.ForEach(imageNode =>
            {
                sortedKeywords.ForEach(keyword =>
                {
                    var newElement = document.CreateElement("FITSKeyword", document.DocumentElement?.NamespaceURI);
                    newElement.SetAttribute("name", keyword.Name);
                    newElement.SetAttribute("comment", keyword.Comment);
                    newElement.SetAttribute("value", keyword.Value.ToString());
                    imageNode.AppendChild(newElement);
                });
            });
        }


        // ****************************************************************************************************
        // ****************************************************************************************************

        /// <summary>
        /// Searches for the specified text within the given byte array and returns the index of its first occurrence.
        /// </summary>
        /// <param name="buffer">The byte array to search.</param>
        /// <param name="findText">The text to find within the byte array.</param>
        /// <returns>The index of the first occurrence of the specified text within the byte array, or -1 if not found.</returns>
        private static int BinaryFind(byte[] buffer, string findText)
        {
            // Convert the search text to a byte array
            byte[] searchBytes = Encoding.UTF8.GetBytes(findText);
            int bufferLength = buffer.Length;
            int searchLength = searchBytes.Length;

            // Loop through the buffer to search for the text
            for (int i = 0; i <= bufferLength - searchLength; i++)
            {
                // Check if the first byte matches
                if (buffer[i] == searchBytes[0])
                {
                    int j;
                    // Check subsequent bytes
                    for (j = 1; j < searchLength && buffer[i + j] == searchBytes[j]; j++) ;
                    // If all bytes match, return the starting index
                    if (j == searchLength)
                        return i;
                }
            }

            // Return -1 if the text is not found
            return -1;
        }


        // ****************************************************************************************************
        // ****************************************************************************************************

        /// <summary>
        /// Checks if the FITS keywords in the XISF file match the keywords in the XML string.
        /// The method compares the keywords in the XISF file with the FITSKeyword elements in the XML document.
        /// </summary>
        /// <param name="xFile">The XISF file containing the keywords and XML string.</param>
        /// <returns>True if the keywords match the XML FITSKeyword elements; otherwise, false.</returns>
        private static bool KeywordsMatchXml(XisfFile xFile)
        {
            // Load the XML string into an XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xFile.XmlString);

            // Create an XmlNamespaceManager and add the necessary namespace
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            namespaceManager.AddNamespace("ns", "http://www.pixinsight.com/xisf");

            // Select all FITSKeyword elements using the namespace
            var fitsKeywordNodes = (xmlDoc.SelectNodes("//ns:FITSKeyword", namespaceManager) ?? throw new InvalidOperationException("No FITSKeyword nodes found")).Cast<XmlNode>().ToList();

            // Alphabetize the KeywordList from the XISF file
            var keywords = xFile.KeywordList.mKeywordList.OrderBy(p => p.Name).ToList();

            // Check if fitsKeywordNodes and keywords have identical elements in the same order
            bool bIdentical = keywords.Count == fitsKeywordNodes.Count &&
                              !keywords.Where((keyword, index) =>
                              {
                                  var fitsKeywordNode = fitsKeywordNodes[index];
                                  var nameAttribute = fitsKeywordNode.Attributes?["name"]?.Value;
                                  var valueAttribute = fitsKeywordNode.Attributes?["value"]?.Value;
                                  var commentAttribute = fitsKeywordNode.Attributes?["comment"]?.Value;

                                  return nameAttribute != keyword.Name || valueAttribute != keyword.Value || commentAttribute != keyword.Comment;
                              }).Any();

            return bIdentical;
        }


        // ##############################################################################################################################################
        // ##############################################################################################################################################

        /// <summary>
        /// Writes the binary data from the buffer list to a specified file.
        /// Handles different buffer types and ensures data is written correctly.
        /// </summary>
        /// <param name="fileName">The name of the file to write to.</param>
        /// <returns>True if the file is written successfully; otherwise, false.</returns>
        private async Task<bool> WriteBinaryFileAsync(string fileName)
        {
            byte[] zero = { 0x00 };
            string tempFile = fileName + ".xfmtmp";

            try
            {
                using (MemoryStream rawStream = new MemoryStream())
                using (BinaryWriter binaryWriter = new BinaryWriter(rawStream))
                {
                    // Write each buffer to the memory stream
                    foreach (var buffer in mBufferList)
                    {
                        long position = rawStream.Position;

                        switch (buffer.Type)
                        {
                            case eBufferData.ASCII:
                                byte[] asciiData = Encoding.UTF8.GetBytes(buffer.AsciiData);
                                binaryWriter.Write(asciiData, 0, asciiData.Length);
                                break;

                            case eBufferData.BINARY:
                                binaryWriter.Write(buffer.BinaryData, buffer.BinaryDataStart, buffer.BinaryByteLength);
                                break;

                            case eBufferData.ZEROS:
                                Enumerable.Range((int)position, buffer.BinaryByteLength)
                                          .ToList()
                                          .ForEach(_ => binaryWriter.Write(zero, 0, 1));
                                break;

                            case eBufferData.POSITION:
                                if ((int)position > buffer.ToPosition)
                                {
                                    string title = "WriteBinaryFile(string fileName) POSITION Error";
                                    string message = "\n\nThe length of xml xisfString is after the start of image data:\n\n" +
                                                     fileName + "\n\n" +
                                                     "Current Write Position:          " + position.ToString() + "\n" +
                                                     "Image Attachment Start Position: " + buffer.ToPosition.ToString() + "\n\nAborting.";

                                    MessageBox.Show(message, title, MessageBoxButtons.OK);
                                    throw new InvalidOperationException("POSITION Error");
                                }

                                Enumerable.Range((int)position, (int)(buffer.ToPosition - position))
                                          .ToList()
                                          .ForEach(_ => binaryWriter.Write(zero, 0, 1));
                                break;
                        }
                    }

                    // Write to a temp file first, then atomically move it into place. The image data is now
                    // transformed (compressed) rather than copied verbatim, so a mid-write failure must not
                    // corrupt the destination (which is the source file itself for in-place keyword updates).
                    byte[] binaryData = rawStream.ToArray();
                    using (FileStream fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                    {
                        await fileStream.WriteAsync(binaryData.AsMemory());
                        await fileStream.FlushAsync();
                    }

                    File.Move(tempFile, fileName, overwrite: true);
                }
                return true;
            }
            catch (Exception ex)
            {
                try { if (File.Exists(tempFile)) File.Delete(tempFile); } catch { /* best-effort cleanup */ }

                string title = "WriteBinaryFile() XisfFileUpdate.cs Failed";
                string message = "\n\n" + fileName + "\n\n" + ex.Message;
                MessageBox.Show(message, title, MessageBoxButtons.OK);
                return false;
            }
        }


    }
}
