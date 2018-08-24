﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string _nextViewModel;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DelegateCommand SetNextViewModel(string nextViewModel)
        {
            return new DelegateCommand(o =>
            {
                _nextViewModel = nextViewModel;
                OnPropertyChanged("nextViewModel");
            });
        }
    }
}
