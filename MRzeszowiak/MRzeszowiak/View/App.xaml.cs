using MRzeszowiak.View;
using MRzeszowiak.ViewModel;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace MRzeszowiak
{
	public partial class App : PrismApplication
    {
        private static ViewModelLocator locator;
        public static ViewModelLocator Locator
        {
            get { return locator ?? (locator = new ViewModelLocator()) ; }
        }

        public static Color highlightRow = Color.FromHex("#f4f4f4");
        public static Color normalRow = Color.FromHex("#FFFFFF0");
        public static Color highlightPremiumRow = Color.FromHex("#fcd890");
        public static Color normalPremiumRow = Color.FromHex("#ffedcc");
        public static double DisplayScreenWidth = 0f;
        public static double DisplayScreenHeight = 0f;
        public static double DisplayScaleFactor = 0f;



        public static CategorySelectPopup CatalogPopUp { get { return new CategorySelectPopup(); } }

        public App(IPlatformInitializer initializer = null) : base(initializer)
		{
        }

        protected override void OnInitialized()
        {
            #if DEBUG
            LiveReload.Init();
            #endif

            InitializeComponent();
            MainPage = new MenuPage();
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ListPage>();
            containerRegistry.RegisterForNavigation<PreviewPage>();
            containerRegistry.RegisterForNavigation<PreviewImagePage>();
            containerRegistry.RegisterForNavigation<CategorySelectPopup>();
            containerRegistry.RegisterForNavigation<SettingPage>();
        }
    }
}
