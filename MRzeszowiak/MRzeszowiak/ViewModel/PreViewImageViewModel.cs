using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    public class PreViewImageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> ImageURLsList { get; set; } = new ObservableCollection<string>();
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

        public PreViewImageViewModel(INavigationService navigationService)
        {
            MessagingCenter.Subscribe<View.PreviewPage, Tuple<IEnumerable<string>, int>>(this, "ShowImagePreview", (sender, imageList) => {
                LoadImage(imageList.Item1, imageList.Item2);
            });  
        }

        ~PreViewImageViewModel()
        {
            MessagingCenter.Unsubscribe<View.PreviewPage, Tuple<IEnumerable<string>, int>>(this, "ShowImagePreview");
        }

        void LoadImage(IEnumerable<string> imageList, int position)
        {
            ImageURLsList.Clear();
            foreach (var item in imageList)
                ImageURLsList.Add(item);

            if (position >= 0 && position < ImageURLsList.Count)
                Position = position;
            else Position = 0;
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
