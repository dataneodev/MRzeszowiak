using MRzeszowiak.Services;
using CommonServiceLocator;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;
using Unity.Injection;
using Prism.Navigation;
using System.ComponentModel;

namespace MRzeszowiak.ViewModel
{
    public class ViewModelLocator
    {
        //public ListViewModel ListViewModel => ServiceLocator.Current.GetInstance<ListViewModel>();
        public PreviewViewModel PreviewViewModel => ServiceLocator.Current.GetInstance<PreviewViewModel>();
        public SettingViewModel SettingViewModel => ServiceLocator.Current.GetInstance<SettingViewModel>();
        public PreViewImageViewModel PreViewImageViewModel => ServiceLocator.Current.GetInstance<PreViewImageViewModel>();
        public CategorySelectViewModel CategorySelectViewModel => ServiceLocator.Current.GetInstance<CategorySelectViewModel>();
        public Prism.Navigation.INavigationService NavigationService => ServiceLocator.Current.GetInstance<INavigationService>();

        public ViewModelLocator()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterType<INavigationService, PageNavigationService>();
            unityContainer.RegisterType<IRzeszowiak, RzeszowiakRepository>();

            //unityContainer.RegisterType<ListViewModel>(new ContainerControlledLifetimeManager(), 
            //    new InjectionConstructor(PageNavigationService, new RzeszowiakRepository()));

            unityContainer.RegisterType<PreviewViewModel>(new ContainerControlledLifetimeManager(), 
                new InjectionConstructor(new RzeszowiakRepository(), new RzeszowiakImageContainer()));

            unityContainer.RegisterType<PreViewImageViewModel>(new ContainerControlledLifetimeManager());

            unityContainer.RegisterType<CategorySelectViewModel>(new ContainerControlledLifetimeManager(), 
                new InjectionConstructor(new RzeszowiakRepository()));

            unityContainer.RegisterType<SettingViewModel>(new ContainerControlledLifetimeManager());

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }          
    }
}
