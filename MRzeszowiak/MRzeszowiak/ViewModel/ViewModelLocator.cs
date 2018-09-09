﻿using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.ViewModel
{
    class ViewModelLocator
    {
        public ListViewModel ListViewModel => ServiceLocator.Current.GetInstance<ListViewModel>();
        public PreviewViewModel PreviewViewModel => ServiceLocator.Current.GetInstance<PreviewViewModel>();
        public SettingViewModel SettingViewModel => ServiceLocator.Current.GetInstance<SettingViewModel>();

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ListViewModel>();
            SimpleIoc.Default.Register<PreviewViewModel>();
            SimpleIoc.Default.Register<SettingViewModel>();
        }          
    }
}