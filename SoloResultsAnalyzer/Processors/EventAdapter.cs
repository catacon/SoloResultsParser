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
    public class EventAdapter
    {
        private DbConnection _dbConnection;

        private DbDataAdapter _adapter;

        public EventAdapter(DbConnection dbConnection, int seasonId)
        {
            _dbConnection = dbConnection;

            InitializeAdapter(seasonId);
        }

        private void InitializeAdapter(int seasonId)
        {
            var factory = DbProviderFactories.GetFactory(_dbConnection);
            _adapter = factory.CreateDataAdapter();
            var builder = factory.CreateCommandBuilder();
            builder.DataAdapter = _adapter;

            var selectCommand = factory.CreateCommand();
            selectCommand.CommandText = "SELECT * FROM Events WHERE SeasonId = @SeasonId";
            selectCommand.Connection = _dbConnection;
            Utilities.Extensions.AddParamWithValue(ref selectCommand, "SeasonId", seasonId);

            _adapter.SelectCommand = selectCommand;
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.UpdateCommand = builder.GetUpdateCommand();
            _adapter.DeleteCommand = builder.GetDeleteCommand();
        }

        public DataTable GetEventDataTable()
        {
            DataTable table = new DataTable();
            _adapter.Fill(table);

            // TODO make default SeasonId match current season
            table.Columns["SeasonId"].DefaultValue = 1;

            table.Columns["Points"].DefaultValue = true;

            return table;
        }

        public List<Models.Event> GetEventList()
        {
            var list = GetEventDataTable().AsEnumerable().Select(m => new Models.Event()
            {
                Id = m.Field<int>("Id"),
                SeasonId = m.Field<int>("SeasonId"),
                EventNumber = m.Field<int>("EventNumber"),
                Date = m.Field<DateTime>("Date"),
                Location = m.Field<string>("Location")
            }).ToList();


            // TODO handle no events in season
            if (list.Count == 0)
            {
                list.Add(new Models.Event());
            }

            return list;
        }

        public void Update(DataTable table)
        {
            _adapter.Update(table);
        }
    }
}
