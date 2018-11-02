using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    public class PreViewImageViewModel : BaseViewModel, INavigationAware
    {
        protected readonly INavigationService _navigationService;
        public ObservableCollection<string> ImageURLsList { get; private set; } = new ObservableCollection<string>();

        private int position;
        public int Position
        {
            get { return position; }
            set
            {
                position = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackButtonTapped { get; private set; }

        public PreViewImageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new NullReferenceException("INavigationService navigationService == null !");
            BackButtonTapped = new Command(()=>_navigationService.GoBackAsync(null, useModalNavigation: false, animated: false));
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("ImageSelectedIndex") && parameters.ContainsKey("ImageList"))
            {
                if ((parameters["ImageSelectedIndex"] is int index) && 
                   (parameters["ImageList"] is IEnumerable<string> ImageList))
                        LoadImage(ImageList, index);    
            }
        }

        public void OnNavigatingTo(INavigationParameters parameters) { }
        public void OnNavigatedFrom(INavigationParameters parameters) { }

        void LoadImage(IEnumerable<string> imageList, int position)
        {
            ImageURLsList.Clear();
            foreach (var item in imageList)
                ImageURLsList.Add(item);

            if (position >= 0 && position < ImageURLsList.Count)
                Position = position;
            else Position = 0;
        }        
    }
}
