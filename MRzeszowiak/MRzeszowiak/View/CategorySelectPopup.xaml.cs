using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
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
	public partial class CategorySelectPopup : PopupPage
    {
		public CategorySelectPopup ()
		{
			InitializeComponent ();
		}

        private void PopupOKButton_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync(true);
        }

        private void categoryListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}