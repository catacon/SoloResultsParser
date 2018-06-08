﻿using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer
{
    class ReportBuilder
    {
        // Column definitions for PAX report
        private static int PaxPositionColumn = 1;
        private static int PaxClassColumn = 2;
        private static int PaxNumberColumn = 3;
        private static int PaxDriverColumn = 4;
        private static int PaxCarColumn = 5;
        private static int PaxBestRunColumn = 6;
        private static int PaxTimeColumn = 7;
        private static int PaxDiffColumn = 8;
        private static int PaxFromFristColumn = 9;
        private static int PaxPointsColumn = 10;

        // Column definitions for raw report
        private static int RawPositionColumn = 1;
        private static int RawClassColumn = 2;
        private static int RawNumberColumn = 3;
        private static int RawDriverColumn = 4;
        private static int RawCarColumn = 5;
        private static int RawBestRunColumn = 6;
        private static int RawDiffColumn = 7;
        private static int RawFromFristColumn = 8;

        // Column definitions for class report
        private static int ClassPositionColumn = 1;
        private static int ClassClassColumn = 2;
        private static int ClassNumberColumn = 3;
        private static int ClassDriverColumn = 4;
        private static int ClassCarColumn = 5;
        private static int ClassTimesColumn = 6;
        private static int ClassBestRunColumn = 16;
        private static int ClassDiffColumn = 17;
        private static int ClassFromFristColumn = 18;
        private static int ClassPointsColumn = 19;
        private static int ClassMaxTimes = 10;

        public static bool GenerateEventPaxReport(int Season, int Event, string File, string Database)
        {
            // Store all results
            List<Result> Results = new List<Result>();

            // Get data from the database
            // Update class database
            try
            {
                using (SqlConnection db = new SqlConnection(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True", Database)))
                {
                    if (!GetEventResults(Season, Event, -1, false, false, db, ref Results))
                    {
                        Console.WriteLine("Failed to retrieve results.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            List<Result> SortedResults = Results.OrderBy(x => x.PaxTime).ToList();
  

            // Open Excel file and create new sheet for this event
            FileInfo info = new FileInfo(File);

            ExcelPackage package = new ExcelPackage(info);

            // TODO handle no template
            // TODO handle existing event

            ExcelWorksheet NewEventSheet = package.Workbook.Worksheets.Copy("Template", string.Format("event{0}", Event));

            NewEventSheet.Cells["A2"].Value = string.Format("{0} PAX Results - Event #{1} - {2}", Season, Event, 0); // TODO need event date

            // Populate the results
            for (int i = 0; i < SortedResults.Count; ++i)
            {
                NewEventSheet.Cells[5 + i, PaxPositionColumn].Value = i + 1;
                NewEventSheet.Cells[5 + i, PaxClassColumn].Value = SortedResults[i].ClassString;
                NewEventSheet.Cells[5 + i, PaxNumberColumn].Value = SortedResults[i].ClassNumber;
                NewEventSheet.Cells[5 + i, PaxDriverColumn].Value = string.Format("{0} {1}", SortedResults[i].FirstName, SortedResults[i].LastName);
                NewEventSheet.Cells[5 + i, PaxCarColumn].Value = SortedResults[i].Car;
                NewEventSheet.Cells[5 + i, PaxBestRunColumn].Value = SortedResults[i].RawTime;
                NewEventSheet.Cells[5 + i, PaxTimeColumn].Value = SortedResults[i].PaxTime;
                NewEventSheet.Cells[5 + i, PaxDiffColumn].Value = (i == 0 ? 0 : SortedResults[i].PaxTime - SortedResults[i - 1].PaxTime);
                NewEventSheet.Cells[5 + i, PaxFromFristColumn].Value = (i == 0 ? 0 : SortedResults[i].PaxTime - SortedResults[0].PaxTime);
                NewEventSheet.Cells[5 + i, PaxPointsColumn].Value = Math.Floor(10000 * SortedResults[0].PaxTime / SortedResults[i].PaxTime);  // TODO define points
            }

            package.Save();

            return true;
        }

        public static bool GenerateEventRawReport(int Season, int Event, string File, string Database)
        {
            // Store all results
            List<Result> Results = new List<Result>();

            // Get data from the database
            // Update class database
            try
            {
                using (SqlConnection db = new SqlConnection(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True", Database)))
                {
                    if (!GetEventResults(Season, Event, -1, false, false, db, ref Results))
                    {
                        Console.WriteLine("Failed to retrieve results.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            List<Result> SortedResults = Results.OrderBy(x => x.RawTime).ToList();


            // Open Excel file and create new sheet for this event
            FileInfo info = new FileInfo(File);

            ExcelPackage package = new ExcelPackage(info);

            // TODO handle no template
            // TODO handle existing event

            ExcelWorksheet NewEventSheet = package.Workbook.Worksheets.Copy("Template", string.Format("event{0}", Event));

            NewEventSheet.Cells["A2"].Value = string.Format("{0} Raw Time Results - Event #{1} - {2}", Season, Event, 0); // TODO need event date

            // Populate the results
            for (int i = 0; i < SortedResults.Count; ++i)
            {
                NewEventSheet.Cells[5 + i, RawPositionColumn].Value = i + 1;
                NewEventSheet.Cells[5 + i, RawClassColumn].Value = SortedResults[i].ClassString;
                NewEventSheet.Cells[5 + i, RawNumberColumn].Value = SortedResults[i].ClassNumber;
                NewEventSheet.Cells[5 + i, RawDriverColumn].Value = string.Format("{0} {1}", SortedResults[i].FirstName, SortedResults[i].LastName);
                NewEventSheet.Cells[5 + i, RawCarColumn].Value = SortedResults[i].Car;
                NewEventSheet.Cells[5 + i, RawBestRunColumn].Value = SortedResults[i].RawTime;
                NewEventSheet.Cells[5 + i, RawDiffColumn].Value = (i == 0 ? 0 : SortedResults[i].RawTime - SortedResults[i - 1].RawTime);
                NewEventSheet.Cells[5 + i, RawFromFristColumn].Value = (i == 0 ? 0 : SortedResults[i].RawTime - SortedResults[0].RawTime);
            }

            package.Save();

            return true;
        }

        public static bool GenerateEventClassReport(int Season, int Event, string File, string Database)
        {

            // Open Excel file and create new sheet for this event
            FileInfo info = new FileInfo(File);

            ExcelPackage package = new ExcelPackage(info);

            // TODO handle no template
            // TODO handle existing event

            ExcelWorksheet NewEventSheet = package.Workbook.Worksheets.Copy("Template", string.Format("event{0}", Event));

            NewEventSheet.Cells["A2"].Value = string.Format("{0} Class Results - Event #{1} - {2}", Season, Event, 0); // TODO need event date

            // Get class list
            int ClassCount = 0;

            try
            {
                string ClassCountQuery = "SELECT COUNT(*) FROM Classes";

                using (SqlConnection db = new SqlConnection(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True", Database)))
                {
                    db.Open();
                    using (SqlCommand cmdCount = new SqlCommand(ClassCountQuery, db))
                    {
                        ClassCount = (int)cmdCount.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Unable to get class count: {1}", ex.ToString()));
            }

            // Count to keep track of spreadsheet rows
            int RowCounter = 5;

            for (int Class = 1; Class < ClassCount; ++Class)
            {
                for (int Ladies = 0; Ladies <= 1; ++Ladies)
                {
                    // Store all runs
                    List<Run> Runs = new List<Run>();

                    // Get data from the database
                    try
                    {
                        using (SqlConnection db = new SqlConnection(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True", Database)))
                        {
                            db.Open();

                            if (!GetEventRuns(Season, Event, Class, Ladies == 1, false, db, ref Runs))
                            {
                                Console.WriteLine("Failed to retrieve results.");
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    // No need to generate report for empty class
                    if (Runs.Count <= 0)
                    {
                        continue;
                    }

                    // Store all results
                    List<Result> Results = new List<Result>();

                    // Get data from the database
                    try
                    {
                        using (SqlConnection db = new SqlConnection(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True", Database)))
                        {
                            db.Open();
                            if (!GetEventResults(Season, Event, Class, Ladies == 1, false, db, ref Results))
                            {
                                Console.WriteLine("Failed to retrieve results.");
                                continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    // Now need to generate report for empty class
                    if (Results.Count <= 0)
                    {
                        continue;
                    }

                    List<Result> SortedResults = Results.OrderBy(x => x.RawTime).ToList();

                    // Get class abbvreviation and name
                    string ClassAbbreviation = "", ClassLongName = "";

                    using (SqlConnection db = new SqlConnection(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True", Database)))
                    {
                        db.Open();
                        GetClassString(Class, ref ClassAbbreviation, ref ClassLongName, db);
                    }

                    if (Ladies == 1)
                    {
                        ClassAbbreviation += "L";
                        ClassLongName += " Ladies";
                    }

                    // Write class heading
                    NewEventSheet.Cells[RowCounter, 1].Value = string.Format("{0} - {1}", ClassAbbreviation, ClassLongName);
                    NewEventSheet.Cells[RowCounter, 1].Style.Font.Bold = true;

                    RowCounter += 1;

                    // Populate the results
                    for (int i = 0; i < SortedResults.Count; ++i, ++RowCounter)
                    {
                        List<Run> DriverRuns = Runs.Where(x => x.ClassNumber == SortedResults[i].ClassNumber).OrderBy(x => x.RunNumber).ToList();

                        NewEventSheet.Cells[RowCounter, ClassPositionColumn].Value = i + 1;
                        NewEventSheet.Cells[RowCounter, ClassClassColumn].Value = SortedResults[i].ClassString;
                        NewEventSheet.Cells[RowCounter, ClassNumberColumn].Value = SortedResults[i].ClassNumber;
                        NewEventSheet.Cells[RowCounter, ClassDriverColumn].Value = string.Format("{0} {1}", SortedResults[i].FirstName, SortedResults[i].LastName);
                        NewEventSheet.Cells[RowCounter, ClassCarColumn].Value = SortedResults[i].Car;

                        for (int time = 0; time < ClassMaxTimes; ++time)
                        {
                            if (time >= DriverRuns.Count)
                            {
                                continue;
                            }

                            Run r = DriverRuns[time];

                            string RunValue = "";

                            if (r.Penalty == RunPenalty.DNF)
                            {
                                RunValue = "DNF";
                            }
                            else if (r.Penalty == RunPenalty.RRN)
                            {
                                RunValue = "RRN";
                            }
                            else
                            {
                                RunValue = string.Format("{0}{1}{2}", r.RawTime, (r.Cones > 0 ? "+" : ""), (r.Cones > 0 ? r.Cones.ToString() : ""));
                            }

                            NewEventSheet.Cells[RowCounter, ClassTimesColumn + time].Value = RunValue;
                        }


                        NewEventSheet.Cells[RowCounter, ClassBestRunColumn].Value = SortedResults[i].RawTime;
                        NewEventSheet.Cells[RowCounter, ClassDiffColumn].Value = (i == 0 ? 0 : SortedResults[i].RawTime - SortedResults[i - 1].RawTime);
                        NewEventSheet.Cells[RowCounter, ClassFromFristColumn].Value = (i == 0 ? 0 : SortedResults[i].RawTime - SortedResults[0].RawTime);
                        NewEventSheet.Cells[RowCounter, ClassPointsColumn].Value = Math.Floor(10000 * SortedResults[0].PaxTime / SortedResults[i].PaxTime);  // TODO define points

                        // TODO Points
                    }

                    package.Save();

                    // Add a blank row between columns;
                    ++RowCounter;
                }
            }



            return true;
        }

        public static bool GenerateEventLadiesReport(int Season, int Event, string File, string Database)
        {
            return true;
        }

        public static bool GenerateEventNoviceReport(int Season, int Event, string File, string Database)
        {
            return true;
        }

        public static bool GenerateYtdPaxReport(int Season, int Event, string File, string Database)
        {
            return true;
        }

        public static bool GenerateYtdClassReport(int Season, int Event, string File, string Database)
        {
            return true;
        }

        public static bool GenerateYtdLadiesReport(int Season, int Event, string File, string Database)
        {
            return true;
        }

        public static bool GenerateYtdNoviceReport(int Season, int Event, string File, string Database)
        {
            return true;
        }

        private static bool GetEventResults(int Season, int Event, int Class, bool Ladies, bool Novice, SqlConnection Conn, ref List<Result> Results)
        {
            // Validate season input
            if (Season <= 0)
            {
                Console.WriteLine(string.Format("Invalid season: {0}", Season));
                return false;
            }

            // Validate event input
            if (Event <= 0)
            {
                Console.WriteLine(string.Format("Invalid event: {0}", Event));
                return false;
            }

            // Build the result query
            String ResultQuery = "";

            if (Class > 0)
            {
                ResultQuery = string.Format("SELECT * FROM Results WHERE Season = '{0}' AND Event = '{1}' AND Class = '{2}' AND Ladies = '{3}' AND Novice = '{4}'", Season, Event, Class, (Ladies ? 1 : 0), (Novice ? 1 : 0));
            }
            else
            {
                ResultQuery = string.Format("SELECT * FROM Results WHERE Season = '{0}' AND Event = '{1}' AND Ladies = '{2}' AND Novice = '{3}'", Season, Event, (Ladies ? 1 : 0), (Novice ? 1 : 0));
            }

            using (SqlCommand command = new SqlCommand(ResultQuery, Conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Result NewResult = new Result();

                        NewResult.FirstName = reader["FirstName"].ToString();
                        NewResult.LastName = reader["LastName"].ToString();
                        NewResult.RawTime = double.Parse(reader["RawTime"].ToString());
                        NewResult.PaxTime = double.Parse(reader["PaxTime"].ToString());
                        NewResult.Car = reader["Car"].ToString();
                        NewResult.ClassNumber = int.Parse(reader["Number"].ToString());
                        NewResult.ClassId = int.Parse(reader["Class"].ToString());

                        string LongName = "";
                        GetClassString(NewResult.ClassId, ref NewResult.ClassString, ref LongName, Conn);

                        Results.Add(NewResult);
                    }
                }
            }

            return true;
        }

        private static bool GetEventRuns(int Season, int Event, int Class, bool Ladies, bool Novice, SqlConnection Conn, ref List<Run> Runs)
        {
            // Validate season input
            if (Season <= 0)
            {
                Console.WriteLine(string.Format("Invalid season: {0}", Season));
                return false;
            }

            // Validate event input
            if (Event <= 0)
            {
                Console.WriteLine(string.Format("Invalid event: {0}", Event));
                return false;
            }

            // Validate class input
            if (Class <= 0)
            {
                Console.WriteLine(string.Format("Invalid class: {0}", Class));
                return false;
            }

            // Build the run query
            String RunQuery = string.Format("SELECT * FROM Runs WHERE Season = '{0}' AND Event = '{1}' AND Class = '{2}' AND Ladies = '{3}' AND Novice = '{4}'", Season, Event, Class, (Ladies ? 1: 0), (Novice ? 1 : 0));

            using (SqlCommand command = new SqlCommand(RunQuery, Conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Run NewRun = new Run();

                        NewRun.FirstName = reader["FirstName"].ToString();
                        NewRun.LastName = reader["LastName"].ToString();
                        NewRun.RawTime = double.Parse(reader["RawTime"].ToString());
                        NewRun.Car = reader["Car"].ToString();
                        NewRun.ClassNumber = int.Parse(reader["Number"].ToString());
                        NewRun.Cones = int.Parse(reader["Cones"].ToString());
                        NewRun.Penalty = (RunPenalty)int.Parse(reader["Penalty"].ToString());
                        NewRun.RunNumber = int.Parse(reader["RunNumber"].ToString());


                        if (NewRun.Penalty == RunPenalty.None)
                        {
                            NewRun.CorrectedTime = NewRun.RawTime + (2 * NewRun.Cones);     // TODO define cone penalty
                        }
                        else
                        {
                            NewRun.CorrectedTime = 999.99;
                        }

                        Runs.Add(NewRun);
                    }
                }
            }

            return true;
        }

        private static bool GetClassString(int Class, ref string Abbreviation, ref string LongName, SqlConnection Conn)
        {
            // Get the class string
            String ClassQuery = string.Format("SELECT Abbreviation, LongName FROM Classes WHERE Id = '{0}'", Class);

            using (SqlCommand ClassCommand = new SqlCommand(ClassQuery, Conn))
            {
                using (SqlDataReader reader = ClassCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        try
                        {
                            Abbreviation = reader["Abbreviation"].ToString().Trim();
                            LongName = reader["LongName"].ToString().Trim();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            Abbreviation = "ERR";
                            LongName = "ERR";
                            return false;
                        }
                    }
                    else
                    {
                        Abbreviation = "ERR";
                        LongName = "ERR";
                        return false;
                    }
                }
            }
        }
    }

}
