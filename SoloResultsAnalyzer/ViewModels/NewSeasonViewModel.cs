using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoloResultsAnalyzer.ViewModels
{
    public class NewSeasonViewModel : ViewModelBase
    {
        DbConnection _dbConnection;
        Processors.EventAdapter _eventAdapter;

        public NewSeasonViewModel(string pageTitle, ref DbConnection dbConnection, Processors.EventAdapter eventAdapter) : base (pageTitle)
        {
            _dbConnection = dbConnection;
            _eventAdapter = eventAdapter;
        }

        public ICommand SelectDatabase
        {
            get
            {
                return new DelegateCommand(o =>
                {

                });
            }
        }

        public ICommand StartNewSeason
        {
            get
            {
                return new DelegateCommand(o =>
                {

                });
            }
        }
    }
}
