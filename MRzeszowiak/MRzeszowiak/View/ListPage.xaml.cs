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
    }
}