using MRzeszowiak.Extends;
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
	public partial class MainNavigation : NavigationPage, IDestructible, INavigationPageOptions
    {
		public MainNavigation ()
		{
			InitializeComponent ();
		}

        public bool ClearNavigationStackOnNavigation
        {
            get { return false; }
        }

        public void Destroy()
        {

        }
    }
}