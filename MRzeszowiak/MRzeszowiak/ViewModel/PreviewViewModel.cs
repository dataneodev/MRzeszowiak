using MRzeszowiak.Model;
using MRzeszowiak.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    class PreviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected IRzeszowiakRepository _rzeszowiakRepository;

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

        public bool ImageVisible => (ImageURLsList?.Count ?? 0) > 0 ? true: false ; 

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

        private Image phoneImage;
        public Image PhoneImage
        {
            get { return phoneImage; }
            set
            {
                phoneImage = value;
                OnPropertyChanged();
            }
        }

        private string errorMessage;
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
        public ObservableCollection<string> ImageURLsList { get; set; } = new ObservableCollection<string>();

        public PreviewViewModel(IRzeszowiakRepository RzeszowiakRepository)
        {
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("ListViewModel => IRzeszowiakRepository RzeszowiakRepository == null !");
            ImageURLsList.CollectionChanged += (s, e) => { OnPropertyChanged("ImageVisible"); };

            MessagingCenter.Subscribe<View.PreviewPage, AdvertShort>(this, "LoadAdvertShort", (sender, advertShort) => {
                LoadAdvert(advertShort);
            });
        }

        async void LoadAdvert(AdvertShort advertShort)
        {
            if(advertShort == null)
                throw new NullReferenceException("LoadAdvert => advertShort == null !");
            if (advertShort.AdverIDinRzeszowiak == 0)
                throw new ArgumentException("advertShort.AdverIDinRzeszowiak == 0");

            ErrorMessage = String.Empty;
            Activity = true;
   
            var _advert = await _rzeszowiakRepository.GetAdvertAsync(advertShort);
                     
            if (_advert == null)
                ErrorMessage = "Błąd podczas ładowania strony.\nSprawdź połączenie internetowe i spróbuj ponownie.";
            else
                CopyAdverToViewModel(_advert);
            Activity = false;
            Debug.Write(JsonConvert.SerializeObject(_advert));
        }

        void CopyAdverToViewModel(Advert advert)
        {
            AdverIDinRzeszowiak = advert.AdverIDinRzeszowiak;
            Title = advert.Title;
            Price = advert.Price;
            Views = advert.Views;
            DescriptionHTML = advert.DescriptionHTML;
            DateAdd = advert.DateAddString;
            ExpiredDateAdd = advert.ExpiredString;
            Highlighted = advert.Highlighted;
            AdditionalData.Clear();
            foreach (var item in advert.AdditionalData)
                AdditionalData.Add(new KeyValue(item.Key, item.Value));
            ImageURLsList.Clear();
            foreach (var item in advert.ImageURLsList)
                ImageURLsList.Add(item);

        }

        // Create the OnPropertyChanged method to raise the event
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
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
