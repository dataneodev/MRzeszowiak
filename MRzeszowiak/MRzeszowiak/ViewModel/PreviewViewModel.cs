using MRzeszowiak.Model;
using MRzeszowiak.Services;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    public class PreviewViewModel : BaseViewModel, INavigationAware
    {
        protected readonly IRzeszowiak _rzeszowiakRepository;
        protected readonly IRzeszowiakImageContainer _imageContainer;
        protected readonly INavigationService _navigationService;
        protected readonly IPageDialogService _pageDialog;
        protected readonly ISetting _setting;

        public bool ActivityForm => !Activity && !ErrorPanelVisible ? true : false;

        private bool activity = true;
        public bool Activity
        {
            get { return activity; }
            set
            {
                activity = value;
                OnPropertyChanged();
                OnPropertyChanged("ActivityForm");
            }
        }

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

        private string errorMessage = String.Empty;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged("ErrorPanelVisible");
                OnPropertyChanged("ActivityForm");
            }
        }
        public bool ErrorPanelVisible => (errorMessage?.Length ?? 0) > 0 ? true : false;

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

        public ObservableCollection<KeyValue> AdditionalData { get; set; } = new ObservableCollection<KeyValue>();
        public bool AddDataVisible => (AdditionalData?.Count ?? 0) > 0 ? true : false;
        public ObservableCollection<string> ImageURLsList { get; set; } = new ObservableCollection<string>();
        public bool ImageVisible => (ImageURLsList?.Count ?? 0) > 0 ? true : false;
        public Action ScrollToButtom;

        private Advert _lastAdvert;
        public ICommand OpenAdvertPage { get; private set; }
        public ICommand RefreshAdvert { get; private set; }
        public ICommand MailAdvert { get; private set; }
        public ICommand FavoriteAdvert { get; private set; }
        public ICommand ImageTapped { get; private set; }
        public ICommand BackButtonTapped { get; private set; }
        public ICommand MailSendButtonTapped { get; private set; }

        public PreviewViewModel(INavigationService navigationService, IRzeszowiak RzeszowiakRepository,
                                IRzeszowiakImageContainer rzeszowiakImageContainer, IPageDialogService pageDialog, ISetting setting)
        {
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("IRzeszowiakRepository RzeszowiakRepository == null !");
            _navigationService = navigationService ?? throw new NullReferenceException("INavigationService navigationService == null !");
            _pageDialog = pageDialog ?? throw new NullReferenceException("IPageDialogService pageDialog == null !");
            _imageContainer = rzeszowiakImageContainer ?? throw new NullReferenceException("IRzeszowiakImageContainer rzeszowiakImageContainer == null !");
            _setting = setting ?? throw new NullReferenceException("ISetting setting == null !");
            _imageContainer.OnDownloadFinish += ImageDownloadFinish;

            ImageURLsList.CollectionChanged += (s, e) => { OnPropertyChanged("ImageVisible"); };
            AdditionalData.CollectionChanged += (s, e) => { OnPropertyChanged("AddDataVisible"); };

            OpenAdvertPage = new Command(() =>
            {
                if (_lastAdvert?.URL?.Length > 0)
                    Device.OpenUri(new Uri(_lastAdvert?.URL));
            });

            RefreshAdvert = new Command(() =>
            {
                if (_lastAdvert != null)
                    LoadAdvertMessage(_lastAdvert);
            });

            MailAdvert = new Command(() =>
            {
                Debug.Write("MailAdvert");
                if (MailStatus == MailStatusEnum.email_creating)
                {
                    MailStatus = MailStatusEnum.email_default;
                    return;
                }

                if (!_setting.IsUserMailCorrect)
                {
                    _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Nie można wysłać wiadomości.\nUzupełnij w ustawieniach aplikacji swój adres email.", "OK");
                    return;
                }

                if (!setting.CanSendMail(_lastAdvert))
                {
                    _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Nie można narazie wysłać wiadomości.", "OK");
                    return;
                }

                if (_lastAdvert?.EmailToken.Length != 10)
                {
                    _pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Problem z wysłaniem wiadomości. Odśwież ogłoszenie.", "OK");
                    return;
                }

                MailStatus = MailStatusEnum.email_creating;
                ScrollToButtom?.Invoke();
            });

            FavoriteAdvert = new Command(() =>
            {
                IsFavorite = !IsFavorite;
                if (_lastAdvert != null) _lastAdvert.IsFavorite = IsFavorite;
            });

            ImageTapped = new Command<int>((selecteIndex) =>
            {
                if (ImageURLsList.Count == 0) return;

                var navigationParams = new NavigationParameters();
                navigationParams.Add("ImageSelectedIndex", selecteIndex);
                navigationParams.Add("ImageList", ImageURLsList);
                _navigationService.NavigateAsync("PreviewImagePage", navigationParams, useModalNavigation: true, animated: false);
            });

            BackButtonTapped = new Command(() => 
            {
                _navigationService.GoBackAsync(null,useModalNavigation: true, animated: false);
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

                if (!setting.CanSendMail(_lastAdvert))
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
                    _setting.SendMailNotice(_lastAdvert);
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

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            Debug.Write("OnNavigatedTo PreviewViewModel");
            if (parameters.ContainsKey("AdvertShort"))
                if (parameters["AdvertShort"] is AdvertShort advertShort)
                    LoadAdvertMessage(advertShort);
        }

        public void OnNavigatingTo(INavigationParameters parameters) {  }
        public void OnNavigatedFrom(INavigationParameters parameters)   {  }

        async void LoadAdvertMessage(AdvertShort advertShort)
        {
            if(advertShort == null)
                throw new NullReferenceException("LoadAdvert => advertShort == null !");
            if (advertShort.AdverIDinRzeszowiak == 0)
                throw new ArgumentException("advertShort.AdverIDinRzeszowiak == 0");

            ErrorMessage = String.Empty;
            Activity = true;
            CopyAdverToViewModel(new Advert());

            var _advert = await _rzeszowiakRepository.GetAdvertAsync(advertShort);
                     
            if (_advert == null)
                ErrorMessage = "Błąd podczas ładowania strony.\nSprawdź połączenie internetowe i spróbuj ponownie.";
            else
                CopyAdverToViewModel(_advert);
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
            IsFavorite = advert.IsFavorite;

            if (advert.PhoneSsid.Length == 10 && advert.PhonePHPSSESION != null)
            {
                HasPhoneImage = true;
                _imageContainer.HideImage();
                _imageContainer.DownloadImage(advert.PhoneSsid, advert.AdverIDinRzeszowiak, advert.URLPath, advert.PhonePHPSSESION); // no wait
            }
            else
                HasPhoneImage = false;       
                
            _lastAdvert = advert;
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
