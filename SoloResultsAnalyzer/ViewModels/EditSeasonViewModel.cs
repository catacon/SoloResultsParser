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
    class EditSeasonViewModel : ViewModelBase
    {
        public DataTable Events { get; set; }

        private Processors.EventAdapter _eventCreator;

        public EditSeasonViewModel(string pageTitle, Processors.EventAdapter eventCreator) : base(pageTitle)
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
