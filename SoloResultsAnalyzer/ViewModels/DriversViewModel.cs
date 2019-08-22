using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoloResultsAnalyzer.ViewModels
{
    class DriversViewModel : ViewModelBase
    {
        private DbConnection _dbConnection;

        public ObservableCollection<Models.Driver> Drivers { get; } = new ObservableCollection<Models.Driver>();

        public Models.Driver SelectedDriver { get; set; } = new Models.Driver();

        public DriversViewModel(string pageTitle, DbConnection dbConnection) : base(pageTitle)
        {
            _dbConnection = dbConnection;
        }

        public ICommand Save
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    SaveDrivers();

                    // TODO indicate save went OK
                });
            }
        }

        public override void Update()
        {
            GetDrivers();
        }

        private void GetDrivers()
        {
            _dbConnection.Open();

            using (DbCommand command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Drivers";  // TODO add season ID

                var reader = command.ExecuteReader();

                Drivers.Clear();

                while (reader.Read())
                {
                    Models.Driver driver = new Models.Driver();
                    driver.FirstName = (string)reader["FirstName"];
                    driver.LastName = (string)reader["LastName"];
                    driver.IsLadies = (bool)reader["IsLadies"];
                    driver.IsNovice = (bool)reader["IsNovice"];
                    driver.Id = (int)reader["Id"];
                    driver.SeasonId = (int)reader["SeasonId"];
                    driver.DriverExists = true;

                    Drivers.Add(driver);
                }
            }

            _dbConnection.Close();
        }

        private void SaveDrivers()
        {
            _dbConnection.Open();

            DbCommand command = _dbConnection.CreateCommand();

            foreach (Models.Driver driver in Drivers)
            {
                command.CommandText = "UPDATE Drivers SET FirstName = @FirstName, LastName = @LastName, IsLadies = @IsLadies, IsNovice = @IsNovice WHERE Id = @Id";

                command.Parameters.Clear();

                Utilities.Extensions.AddParamWithValue(ref command, "FirstName", driver.FirstName);
                Utilities.Extensions.AddParamWithValue(ref command, "LastName", driver.LastName);
                Utilities.Extensions.AddParamWithValue(ref command, "IsLadies", driver.IsLadies);
                Utilities.Extensions.AddParamWithValue(ref command, "IsNovice", driver.IsNovice);
                Utilities.Extensions.AddParamWithValue(ref command, "Id", driver.Id);

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
