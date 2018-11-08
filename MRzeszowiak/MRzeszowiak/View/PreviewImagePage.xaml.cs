using MRzeszowiak.Extends;
using MRzeszowiak.ViewModel;
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
    public partial class PreviewImagePage : ContentPage
    {
        public PreviewImagePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override bool OnBackButtonPressed()
        {
            Debug.Write("OnBackButtonPressed");
            if (BindingContext is PreViewImageViewModel model)
                model.BackButtonTapped?.Execute(null);
            return true;
        }
    }
}