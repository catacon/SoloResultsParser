using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoloResultsAnalyzer.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private Processors.EventAdapter _eventCreator;

        public DataTable Events { get; set; }

        public HomeViewModel(string pageTitle, int seasonYear, Processors.EventAdapter eventCreator) : base(pageTitle)
        {
            _eventCreator = eventCreator;
            SeasonYear = seasonYear;
        }

        public int SeasonYear { get; }

        public ICommand Import
        {
            get
            {
                return SetNextViewModel("EventImportViewModel");
            }
        }

        public ICommand EventReport
        {
            get
            {
                return SetNextViewModel("EventReportViewModel");
            }
        }

        public ICommand ChampionshipReport
        {
            get
            {
                return SetNextViewModel("ChampionshipReportViewModel");
            }
        }

        public ICommand ViewDrivers
        {
            get
            {
                return SetNextViewModel("DriversViewModel");
            }
        }

        public ICommand EditSeason
        {
            get
            {
                return SetNextViewModel("EditSeasonViewModel");
            }
        }

        public ICommand NewSeason
        {
            get
            {
                return SetNextViewModel("NewSeasonViewModel");
            }
        }

        public override void Update()
        {
            Events = _eventCreator.GetEventDataTable();
        }


    }
}
