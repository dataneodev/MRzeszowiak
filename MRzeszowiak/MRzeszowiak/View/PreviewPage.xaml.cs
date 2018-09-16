using CarouselView.FormsPlugin.Abstractions;
using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MRzeszowiak.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PreviewPage : ContentPage
	{
		public PreviewPage (AdvertShort advertShort)
		{
			InitializeComponent ();
            MessagingCenter.Send<View.PreviewPage, AdvertShort>(this, "LoadAdvertShort", advertShort);
        }

        private void AddDataList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private void AddDataList_BindingContextChanged(object sender, EventArgs e)
        {
            var lv = (sender as ListView);
            lv.HeightRequest = ((lv.ItemsSource as System.Collections.ArrayList)?.Count ?? 1) * lv.RowHeight;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Debug.Write("TapGestureRecognizer_Tapped");
            if(CarouselViewImageList?.ItemsSource?.GetCount() > 0)
            {
                await Navigation.PushModalAsync(new PreviewImagePage(), true);
                MessagingCenter.Send<View.PreviewPage, IEnumerable<string>>(this, "ShowImagePreview", 
                    CarouselViewImageList?.ItemsSource?.Cast<string>());
            }
        }
    }
}