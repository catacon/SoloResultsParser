using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoloResultsAnalyzer.ViewModels
{
    class NewSeasonViewModel : ViewModelBase
    {
        public ObservableCollection<Models.Event> Events { get; } = new ObservableCollection<Models.Event>();

        private Processors.EventCreator _eventCreator;

        public NewSeasonViewModel(string pageTitle, Processors.EventCreator eventCreator) : base(pageTitle)
        {
            _eventCreator = eventCreator;
        }

        public override void Update()
        {
            var newEvents = _eventCreator.GetEvents();

            Events.Clear();

            foreach (Models.Event ev in newEvents)
            {
                Events.Add(ev);
            }
        }

        public ICommand Save
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    _eventCreator.SaveEvents(Events);
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
