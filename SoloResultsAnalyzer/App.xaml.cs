using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SoloResultsAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Capture unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = args.ExceptionObject as Exception;
                var message = exception.Message;
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    message += Environment.NewLine + Environment.NewLine + exception.Message;
                }

                MessageBox.Show(message, "Exception");
            };

            base.OnStartup(e);

            // Setup log file
            AppLog = NLog.LogManager.GetLogger(GetType().Name);

            // Load setup file
            if (!AppSettings.LoadFromFile(Settings.SettingsPath))
            {
                MessageBox.Show("Unabled to load settings file.");
            }
        }

        public Settings AppSettings = new Settings();
        public NLog.Logger AppLog;
    }
}
