using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Forms;
using Velopack;

[assembly: SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyIFormatProvider", Justification = "This code is for personal use and does not require culture-specific formatting.")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "This code is for personal use and does not require culture-specific formatting.")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1309:SpecifyIFormatProvider", Justification = "This code is for personal use and does not require culture-specific formatting.")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1310:SpecifyIFormatProvider", Justification = "This code is for personal use and does not require culture-specific formatting.")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1311:SpecifyIFormatProvider", Justification = "This code is for personal use and does not require culture-specific formatting.")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1707:SpecifyIFormatProvider", Justification = "This code is for personal use and does not require culture-specific formatting.")]


namespace XisfFileManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            VelopackApp.Build().Run();

            // Shared diagnostics log (Astronomy.Diagnostics): rotate xfm.log -> xfm.log.prev and start
            // fresh — each run's trail is self-contained, one run back kept for postmortem. Diag
            // channels default to all in Debug / off in Release; XFM_DIAG overrides at runtime.
#if DEBUG
            const Astronomy.Diagnostics.DiagDefault diag = Astronomy.Diagnostics.DiagDefault.All;
#else
            const Astronomy.Diagnostics.DiagDefault diag = Astronomy.Diagnostics.DiagDefault.None;
#endif
            Astronomy.Diagnostics.Log.Init(new Astronomy.Diagnostics.AppLogIdentity("XisfFileManager", "xfm.log", "XFM_DIAG", diag));
            Astronomy.Diagnostics.Log.StartNewSession();

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

