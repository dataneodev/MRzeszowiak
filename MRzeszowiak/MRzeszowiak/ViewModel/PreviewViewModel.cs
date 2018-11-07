using MRzeszowiak.Model;
using MRzeszowiak.Extends;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;
using Prism.Events;
using MRzeszowiak.Interfaces;

namespace MRzeszowiak.ViewModel
{
    public class PreviewViewModel : BaseViewModel, INavigationAware
    {
        protected readonly IRzeszowiak _rzeszowiakRepository;
        protected readonly IRzeszowiakImageContainer _imageContainer;
        protected readonly INavigationService _navigationService;
        protected readonly IPageDialogService _pageDialog;
        protected readonly IDependencyService _dependencyService;
        protected readonly ISetting _setting;
        private readonly IEventAggregator _eventAggregator;

        private int adverIDinRzeszowiak;
        public int AdverIDinRzeszowiak
        {
            get { return adverIDinRzeszowiak; }
            set
            {
                adverIDinRzeszowiak = value;
                OnPropertyChanged();
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        private string dateAdd;
        public string DateAdd
        {
            get { return dateAdd; }
            set
            {
                dateAdd = value;
                OnPropertyChanged();
            }
        }

        private string expiredDateAdd;
        public string ExpiredDateAdd
        {
            get { return expiredDateAdd; }
            set
            {
                expiredDateAdd = value;
                OnPropertyChanged();
            }
        }

        private int views;
        public int Views
        {
            get { return views; }
            set
            {
                views = value;
                OnPropertyChanged();
            }
        }

        private int price;
        public int Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }

        private bool highlighted;
        public bool Highlighted
        {
            get { return highlighted; }
            set
            {
                highlighted = value;
                OnPropertyChanged();
            }
        }

        private string descriptionHTML;
        public string DescriptionHTML
        {
            get { return descriptionHTML; }
            set
            {
                descriptionHTML = value;
                OnPropertyChanged();
            }
        }

        public ImageSource PhoneImage
        {
            get { return _imageContainer?.ImageData; }
        }

        private bool isFavorite;
        public bool IsFavorite
        {
            get { return isFavorite; }
            set
            {
                isFavorite = value;
                OnPropertyChanged();
            }
        }

        private bool hasPhoneImage;
        public bool HasPhoneImage
        {
            get { return hasPhoneImage; }
            set
            {
                hasPhoneImage = value;
                OnPropertyChanged();
            }
        }

        private bool activity = true;
        public bool Activity
        {
            get { return activity; }
            set
            {
                activity = value;
                OnPropertyChanged();
                OnPropertyChanged("ViewActive");
            }
        }

        private string errorMessage = String.Empty;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged("ViewActive");
            }
        }

        public bool ViewActive { get => ErrorMessage?.Length == 0 && !Activity;  }
   
        private MailStatusEnum mailStatus = MailStatusEnum.email_default;
        public MailStatusEnum MailStatus
        {
            get { return mailStatus; }
            set
            {
                mailStatus = value;
                OnPropertyChanged();
            }
        }

        private DateTime _lastSendMailDate;
        public DateTime LastSendMailDate
        {
            get { return _lastSendMailDate; }
            set
            {
                _lastSendMailDate = value;
                OnPropertyChanged("LastSendMailDate");
            }
        }

        public ObservableCollection<KeyValue> AdditionalData { get; set; } = new ObservableCollection<KeyValue>();
        public ObservableCollection<string> ImageURLsList { get; set; } = new ObservableCollection<string>();
        public Action ScrollToButtom;

        private AdvertShort _lastAdvertShort;
        private Advert _lastAdvert;
        public ICommand OpenAdvertPage { get; private set; }
        public ICommand RefreshAdvert { get; private set; }
        public ICommand MailAdvert { get; private set; }
        public ICommand FavoriteAdvert { get; private set; }
        public ICommand ImageTapped { get; private set; }
        public ICommand BackButtonTapped { get; private set; }
        public ICommand MailSendButtonTapped { get; private set; }

