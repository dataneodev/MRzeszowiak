using MRzeszowiak.Services;
using MRzeszowiak.View;
using MRzeszowiak.ViewModel;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Prism.Unity;
using System;
using Unity.Lifetime;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace MRzeszowiak
{
	public partial class App : PrismApplication
    {
        public static Color highlightRow = Color.FromHex("#f4f4f4");
        public static Color normalRow = Color.FromHex("#FFFFFF0");
        public static Color highlightPremiumRow = Color.FromHex("#fcd890");
        public static Color normalPremiumRow = Color.FromHex("#ffedcc");
        public static double DisplayScreenWidth = 0f;
        public static double DisplayScreenHeight = 0f;
        public static double DisplayScaleFactor = 0f;

        public App():this(null){ }

        public App(IPlatformInitializer initializer = null) : base(initializer)
		{
        }

        protected override async void OnInitialized()
        {
            #if DEBUG
            LiveReload.Init();
            #endif

            InitializeComponent();
            await NavigationService.NavigateAsync("MenuPage");
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
            containerRegistry.RegisterPopupNavigationService();

            containerRegistry.Register<IRzeszowiak, RzeszowiakRepository>();
            containerRegistry.Register<IRzeszowiakImageContainer, RzeszowiakImageContainer>();

            containerRegistry.RegisterForNavigation<MenuPage>();
            containerRegistry.RegisterForNavigation<ListPage, ListViewModel>();
            containerRegistry.RegisterForNavigation<PreviewPage, PreviewViewModel>();
            containerRegistry.RegisterForNavigation<PreviewImagePage, PreViewImageViewModel>();
            containerRegistry.RegisterForNavigation<CategorySelectPopup, CategorySelectViewModel>();
            containerRegistry.RegisterForNavigation<SettingPage, SettingViewModel>();
        }
    }
}
