using MRzeszowiak.Model;
using MRzeszowiak.Services;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    class FavAdvertViewModel : BaseViewModel, INavigationAware
    {
        public ObservableCollection<AdvertShort> AdvertShortList { get; private set; } = new ObservableCollection<AdvertShort>();
        protected readonly IRzeszowiak _rzeszowiakRepository;
        protected readonly INavigationService _navigationService;
        protected readonly IPageDialogService _pageDialog;
        protected readonly ISetting _setting;
        protected readonly IDependencyService _dependencyService;
        private readonly IEventAggregator _eventAggregator;

        private bool activity = true;
        public bool Activity
        {
            get { return activity; }
            set
            {
                bool changed = activity == value ? false : true;
                activity = value;
                if (changed)
                {
                    OnPropertyChanged();
                }
            }
        }
        public bool FotterActivity => false;
        public ICommand LoadNextAdvert { get; private set; }
        public ICommand ListViewItemTapped { get; private set; }
        public ICommand DeleteAllButtonTapped { get; private set; }

        public FavAdvertViewModel(INavigationService navigationService, IPageDialogService pageDialog,
            IRzeszowiak RzeszowiakRepository, ISetting setting, IDependencyService dependencyService, IEventAggregator eventAggregator)
        {
            Debug.Write("FavAdvertViewModel Contructor");
            _setting = setting ?? throw new NullReferenceException("ISetting setting == null !");
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("ListViewModel => IRzeszowiakRepository RzeszowiakRepository == null !");
            _navigationService = navigationService ?? throw new NullReferenceException("INavigationService navigationService == null !");
            _pageDialog = pageDialog ?? throw new NullReferenceException("IPageDialogService pageDialog == null !");
            _dependencyService = dependencyService ?? throw new NullReferenceException("IDependencyService setting == null !");
            _eventAggregator = eventAggregator ?? throw new NullReferenceException("IEventAggregator eventAggregator == null !");

            ListViewItemTapped = new Command<AdvertShort>(ListViewTappedAsync);
            DeleteAllButtonTapped = new Command(DeleteAllAdverFromDb);
            _eventAggregator.GetEvent<AdvertDeleteFavEvent>().Subscribe(DeleteFromList);
            _eventAggregator.GetEvent<AdvertAddFavEvent>().Subscribe(AddToList);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Debug.Write("FavAdvertViewModel OnNavigatedTo");
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("LoadFavAdvert"))
            {
                Task dbLoading = new Task(() =>
                {
                    Activity = true;
                    AdvertShortList.Clear();
                    _setting.GetFavoriteAdvertListDB(AdvertShortList);
                    Activity = false;
                });
                dbLoading.Start();
            }                
        }

        protected async void ListViewTappedAsync(AdvertShort advertShort)
        {
            Debug.Write("ListViewTappedAsync");
            if (advertShort == null)
            {
                Debug.Write("ListViewTappedAsync => advertShort == null");
                return;
            }

            var navigationParams = new NavigationParameters
            {
                { "AdvertShortDB", advertShort } 
            };
            await _navigationService.NavigateAsync("PreviewPage", navigationParams, useModalNavigation: false, animated: false);
        }

        protected async void DeleteAllAdverFromDb()
        {
            if(await _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, 
                "Czy napewno chcesz usunąć wszystkie ulubione ogłoszenia?", "Usuń wszystko", "Anuluj"))
            {
                if (_setting.DeleteAdvertAllDB())
                {
                    _dependencyService.Get<IToast>().Show("Usunięto wszystkie ogłoszenia z bazy.");
                    AdvertShortList.Clear();
                }
                else
                    _dependencyService.Get<IToast>().Show("Wystąpił błąd podczas usuwania ogłoszeń z bazy.");
            }
        }

        protected void DeleteFromList(AdvertShort advertShort)
        {
            Debug.Write("DeleteFromList");
            if (advertShort != null)
                if (AdvertShortList.Count > 0)
                    AdvertShortList.Remove(advertShort);
        }

        protected void AddToList(AdvertShort advertShort)
        {
            Debug.Write("AddToList");
            if (advertShort != null)
                if(AdvertShortList.IndexOf(advertShort) == -1)
                    AdvertShortList.Insert(0, advertShort);                
        }

        public void Dispose()
        {
            _eventAggregator.GetEvent<AdvertDeleteFavEvent>().Unsubscribe(DeleteFromList);
            _eventAggregator.GetEvent<AdvertAddFavEvent>().Unsubscribe(AddToList);
        }
    }
}
