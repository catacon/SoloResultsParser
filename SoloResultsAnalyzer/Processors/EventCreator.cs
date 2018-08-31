using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer.Processors
{
    public class EventCreator
    {
        private DbConnection _dbConnection;

        public EventCreator(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public List<Models.Event> GetEvents()
        {
            _dbConnection.Open();

            List<Models.Event> events = new List<Models.Event>();

            using (DbCommand command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Events";

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Models.Event driver = new Models.Event();
                    driver.EventNumber = (int)reader["EventNumber"];
                    driver.Location = (string)reader["Location"];
                    driver.Date = (DateTime)reader["Date"];
                    driver.Id = (int)reader["Id"];

                    events.Add(driver);
                }
            }

            _dbConnection.Close();

            return events;
        }
    }
}
