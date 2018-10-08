using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MRzeszowiak.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.ViewModel
{
    public class ViewModelLocator
    {
        public ListViewModel ListViewModel => ServiceLocator.Current.GetInstance<ListViewModel>();
        public PreviewViewModel PreviewViewModel => ServiceLocator.Current.GetInstance<PreviewViewModel>();
        public SettingViewModel SettingViewModel => ServiceLocator.Current.GetInstance<SettingViewModel>();
        public PreViewImageViewModel PreViewImageViewModel => ServiceLocator.Current.GetInstance<PreViewImageViewModel>();
        public CategorySelectViewModel CategorySelectViewModel => ServiceLocator.Current.GetInstance<CategorySelectViewModel>();

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ListViewModel>(() => new ListViewModel(new RzeszowiakRepository()));
            SimpleIoc.Default.Register<PreviewViewModel>(() => new PreviewViewModel(new RzeszowiakRepository(), new RzeszowiakImageContainer()));
            SimpleIoc.Default.Register<SettingViewModel>();
            SimpleIoc.Default.Register<PreViewImageViewModel>();
            SimpleIoc.Default.Register<CategorySelectViewModel>(() => new CategorySelectViewModel(new RzeszowiakRepository()));
        }          
    }
}
