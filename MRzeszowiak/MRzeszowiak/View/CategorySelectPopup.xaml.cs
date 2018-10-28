using MRzeszowiak.ViewModel;
using Rg.Plugins.Popup.Pages;
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
            if (BindingContext is CategorySelectViewModel model)
                model.ScrollToTop = () =>
                {
                    foreach(var item in categoryListView.ItemsSource)
                    {
                        if (item != null)
                            categoryListView.ScrollTo(item, 0, true);
                        break;
                    }

                    foreach (var item in buttonListView.ItemsSource)
                    {
                        if (item != null)
                            buttonListView.ScrollTo(item, 0, true);
                        break;
                    }
                };
        }

        private void categoryListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }

}