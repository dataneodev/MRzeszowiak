using MRzeszowiak.Model;
using Rg.Plugins.Popup.Services;
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
	public partial class ListPage : ContentPage
	{
        public ListPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            MessagingCenter.Send<View.ListPage>(this, "LoadLastOnStartup");            
        }

        private void AdvertListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private void AdvertListView_ScrollStateChanged(object sender, Extends.ScrollStateChangedEventArgs e)
        {
            //var height = (int)ToolBarMenu.Height;
            //var margintop = (int)ToolBarMenu.Margin.Top;
            //var topindex = e.Y;
            //Debug.Write("AdvertListView_ScrollStateChanged : topindex " + topindex.ToString());
            //var delta = (int)(topindex - _lasttopindex) / 10; //-50    -40
            //int newmargintop = margintop;
            //Debug.Write("AdvertListView_ScrollStateChanged : delta " + delta.ToString());

            //if (delta > 0)
            //{
            //    newmargintop = Math.Min(margintop + delta, 0);
            //}
            //else
            //{
            //    newmargintop = Math.Max(margintop + delta, -1 * height);
            //}

            //if (margintop != newmargintop)
            //    ToolBarMenu.Margin = new Thickness(0, newmargintop, 0, 0);
            //_lasttopindex = topindex;
            //Debug.Write("AdvertListView_ScrollStateChanged : new margin top " + newmargintop.ToString());
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
                        MessagingCenter.Send<View.ListPage>(this, "RefreshList");
                    }
                    break;

                case GestureStatus.Completed:
                    break;
            } 
        }
    }
}