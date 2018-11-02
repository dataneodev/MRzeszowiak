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

        private double startScale = 1;
        private double midlleScale = 1.5d;
        private double endScale = 2;

        private void PinchToZoomContainerImage_Tapped(object sender, EventArgs e)
        {
            if(sender is PinchToZoomContainer pinchToZoomContainer)
            {
                double currentScale = pinchToZoomContainer.currentScale;
                double gotoScale = startScale;
                if (currentScale < midlleScale)
                    gotoScale = midlleScale;
                else if(currentScale < endScale)
                {
                    gotoScale = endScale;
                }
                else
                {
                    gotoScale = startScale;
                }

                pinchToZoomContainer.ConstTransform(gotoScale);
                Debug.Write("currentScale: "+ currentScale + " gotoScale:" + gotoScale);
            }
        }
    }
}