        public PreviewViewModel(INavigationService navigationService, IRzeszowiak RzeszowiakRepository,
                                IRzeszowiakImageContainer rzeszowiakImageContainer, IPageDialogService pageDialog, 
                                ISetting setting, IDependencyService dependencyService, IEventAggregator eventAggregator)
        {
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("IRzeszowiakRepository RzeszowiakRepository == null !");
            _navigationService = navigationService ?? throw new NullReferenceException("INavigationService navigationService == null !");
            _pageDialog = pageDialog ?? throw new NullReferenceException("IPageDialogService pageDialog == null !");
            _imageContainer = rzeszowiakImageContainer ?? throw new NullReferenceException("IRzeszowiakImageContainer rzeszowiakImageContainer == null !");
            _setting = setting ?? throw new NullReferenceException("ISetting setting == null !");
            _dependencyService = dependencyService ?? throw new NullReferenceException("IDependencyService dependencyService == null !");
            _eventAggregator = eventAggregator ?? throw new NullReferenceException("IEventAggregator eventAggregator == null !");
            _imageContainer.OnDownloadFinish += ImageDownloadFinish;

            ImageURLsList.CollectionChanged += (s, e) => { OnPropertyChanged("ImageURLsList"); };
            AdditionalData.CollectionChanged += (s, e) => { OnPropertyChanged("AdditionalData"); };

            OpenAdvertPage = new Command(() =>
            {
                if (_lastAdvert?.URL?.Length > 0)
                {
                    _dependencyService.Get<IToast>().Show("Otwieram strone ogłoszenia");
                    Device.OpenUri(new Uri(_lastAdvert?.URL));
                }     
            });

            RefreshAdvert = new Command(() =>
            {
                if (_lastAdvertShort != null)
                {
                    _dependencyService.Get<IToast>().Show("Odświeżam ogłoszenie");
                    LoadAdvertMessage(_lastAdvertShort);
                }    
            });

            MailAdvert = new Command(async() =>
            {
                Debug.Write("MailAdvert");
                if (MailStatus == MailStatusEnum.email_creating)
                {
                    if(LastSendMailDate.IsSend())
                        MailStatus = MailStatusEnum.email_send;
                        else
                        MailStatus = MailStatusEnum.email_default;
                    return;
                }

                if (!_setting.IsUserMailCorrect)
                {
                    await _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Nie można wysłać wiadomości.\nUzupełnij w ustawieniach aplikacji swój adres email.", "OK");
                    return;
                }

                if (!CanSendMail())
                {
                    await _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Nie można narazie wysłać wiadomości.", "OK");
                    return;
                }

                if (_lastAdvert?.EmailToken.Length != 10)
                {
                    await _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Problem z wysłaniem wiadomości. Odśwież ogłoszenie.", "OK");
                    return;
                }

                MailStatus = MailStatusEnum.email_creating;
                ScrollToButtom?.Invoke();
            });

            FavoriteAdvert = new Command(async () =>
            {
                if (_lastAdvert == null) return;

                if (IsFavorite)
                {
                    if (await _setting.DeleteAdvertDBAsync(_lastAdvert))
                    {
                        _dependencyService.Get<IToast>().Show("Usunięto ogłoszenie z ulubionych");
                        _eventAggregator.GetEvent<AdvertDeleteFavEvent>().Publish(_lastAdvertShort);
                        IsFavorite = !IsFavorite;
                    }  
                    else
                        _dependencyService.Get<IToast>().Show("Wystąpił błąd podczas usuwania ogłoszenia z ulubionych");
                }
                else
                {
                    if (await _setting.InsertOrUpdateAdvertDBAsync(_lastAdvert))
                    {
                        _eventAggregator.GetEvent<AdvertAddFavEvent>().Publish(_lastAdvert);
                        _dependencyService.Get<IToast>().Show("Dodano ogłoszenie do ulubionych");
                        IsFavorite = !IsFavorite;
                    }
                    else
                        _dependencyService.Get<IToast>().Show("Wystąpił błąd podczas dodawania ogłoszenia do ulubionych");
                }
            });

            ImageTapped = new Command<int>(async (selecteIndex) =>
            {
                if (ImageURLsList.Count == 0) return;

                var navigationParams = new NavigationParameters();
                navigationParams.Add("ImageSelectedIndex", selecteIndex);
                navigationParams.Add("ImageList", ImageURLsList);
                await _navigationService.NavigateAsync("PreviewImagePage", navigationParams, useModalNavigation: false, animated: false);
            });

            BackButtonTapped = new Command(async () => 
            {
                await _navigationService.GoBackAsync(null,false,false);
            });


            MailSendButtonTapped = new Command<string>(async (message) =>
            {
                Debug.Write("MailSendButtonTapped");
                if (!_setting.IsUserMailCorrect)
                {
                    await _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Nie można wysłać wiadomości.\nUzupełnij w ustawieniach aplikacji swój adres email.", "OK");
                    return;
                }

                if((message?.Length??0) == 0)
                {
                    await _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Nie można wysłać pustej wiadomości. Weź coś napisz.", "OK");
                    return;
                }

                if (!CanSendMail())
                {
                    await _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Nie można narazie wysłać wiadomości.", "OK");
                    return;
                }

                if(_lastAdvert?.EmailToken.Length != 10)
                {
                    await _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Problem z wysłaniem wiadomości. Odśwież ogłoszenie.", "OK");
                    return;
                }

                MailStatus = MailStatusEnum.email_sending;

                var status = await _rzeszowiakRepository.SendUserMessage(_lastAdvert, message, _setting.UserEmail);
                if (status)
                {
                    await _setting.UpdateSendMailNoticeAsync(_lastAdvert);
                    LastSendMailDate = DateTime.Now;
                    MailStatus = MailStatusEnum.email_send;
                    await _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Twoja wiadomość została wysłana.", "OK");
                }
                else
                {
                    MailStatus = MailStatusEnum.email_default;
                    await _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Twoja wiadomość nie została wysłana. Wystąpił nieoczekiwany błąd.", "OK");
                }        
            });
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Debug.Write("OnNavigatedTo PreviewViewModel");
            if (parameters.ContainsKey("AdvertShort"))
                if (parameters["AdvertShort"] is AdvertShort advertShort)
                    LoadAdvertMessage(advertShort);

            if (parameters.ContainsKey("AdvertShortDB"))
                if (parameters["AdvertShortDB"] is AdvertShort advertShort)
                    LoadAdvertMessage(advertShort, true);
        }

