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
            NavigationPage.SetHasNavigationBar(this, false);
            MessagingCenter.Send<View.PreviewPage, AdvertShort>(this, "LoadAdvertShort", advertShort);
        }

        private void AddDataList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Debug.Write("TapGestureRecognizer_Tapped");
            if(CarouselViewImageList?.ItemsSource?.GetCount() > 0)
            {
                await Navigation.PushAsync(new PreviewImagePage(), false);
                MessagingCenter.Send<View.PreviewPage, IEnumerable<string>>(this, "ShowImagePreview", 
                    CarouselViewImageList?.ItemsSource?.Cast<string>());
            }
        }

        private void AddDataList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var slv = (sender as ListView);
            var count = slv?.ItemsSource?.GetCount();
            slv.HeightRequest = (count ?? 0) * (slv.RowHeight + 2) + 14;
        }
    }
}