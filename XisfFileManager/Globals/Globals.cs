using System.Collections.Generic;

namespace XisfFileManager.Globals
{
    // These are the defineds for image frame types
    public enum eFrame { ALL, LIGHT, DARK, FLAT, BIAS, EMPTY }

    // Each image has to be associated with a single Filter
    public enum eFilter { ALL, L, R, G, B, H, O, S, SHUTTER, EMPTY }

    // These specify how frame numbering will occur during renumbering of the files on disk
    public enum eOrder { WEIGHTINDEX, INDEXWEIGHT, WEIGHT, INDEX }

    // These control the display and clearing of the MessageBox for the results of finding and matching calibration files
    public enum eMessageMode { CLEAR, APPEND, NEW, KEEP }

    // In files streaming read and write operations, these define the type of data the buffer being read or written to will contain
    public enum eBufferData { ASCII, BINARY, ZEROS, USERDATA, POSITION }

    // Target Scheduler 
    public enum eProjectPriority { LOW = 0, NORMAL = 1, HIGH = 2 }

    // Main Form Keyword Upadate Mode
    public enum eKeywordUpdateMode { PROTECT, UPDATE_NEW, FORCE }

    // Result of a single file save (XisfFileUpdate.UpdateFileAsync) for status reporting.
    // Failed = write error; Protected = PROTECT mode, never written; Skipped = nothing to do
    // (UPDATE_NEW, keywords match, already compressed); Compressed = block was uncompressed and got
    // compressed; AlreadyCompressed = block copied verbatim.
    public enum eUpdateOutcome { Failed, Protected, Skipped, Compressed, AlreadyCompressed }

    public enum eUiState { DISABLED, ENABLED, RENAME }

    // This class contains static global values
    public static class GlobalValues
    {
        // List of available cameras
        public static readonly List<string> Cameras = new List<string> { "Z183", "Z533", "Q178", "A144" };
    }
}
