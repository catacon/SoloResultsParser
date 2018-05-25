using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer
{
    class ClassUpdater
    {
        public static bool Update(int Season, string ClassFile, string Database)
        {
            // Validate season input
            if (Season <= 0)
            {
                Console.WriteLine(string.Format("Invalid season: {0}", Season));
                return false;
            }

            // Attempt to open class list file
            if (!File.Exists(ClassFile))
            {
                Console.WriteLine(string.Format("Class List File {0} does not exist!", ClassFile));
                return false;
            }

            // Update class database
            try
            {
                using (SqlConnection db = new SqlConnection(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30", Database)))
                {
                    // Open class list file
                    TextFieldParser parser = new TextFieldParser(ClassFile);
                    parser.SetDelimiters(",");

                    bool ParseFailed = false;

                    // Open database
                    db.Open();

                    while (!parser.EndOfData && !ParseFailed)
                    {
                        // Read row - abbreviation,long name,multiplier
                        string[] Fields = parser.ReadFields();

                        Decimal Multiplier = 0;

                        if (!Decimal.TryParse(Fields[2], out Multiplier))
                        {
                            Console.WriteLine("Failed to parse multiplier.");
                            ParseFailed = true;
                            continue;
                        }

                        String query = "INSERT INTO Classes (Season,Abbreviation,LongName,Multiplier) VALUES (@Season,@Abbreviation,@LongName,@Multiplier)";

                        using (SqlCommand command = new SqlCommand(query, db))
                        {
                            command.Parameters.AddWithValue("@Season", Season);
                            command.Parameters.AddWithValue("@Abbreviation", Fields[0]);
                            command.Parameters.AddWithValue("@LongName", Fields[1]);
                            command.Parameters.AddWithValue("@Multiplier", Multiplier);

                            // Execute insert command
                            int result = command.ExecuteNonQuery();

                            // Check Error
                            if (result < 0)
                            {
                                Console.WriteLine("Error inserting data into Database!");
                                ParseFailed = true;
                            }
                        }
                    }

                    // Close database
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed to update database {0}", Database));
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }
    }
}
