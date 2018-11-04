using MRzeszowiak.Model;
using MRzeszowiak.Services;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace MRzeszowiak.ViewModel
{
    class FavAdvertViewModel : BaseViewModel
    {
        public ObservableCollection<AdvertShort> AdvertShortList { get; private set; } = new ObservableCollection<AdvertShort>();
        protected readonly IRzeszowiak _rzeszowiakRepository;
        protected readonly INavigationService _navigationService;
        protected readonly IPageDialogService _pageDialog;
        protected readonly ISetting _setting;

        public bool FotterActivity => false;
        public ICommand LoadNextAdvert { get; private set; }
        public ICommand ListViewItemTapped { get; private set; }

        public FavAdvertViewModel(INavigationService navigationService, IPageDialogService pageDialog,
            IRzeszowiak RzeszowiakRepository, ISetting setting)
        {
            Debug.Write("FavAdvertViewModel Contructor");
            _setting = setting ?? throw new NullReferenceException("ISetting setting == null !");
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("ListViewModel => IRzeszowiakRepository RzeszowiakRepository == null !");
            _navigationService = navigationService ?? throw new NullReferenceException("INavigationService navigationService == null !");
            _pageDialog = pageDialog ?? throw new NullReferenceException("IPageDialogService pageDialog == null !");

            //LoadNextAdvert = new Command(LoadNextItem);
            //ListViewItemTapped = new Command<AdvertShort>(ListViewTappedAsync);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            AdvertShortList.Clear();
            var favList = _setting.GetFavoriteAdvertListDB();
            if (favList != null)
                foreach (var item in favList)
                    AdvertShortList.Add(item);
        }
    }
}
