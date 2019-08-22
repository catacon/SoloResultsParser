using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;

namespace SoloResultsAnalyzer.ViewModels
{
    class EventReportViewModel : ViewModelBase
    {
        public List<Models.Event> Events { get; private set; } = new List<Models.Event>();
        public Models.Event _selectedEvent = new Models.Event();
        private Processors.ReportGenerator _reportGenerator;
        private Processors.EventAdapter _eventAdapter;

        public EventReportViewModel(string pageTitle, Processors.EventAdapter adapter, Processors.ReportGenerator reportGenerator) : base(pageTitle)
        {
            _eventAdapter = adapter;
            Events = adapter.GetEventList();
            _selectedEvent = Events.First();
            _reportGenerator = reportGenerator;
        }

        public Models.Event SelectedEvent
        {
            get
            {
                return _selectedEvent;
            }

            set
            {
                _selectedEvent = value;
                OnPropertyChanged("SelectedEvent");
            }
        }

        public ICommand GenerateReport
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    _reportGenerator.GenerateEventReport(_selectedEvent.Id);
                });
            }
        }

        public override void Update()
        {
            Events = _eventAdapter.GetEventList();
            _selectedEvent = Events.First();
        }
    }
}
