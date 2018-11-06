using MRzeszowiak.Interfaces;
using MRzeszowiak.Services;
using MRzeszowiak.View;
using MRzeszowiak.ViewModel;
using Prism;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Plugin.Popups;
using Prism.Unity;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace MRzeszowiak
{
	public partial class App : PrismApplication
    {
        public static ISetting Setting { get; private set; } = new SettingRepository();

        public static string RzeszowiakURL = "http://www.rzeszowiak.pl";
        public static string GetAppName = "MRzeszowiak";
        public static float GetAppVersion = 0.3f;
        public static string GetAppNameAndVersion = GetAppName + " " + GetAppVersion.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);

        public static Color highlightRow = Color.FromHex("#f4f4f4");
        public static Color normalRow = Color.FromHex("#FFFFFF0");
        public static Color highlightPremiumRow = Color.FromHex("#fcd890");
        public static Color normalPremiumRow = Color.FromHex("#ffedcc");
        public static double DisplayScreenWidth = 0f;
        public static double DisplayScreenHeight = 0f;
        public static double DisplayScaleFactor = 0f;

        public App(string dbpath, IPlatformInitializer initializer = null) : this(initializer, true)
        {
            var dbTask = new Task(async () => { await Setting.SetDBPath(dbpath); });
            dbTask.Start();
            dbTask.Wait();
            var navigationParams = new NavigationParameters("LoadAtStartup=true");
            NavigationService.NavigateAsync("app:///MenuMasterDetail/MainNavigation/ListPage", navigationParams);
        }

        public App(IPlatformInitializer initializer, bool setFormsDependencyResolver)
            : base(initializer, setFormsDependencyResolver){ }

        protected override void OnInitialized()
        {
            #if DEBUG
            LiveReload.Init();
            #endif
            InitializeComponent();
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

            containerRegistry.RegisterInstance<ISetting>(App.Setting);
            containerRegistry.Register<IRzeszowiak, RzeszowiakRepository>();
            containerRegistry.Register<IRzeszowiakImageContainer, RzeszowiakImageContainer>();
 
            containerRegistry.RegisterForNavigation<MainNavigation>();
            containerRegistry.RegisterForNavigation<MenuMasterDetail, MenuMasterDetailViewModel>();
            containerRegistry.RegisterForNavigation<ListPage, ListViewModel>();
            containerRegistry.RegisterForNavigation<PreviewPage, PreviewViewModel>();
            containerRegistry.RegisterForNavigation<PreviewImagePage, PreViewImageViewModel>();
            containerRegistry.RegisterForNavigation<FavAdvertPage, FavAdvertViewModel>();
            containerRegistry.RegisterForNavigation<FavSearchPage, FavSearchViewModel>();
            containerRegistry.RegisterForNavigation<CategorySelectPopup, CategorySelectViewModel>();
            containerRegistry.RegisterForNavigation<SearchPopup, SearchViewModel>();
            containerRegistry.RegisterForNavigation<SettingPage, SettingViewModel>();
            containerRegistry.RegisterForNavigation<AboutPage, AboutViewModel>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => { return Container.Resolve(type); });
        }
    }
}
