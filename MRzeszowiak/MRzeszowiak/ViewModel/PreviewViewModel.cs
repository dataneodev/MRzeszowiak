using MRzeszowiak.Model;
using MRzeszowiak.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public bool ActivityForm => !Activity;
    
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

        private bool imageVisible = false;
        public bool ImageVisible
        {
            get { return imageVisible; }
            set
            {
                imageVisible = value;
                OnPropertyChanged();
            }
        }


        public Advert _advert { get; private set; } = new Advert() { Title = "Testowy" };
        public HtmlWebViewSource DescriptionHtml { get { return new HtmlWebViewSource { Html = _advert?.DescriptionHTML }; } }

        public PreviewViewModel(IRzeszowiakRepository RzeszowiakRepository)
        {
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("ListViewModel => IRzeszowiakRepository RzeszowiakRepository == null !");
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

            Activity = true;
            ImageVisible = false;
            _advert = await _rzeszowiakRepository.GetAdvertAsync(advertShort);
            
            if(_advert?.ImageURLsList?.Count > 0)
            {
                ImageVisible = true;
            }
            
            
            Activity = false;
            OnPropertyChanged("_advert");
            _advert.PropertyRefresh();
            _advert.OnPropertyChanged("Price");
            //Debug.Write(JsonConvert.SerializeObject(_advert));
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
}
