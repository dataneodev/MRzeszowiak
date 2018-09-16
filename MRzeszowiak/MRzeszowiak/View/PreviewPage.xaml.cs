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
	public partial class PreviewPage : ContentPage
	{
		public PreviewPage (AdvertShort advertShort)
		{
			InitializeComponent ();
            MessagingCenter.Send<View.PreviewPage, AdvertShort>(this, "LoadAdvertShort", advertShort);
        }
	}
}