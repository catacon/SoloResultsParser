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

        public EventAdapter(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;

            InitializeAdapter();
        }

        private void InitializeAdapter()
        {
            var factory = DbProviderFactories.GetFactory(_dbConnection);
            _adapter = factory.CreateDataAdapter();
            var builder = factory.CreateCommandBuilder();
            builder.DataAdapter = _adapter;

            var selectCommand = factory.CreateCommand();
            selectCommand.CommandText = "SELECT * FROM Events";
            selectCommand.Connection = _dbConnection;

            _adapter.SelectCommand = selectCommand;
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.UpdateCommand = builder.GetUpdateCommand();
            _adapter.DeleteCommand = builder.GetDeleteCommand();
        }

        public DataTable GetEventDataTable()
        {
            DataTable table = new DataTable();
            _adapter.Fill(table);

            return table;
        }

        public void Update(DataTable table)
        {
            _adapter.Update(table);
        }
    }
}
