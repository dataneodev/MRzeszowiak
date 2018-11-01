using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using Unity;
using Prism;
using Prism.Ioc;

namespace MRzeszowiak.Droid
{
    [Activity(Label = "MRzeszowiak", 
        Icon = "@drawable/icon", 
        Theme = "@style/MainTheme", 
        MainLauncher = false, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.SetTheme(Resource.Style.MainTheme);
            Title = App.GetAppName;

            Forms.SetFlags("FastRenderers_Experimental");

            App.DisplayScreenWidth = (double)Resources.DisplayMetrics.WidthPixels / (double)Resources.DisplayMetrics.Density;
            App.DisplayScreenHeight = (double)Resources.DisplayMetrics.HeightPixels / (double)Resources.DisplayMetrics.Density;
            App.DisplayScaleFactor = (double)Resources.DisplayMetrics.Density;

            base.OnCreate(bundle);

            Rg.Plugins.Popup.Popup.Init(this, bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            CarouselView.FormsPlugin.Android.CarouselViewRenderer.Init();

            var dbpath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            LoadApplication(new App(dbpath, new AndroidInitializer()));
        }

        private bool _backToExitPressedOnce = false;
        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                return;
            }

            base.OnBackPressed();
            return;

            // determine if any popups are open
            var childViewCount = ((ViewGroup)((Activity)Forms.Context).Window.DecorView).ChildCount;

            // Check if a non modal page has been pushed, if any modal page or popups are open
            if (Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count > 1 ||
                Xamarin.Forms.Application.Current.MainPage.Navigation.ModalStack.Count > 0 ||
                childViewCount > 2)
            {
                base.OnBackPressed();
                return;
            }

            

            if (_backToExitPressedOnce)
            {
                base.OnBackPressed();
                Java.Lang.JavaSystem.Exit(0);
                return;
            }

            this._backToExitPressedOnce = true;
            Toast.MakeText(this, "Tap again to exit", ToastLength.Short).Show();

            new Handler().PostDelayed(() => { _backToExitPressedOnce = false; }, 2000);
        }

        public class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry container)
            {

            }
        }
    }
}

