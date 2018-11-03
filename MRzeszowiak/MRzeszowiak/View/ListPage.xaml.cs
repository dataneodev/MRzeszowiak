using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MRzeszowiak.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ListPage : ContentPage
	{
        public ListPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);           
        }

        private void MainMenuButton_Clicked(object sender, EventArgs e)
        {
            (App.Current.MainPage as MasterDetailPage).IsPresented = true;
        }

        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Debug.Write("ListPage -> PanGestureRecognizer_PanUpdated");
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    if ((e?.TotalY ?? 0 * -1) > (ListPageXaml.Height / 4))
                    {
                        Debug.Write("ListPage -> PanGestureRecognizer_PanUpdated -> przeładowanie");
                        
                    }
                    break;

                case GestureStatus.Completed:
                    break;
            } 
        }
    }
}