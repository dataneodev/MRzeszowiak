using CarouselView.FormsPlugin.Abstractions;
using GalaSoft.MvvmLight.Command;
using MRzeszowiak.Model;
using MRzeszowiak.Services;
using Newtonsoft.Json;
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
    class PreviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected IRzeszowiak _rzeszowiakRepository;

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

        private IRzeszowiakImageContainer imageContainer;
        public ImageSource PhoneImage
        {
            get { return imageContainer?.ImageData; }
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

        public ObservableCollection<KeyValue> AdditionalData { get; set; } = new ObservableCollection<KeyValue>();
        public bool AddDataVisible => (AdditionalData?.Count ?? 0) > 0 ? true : false;
        public ObservableCollection<string> ImageURLsList { get; set; } = new ObservableCollection<string>();
        public bool ImageVisible => (ImageURLsList?.Count ?? 0) > 0 ? true : false;

        private Advert _lastAdvert;
        public ICommand OpenAdvertPage { get; private set; }
        public ICommand RefreshAdvert { get; private set; }
        public ICommand MailAdvert { get; private set; }
        public ICommand FavoriteAdvert { get; private set; }

        public PreviewViewModel(IRzeszowiak RzeszowiakRepository, IRzeszowiakImageContainer rzeszowiakImageContainer)
        {
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("ListViewModel => IRzeszowiakRepository RzeszowiakRepository == null !");
            imageContainer = rzeszowiakImageContainer ?? throw new NullReferenceException("ListViewModel => IRzeszowiakImageContainer rzeszowiakImageContainer == null !");
            imageContainer.OnDownloadFinish += ImageDownloadFinish;

            ImageURLsList.CollectionChanged += (s, e) => { OnPropertyChanged("ImageVisible"); };
            AdditionalData.CollectionChanged += (s, e) => { OnPropertyChanged("AddDataVisible"); };

            MessagingCenter.Subscribe<View.PreviewPage, AdvertShort>(this, "LoadAdvertShort", (sender, advertShort) => {
                LoadAdvertMessage(advertShort);
            });

            OpenAdvertPage = new RelayCommand(()=> 
            {
                if(_lastAdvert?.URL?.Length > 0)
                    Device.OpenUri(new Uri(_lastAdvert?.URL));
            });

            RefreshAdvert = new RelayCommand(() =>
            {
                if (_lastAdvert != null)             
                    LoadAdvertMessage(_lastAdvert);
            });

            MailAdvert = new RelayCommand(() =>
            {
                
            });

            FavoriteAdvert = new RelayCommand(() =>
            {
                IsFavorite = ! IsFavorite;
                if (_lastAdvert != null) _lastAdvert.IsFavorite = IsFavorite;
            });
        }

        ~PreviewViewModel()
        {
            MessagingCenter.Unsubscribe<View.PreviewPage, AdvertShort>(this, "LoadAdvertShort");
        }

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
            foreach (var item in advert.AdditionalData)
                AdditionalData.Add(new KeyValue(item.Key, item.Value));

            ImageURLsList.Clear();
            foreach (var item in advert.ImageURLsList)
                ImageURLsList.Add(item);
            IsFavorite = advert.IsFavorite;


            if(advert.PhoneSsid.Length == 10 && advert.PhonePHPSSESION != null)
                imageContainer.DownloadImage(advert.PhoneSsid, advert.AdverIDinRzeszowiak, advert.URLPath, advert.PhonePHPSSESION); // no wait

            _lastAdvert = advert;
        }

        protected void ImageDownloadFinish(object sender, EventArgs e)
        {
            OnPropertyChanged("PhoneImage");   
        }

        // Create the OnPropertyChanged method to raise the event
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
}
