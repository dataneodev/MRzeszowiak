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
	public partial class MenuPage : MasterDetailPage
	{
		public MenuPage ()
		{
            InitializeComponent();
        }

        private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
            var item = e.Item as MasterPageItem;

            if(item?.TargetType == typeof(SettingPage))
            {
                await (Detail as NavigationPage)?.PushAsync(new SettingPage(), false);
            }

            if (item?.TargetType == typeof(AboutPage))
            {
                await (Detail as NavigationPage)?.PushAsync(new AboutPage(), false);
            }

            IsPresented = false;
        }
    }

    public class MasterPageItem
    {
        public string Title { get; set; }

        public string IconSource { get; set; }

        public Type TargetType { get; set; }
    }
}