        async void LoadAdvertMessage(AdvertShort advertShort, bool dbLoad = false)
        {
            if(advertShort == null)
                throw new NullReferenceException("LoadAdvertMessage => advertShort == null !");
            if (advertShort.AdverIDinRzeszowiak == 0)
                throw new ArgumentException("LoadAdvertMessage => advertShort.AdverIDinRzeszowiak == 0");

            _lastAdvertShort = advertShort;
            ErrorMessage = String.Empty;
            Activity = true;
            CopyAdverToViewModel(new Advert());

            Advert _advert;
            if (!dbLoad)
                _advert = await _rzeszowiakRepository.GetAdvertAsync(advertShort);
            else
                _advert = await _setting.GetFavoriteAdvertDBAsync(advertShort);

            if (_advert == null)
            {
                if (!dbLoad)
                    ErrorMessage = "Błąd podczas ładowania strony.\nSprawdź połączenie internetowe i spróbuj ponownie.";
                else
                    ErrorMessage = "Błąd podczas wczytywania ogłoszenia z bazy danych.\nSkontaktuj się z deweloperem aplikacji.";
                MailStatus = MailStatusEnum.email_default;
            }
            else
            {
                CopyAdverToViewModel(_advert);
                LastSendMailDate = await _setting.LastMailSendDateAsync(_advert);
                IsFavorite = await _setting.IsAdvertInDBAsync(_advert);
                if(LastSendMailDate.IsSend())
                    MailStatus = MailStatusEnum.email_send;
                else
                    MailStatus = MailStatusEnum.email_default;
            }
            Activity = false;
        }

        void CopyAdverToViewModel(Advert advert)
        {
            AdverIDinRzeszowiak = advert?.AdverIDinRzeszowiak??0;
            Title = advert?.Title ?? String.Empty;
            Price = advert?.Price ?? 0;
            Views = advert?.Views ?? 0;
            DescriptionHTML = advert?.DescriptionHTML ?? String.Empty;
            DateAdd = advert?.DateAddString ?? String.Empty;
            ExpiredDateAdd = advert?.ExpiredString ?? String.Empty;
            Highlighted = advert?.Highlighted ?? false;
            
            AdditionalData.Clear();
            foreach (var item in advert?.AdditionalData)
                AdditionalData.Add(new KeyValue(item.Key, item.Value));

            ImageURLsList.Clear();
            foreach (var item in advert?.ImageURLsList)
                ImageURLsList.Add(item);                

            if ((advert.PhoneSsid.Length == 10 && advert.PhonePHPSSESION != null) || advert?.PhoneImageByteArray?.Length>0)
            {
                HasPhoneImage = true;
                _imageContainer.HideImage();
                _imageContainer.DownloadImage(advert, advert.PhoneSsid, advert.AdverIDinRzeszowiak, advert.URLPath, advert.PhonePHPSSESION); // no wait
            }
            else
                HasPhoneImage = false;
            _lastAdvert = advert;
        }

        protected bool CanSendMail()
        {
            //if (DateTime.Compare(lastSendMailDate, DateTime.Now.AddHours(-1)) > 0)
            //    return false;
            return true;
        }

        protected void ImageDownloadFinish(object sender, EventArgs e)
        {
            OnPropertyChanged("PhoneImage");   
        }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public KeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public enum MailStatusEnum
    {
        email_default,
        email_creating,
        email_sending,
        email_send,
    }
}
