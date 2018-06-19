using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace SoloResultsAnalyzer
{
    [XmlRoot("SoloResultsAnalyzerSetup")]
    public class Settings
    {
        public string CurrentDatabase = String.Empty;
        public int CurrentSeason = 0;
        public int CurrentEvent = 0;
        public string LogPath = @"C:\ProgramData\STLSolo\analyzer.log";

        [XmlIgnore]
        public const string SettingsPath = @"C:\ProgramData\STLSolo\";

        [XmlIgnore]
        public const string SettingsFile = "settings.xml";

        /// <summary>
        /// Save settings file to selected path
        /// </summary>
        /// <param name="path">Path at which to save settings file</param>
        /// <returns>True if file was saved successfully, false otherwise</returns>
        public bool SaveToFile(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            XmlSerializer ser = new XmlSerializer(typeof(Settings));

            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    ser.Serialize(sw, this);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Load settings file from selected path
        /// </summary>
        /// <param name="path">Path at which settings file is saved</param>
        /// <returns>True if file was loaded successfully, false otherwise</returns>
        public bool LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            XmlSerializer ser = new XmlSerializer(typeof(Settings));

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    var settings = (Settings)ser.Deserialize(sr);

                    // Load all values
                    CurrentDatabase = settings.CurrentDatabase;
                    CurrentSeason = settings.CurrentSeason;
                    CurrentEvent = settings.CurrentEvent;
                    LogPath = settings.LogPath;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create settings file
        /// </summary>
        /// <returns>Retruns truce if file was created, false otherwise</returns>
        public bool CreateDefaultFile()
        {
            Directory.CreateDirectory(SettingsPath);

            var stream = File.Create(SettingsPath + SettingsFile);
            stream.Close();

            return SaveToFile(SettingsPath + SettingsFile);
        }
    }
}
