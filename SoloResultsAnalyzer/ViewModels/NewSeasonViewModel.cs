using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoloResultsAnalyzer.ViewModels
{
    class NewSeasonViewModel : ViewModelBase
    {
        public DataTable Events { get; set; }

        private Processors.EventCreator _eventCreator;

        public NewSeasonViewModel(string pageTitle, Processors.EventCreator eventCreator) : base(pageTitle)
        {
            _eventCreator = eventCreator;
        }

        public override void Update()
        {
            Events = _eventCreator.GetEventDataTable();
        }

        public ICommand Save
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    _eventCreator.Update(Events);
                });
            }
        }

        public ICommand NewSeason
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    // TODO
                });
            }
        }
    }
}
