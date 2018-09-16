using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            MessagingCenter.Send<View.ListPage>(this, "LoadLastOnStartup");
        }

        private async void AdvertListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
            var advertShort = e.Item as AdvertShort;
            if(advertShort != null)
                await Navigation.PushModalAsync(new PreviewPage(advertShort), true);
        }
    }
}