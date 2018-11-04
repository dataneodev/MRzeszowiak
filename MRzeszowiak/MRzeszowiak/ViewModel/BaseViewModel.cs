using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MRzeszowiak.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnNavigatedTo(INavigationParameters parameters) { }
        public virtual void OnNavigatingTo(INavigationParameters parameters) { }
        public virtual void OnNavigatedFrom(INavigationParameters parameters) { }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
