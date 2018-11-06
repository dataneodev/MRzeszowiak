using MRzeszowiak.ViewModel;
using Prism.Navigation;
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
	public partial class MenuMasterDetail : MasterDetailPage, IMasterDetailPageOptions
    {
		public MenuMasterDetail()
		{
            InitializeComponent();
            if (BindingContext is MenuMasterDetailViewModel model)
                model.SetMenuPresented = (state) => IsPresented = state;
        }

        public bool IsPresentedAfterNavigation
        {
            get { return false; }
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }


}