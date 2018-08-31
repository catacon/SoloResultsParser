﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoloResultsAnalyzer.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private Processors.EventCreator _eventCreator;

        public ObservableCollection<Models.Event> Events { get; } = new ObservableCollection<Models.Event>();

        public HomeViewModel(string pageTitle, int seasonYear, Processors.EventCreator eventCreator) : base(pageTitle)
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

        public ICommand NewSeason
        {
            get
            {
                return SetNextViewModel("NewSeasonViewModel");
            }
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


    }
}
