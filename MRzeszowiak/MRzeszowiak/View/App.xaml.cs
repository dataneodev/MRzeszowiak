using MRzeszowiak.Model;
using MRzeszowiak.Services;
using MRzeszowiak.View;
using MRzeszowiak.ViewModel;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
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
        public App(IPlatformInitializer initializer = null) : base(initializer){  }
        public App(IPlatformInitializer initializer, bool setFormsDependencyResolver)
            : base(initializer, setFormsDependencyResolver){ }

        protected override async void OnInitialized()
        {
            #if DEBUG
            LiveReload.Init();
            #endif

            InitializeComponent();

            var navigationParams = new NavigationParameters("LoadAtStartup=true");
            await NavigationService.NavigateAsync(new Uri("app:///MenuMasterDetail/MainNavigation/ListPage", UriKind.Absolute), navigationParams);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Support IApplicationLifecycleAware
            base.OnSleep();
        }

        protected override void OnResume()
        {
            // Support IApplicationLifecycleAware
            base.OnResume();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterPopupNavigationService();

            containerRegistry.RegisterSingleton<ISetting, SettingRepository>();
            containerRegistry.Register<IRzeszowiak, RzeszowiakRepository>();
            containerRegistry.Register<IRzeszowiakImageContainer, RzeszowiakImageContainer>();

            containerRegistry.RegisterForNavigation<MainNavigation>();
            containerRegistry.RegisterForNavigation<MenuMasterDetail, MenuMasterDetailViewModel>();
            containerRegistry.RegisterForNavigation<ListPage, ListViewModel>();
            containerRegistry.RegisterForNavigation<PreviewPage, PreviewViewModel>();
            containerRegistry.RegisterForNavigation<PreviewImagePage, PreViewImageViewModel>();
            containerRegistry.RegisterForNavigation<CategorySelectPopup, CategorySelectViewModel>();
            containerRegistry.RegisterForNavigation<SearchPopup, SearchViewModel>();
            containerRegistry.RegisterForNavigation<SettingPage, SettingViewModel>();            
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            Prism.Mvvm.ViewModelLocationProvider.SetDefaultViewModelFactory((type) => { return Container.Resolve(type); });
        }
    }
}
