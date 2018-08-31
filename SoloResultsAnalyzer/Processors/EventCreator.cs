using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer.Processors
{
    public class EventCreator
    {
        private DbConnection _dbConnection;

        private SqlDataAdapter _adapter = new SqlDataAdapter();

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

        public void GetEventDataTable()
        {
            _dbConnection.Open();

            using (DbCommand command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Events";

                var factory = DbProviderFactories.GetFactory(_dbConnection);
                var adapter = factory.CreateDataAdapter();

                //adapter.

                //while (reader.Read())
                //{
                //    Models.Event driver = new Models.Event();
                //    driver.EventNumber = (int)reader["EventNumber"];
                //    driver.Location = (string)reader["Location"];
                //    driver.Date = (DateTime)reader["Date"];
                //    driver.Id = (int)reader["Id"];

                //    events.Add(driver);
                //}
            }

            

            _dbConnection.Close();
        }

        public void SaveEvents(IEnumerable<Models.Event> events)
        {
            _dbConnection.Open();

            DbCommand command = _dbConnection.CreateCommand();

            foreach (Models.Event ev in events)
            {
                command.CommandText = "UPDATE Events SET EventNumber = @EventNumber, Date = @Date, Location = @Location, Id = @Id";

                Utilities.Extensions.AddParamWithValue(ref command, "EventNumber", ev.EventNumber);
                Utilities.Extensions.AddParamWithValue(ref command, "Date", ev.Date);
                Utilities.Extensions.AddParamWithValue(ref command, "Location", ev.Location);
                Utilities.Extensions.AddParamWithValue(ref command, "Id", ev.Id);

                int result = command.ExecuteNonQuery();

                if (result < 0)
                {
                    Console.WriteLine("Error inserting result data into Database!");
                }

            }

            _dbConnection.Close();
        }
    }
}
