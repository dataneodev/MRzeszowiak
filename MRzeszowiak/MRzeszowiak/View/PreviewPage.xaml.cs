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
		public PreviewPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void AddDataList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //Debug.Write("TapGestureRecognizer_Tapped");
            //if(CarouselViewImageList?.ItemsSource?.GetCount() > 0)
            //{
            //    await Navigation.PushAsync(new PreviewImagePage(), false);
            //    MessagingCenter.Send<View.PreviewPage, Tuple<IEnumerable<string>, int>>(this, "ShowImagePreview", 
            //        new Tuple<IEnumerable<string>, int>(CarouselViewImageList?.ItemsSource?.Cast<string>(), CarouselViewImageList?.Position??0));
            //}
        }

        private void AddDataList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var slv = (sender as ListView);
            var count = slv?.ItemsSource?.GetCount();
            slv.HeightRequest = (count ?? 0) * (slv.RowHeight + 2) + 14;
        }

        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    if(e.TotalX > (App.DisplayScreenWidth  / 2))
                    {
                        Debug.Write("PreviewPage -> ładowanie do przodu");
                        MessagingCenter.Send<View.PreviewPage>(this, "PreViewFowardRequest");
                    }

                    if (e.TotalX > (App.DisplayScreenWidth / 2))
                    {
                        Debug.Write("PreviewPage -> ładowanie do tyłu");
                        MessagingCenter.Send<View.PreviewPage>(this, "PreViewBackRequest");
                    }
                    break;
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            
        }
    }
}