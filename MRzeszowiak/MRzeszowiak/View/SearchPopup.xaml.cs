using Rg.Plugins.Popup.Pages;
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
	public partial class SearchPopup : PopupPage
    {
		public SearchPopup ()
		{
			InitializeComponent ();
		}

        private void PickerDateAdd_BindingContextChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if((picker != null) && (picker.Items.Count > 0))
                picker.SelectedIndex = picker.Items.Count - 1;                
        }

        private void PickerSortType_BindingContextChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if ((picker != null) && (picker.Items.Count > 0))
                picker.SelectedIndex = 0;
        }
    }
}