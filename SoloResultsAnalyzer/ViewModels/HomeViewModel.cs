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
        public HomeViewModel(string pageTitle) : base(pageTitle)
        {

        }

        /// <summary>
        /// Command for navigating to event data import view
        /// </summary>
        public ICommand Import
        {
            get
            {
                return SetNextViewModel("EventImportViewModel");
            }
        }

        /// <summary>
        /// Command for navigating to event report view
        /// </summary>
        public ICommand EventReport
        {
            get
            {
                return SetNextViewModel("EventReportViewModel");
            }
        }

        /// <summary>
        /// Command for navigating to championship report view
        /// </summary>
        public ICommand ChampionshipReport
        {
            get
            {
                return SetNextViewModel("ChampionshipReportViewModel");
            }
        }

        /// <summary>
        /// Command for navigating to new season view
        /// </summary>
        public ICommand NewSeason
        {
            get
            {
                return SetNextViewModel("NewSeasonViewModel");
            }
        }
    }
}
