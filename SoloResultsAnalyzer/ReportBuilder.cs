using OfficeOpenXml;
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
        public static bool GenerateEventPaxReport(int Season, int Event, string File, string Database)
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

            // Store all results
            List<Result> Results = new List<Result>();

            // Get data from the database
            // Update class database
            try
            {
                using (SqlConnection db = new SqlConnection(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True", Database)))
                {
                    // Open the database
                    db.Open();

                    // Build the PAX result query
                    String PaxResultQuery = string.Format("SELECT * FROM Results WHERE Season = '{0}' AND Event = '{1}'", Season, Event);

                    using (SqlCommand command = new SqlCommand(PaxResultQuery, db))
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

                                // Get the class string
                                String ClassQuery = string.Format("SELECT Abbreviation FROM Classes WHERE Id = '{0}'", reader["Class"]);

                                using (SqlCommand ClassCommand = new SqlCommand(ClassQuery, db))
                                {
                                    try
                                    {
                                        NewResult.ClassString = (string)ClassCommand.ExecuteScalar();
                                    }
                                    catch (Exception ex)
                                    {
                                        // TODO bail out
                                        Console.WriteLine("Unable to get class");
                                        Console.WriteLine(ex.Message);
                                        NewResult.ClassString = "ERR";
                                    }
                                }

                                Results.Add(NewResult);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var SortedResults = Results.OrderBy(x => x.PaxTime).ToList();

            // Open Excel file and create new sheet for this event
            FileInfo info = new FileInfo(File);

            ExcelPackage package = new ExcelPackage(info);

            // TODO handle no template
            // TODO handle existing event

            ExcelWorksheet NewEventSheet = package.Workbook.Worksheets.Copy("Template", string.Format("event{0}", Event));

            NewEventSheet.Cells["A2"].Value = string.Format("{0} PAX Results - Event {1} - {2}", Season, Event, 0); // TODO need event date

            // TODO define columns

            // Populate the results
            for (int i = 0; i < SortedResults.Count; ++i)
            {
                NewEventSheet.Cells[5 + i, 1].Value = i + 1;
                NewEventSheet.Cells[5 + i, 2].Value = SortedResults[i].ClassString;
                NewEventSheet.Cells[5 + i, 3].Value = SortedResults[i].ClassNumber;
                NewEventSheet.Cells[5 + i, 4].Value = string.Format("{0} {1}", SortedResults[i].FirstName, SortedResults[i].LastName);
                NewEventSheet.Cells[5 + i, 5].Value = SortedResults[i].Car;
                NewEventSheet.Cells[5 + i, 6].Value = SortedResults[i].RawTime;
                NewEventSheet.Cells[5 + i, 7].Value = SortedResults[i].PaxTime;
                NewEventSheet.Cells[5 + i, 8].Value = (i == 0 ? 0 : SortedResults[i].PaxTime - SortedResults[i - 1].PaxTime);
                NewEventSheet.Cells[5 + i, 9].Value = (i == 0 ? 0 : SortedResults[i].PaxTime - SortedResults[0].PaxTime);
                NewEventSheet.Cells[5 + i, 10].Value = Math.Floor(10000 * SortedResults[0].PaxTime / SortedResults[i].PaxTime);  // TODO define points
            }

            package.Save();

            return true;
        }

        public static bool GenerateEventRawReport(int Season, int Event, string File)
        {
            return true;
        }

        public static bool GenerateEventClassReport(int Season, int Event, string File)
        {
            return true;
        }

        public static bool GenerateEventLadiesReport(int Season, int Event, string File)
        {
            return true;
        }

        public static bool GenerateEventNoviceReport(int Season, int Event, string File)
        {
            return true;
        }

        public static bool GenerateYtdPaxReport(int Season, int Event, string File)
        {
            return true;
        }

        public static bool GenerateYtdClassReport(int Season, int Event, string File)
        {
            return true;
        }

        public static bool GenerateYtdLadiesReport(int Season, int Event, string File)
        {
            return true;
        }

        public static bool GenerateYtdNoviceReport(int Season, int Event, string File)
        {
            return true;
        }
    }
}
