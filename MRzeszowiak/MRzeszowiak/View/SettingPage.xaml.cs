using MRzeszowiak.ViewModel;
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
	public partial class SettingPage : ContentPage
	{
		public SettingPage ()
		{
			InitializeComponent ();
            Disappearing += async (sender, e) => {
                if (BindingContext is SettingViewModel model)
                    await model.Setting.SaveSettingAsync();
            };
		}


	}
}