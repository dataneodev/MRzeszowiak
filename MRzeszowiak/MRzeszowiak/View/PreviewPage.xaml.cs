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

            AddDataList.ItemAppearing += (object sender, ItemVisibilityEventArgs e) =>
            {
                Debug.Write("New hight: ////////////////////////");
                try
                    {
                        //if (AddDataList.ItemsSource != null)
                        {
                            var slv = (sender as ListView);
                            var count = slv?.ItemsSource?.GetCount();
                            slv.HeightRequest = count * slv.RowHeight ?? 40;
                            Debug.Write("New hight: " + slv.HeightRequest);
                        }
                    }
                    catch (Exception)
                    {

                    }
                
            };
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
                await Navigation.PushModalAsync(new PreviewImagePage(), false);
                MessagingCenter.Send<View.PreviewPage, IEnumerable<string>>(this, "ShowImagePreview", 
                    CarouselViewImageList?.ItemsSource?.Cast<string>());
            }
        }
    }
}