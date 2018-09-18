﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    public class PreViewImageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> ImageURLsList { get; set; } = new ObservableCollection<string>();

        public PreViewImageViewModel()
        {
            MessagingCenter.Subscribe<View.PreviewPage, IEnumerable<string>>(this, "ShowImagePreview", (sender, imageList) => {
                LoadImage(imageList);
            });  
        }

        ~PreViewImageViewModel()
        {
            MessagingCenter.Unsubscribe<View.PreviewPage, IEnumerable<string>>(this, "ShowImagePreview");
        }

        void LoadImage(IEnumerable<string> imageList)
        {
            ImageURLsList.Clear();
            foreach (var item in imageList)
                ImageURLsList.Add(item);
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
