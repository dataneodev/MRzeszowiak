using MRzeszowiak.ViewModel;
using System.Diagnostics;
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
            if(BindingContext is PreviewViewModel model)
                model.ScrollToButtom = async () => await PreviewPageScroll.ScrollToAsync(0, PreviewPageScroll.ContentSize.Height, true); 
        }

        private void AddDataList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private void AddDataList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var slv = (sender as ListView);
            if (BindingContext is PreviewViewModel model)
            {
                var count = model.AdditionalData?.Count ?? 0;
                slv.HeightRequest = count * (slv.RowHeight + 2) + 14;
            }
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

        protected override bool OnBackButtonPressed()
        {
            Debug.Write("OnBackButtonPressed");
            if (BindingContext is PreviewViewModel model)
                model.BackButtonTapped?.Execute(null);
            return true;
        }
    }
}