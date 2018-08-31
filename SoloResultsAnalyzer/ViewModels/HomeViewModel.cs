using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoloResultsAnalyzer.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {

        public HomeViewModel(string pageTitle, int seasonYear) : base(pageTitle)
        {
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

        public ICommand NewSeason
        {
            get
            {
                return SetNextViewModel("NewSeasonViewModel");
            }
        }
    }
}
