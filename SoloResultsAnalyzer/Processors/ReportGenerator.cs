using System.Data.Common;
using System.IO;
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SoloResultsAnalyzer.Processors
{
    public class ReportGenerator
    {
        private DbConnection _dbConnection;

        public ReportGenerator(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void GenerateEventReport(int eventId)
        {
            var rs = new LocalReporting()
                .RunInDirectory(Path.Combine(Directory.GetCurrentDirectory(), "jsreport"))
                .KillRunningJsReportProcesses()
                .UseBinary(JsReportBinary.GetBinary())
                .Configure(cfg => cfg.AllowedLocalFilesAccess().FileSystemStore().BaseUrlAsWorkingDirectory())
                .AsUtility()
                .Create();

            var r = new PaxResult { pos = 1, number = 25, class_name = "GS", car = "GTI", name = "Aaron Hall", best_run = 33.456, pax_time = 30.234, diff = 0, from_first = 0, points = 10000 };
           
            var p = new Pax { year = 2019, event_num = 5, date = "8/19/2018", drivers = new List<PaxResult>()};

            for (int i = 0; i < 200; ++i)
                p.drivers.Add(r);

            var json = JsonConvert.SerializeObject(p);

            var invoiceReport = rs.RenderByNameAsync("EventPaxReport", json).Result;

            invoiceReport.Content.CopyTo(File.OpenWrite("event.pdf"));
            invoiceReport.Content.Close();
            rs.KillAsync();
        }

        private void GetEventData(int eventId)
        {
            _dbConnection.Open();

            using (DbCommand command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Results WHERE ";

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Models.Driver driver = new Models.Driver();
                    driver.FirstName = (string)reader["FirstName"];
                    driver.LastName = (string)reader["LastName"];
                    driver.IsLadies = (bool)reader["IsLadies"];
                    driver.IsNovice = (bool)reader["IsNovice"];
                    driver.Id = (int)reader["Id"];
                    driver.DriverExists = true;
                }
            }

            _dbConnection.Close();
        }
    }

    public class Pax
    {
        public int year;
        public int event_num;
        public string date;
        public List<PaxResult> drivers;
    }

    public class PaxResult
    {
        public int pos;
        public int number;
        public string class_name;
        public string name;
        public string car;
        public double best_run;
        public double pax_time;
        public double diff;
        public double from_first;
        public int points;

    }
}
