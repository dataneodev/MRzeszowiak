using MRzeszowiak.Services;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MRzeszowiak.ViewModel
{
    public class SettingViewModel : BaseViewModel
    {
        protected readonly INavigationService _navigationService;
        protected readonly IPageDialogService _pageDialog;
        protected readonly ISetting _setting;


        public SettingViewModel(INavigationService navigationService, IPageDialogService pageDialog, ISetting setting)
        {
            Debug.Write("SettingViewModel Contructor");
            _navigationService = navigationService ?? throw new NullReferenceException("INavigationService navigationService == null !");
            _pageDialog = pageDialog ?? throw new NullReferenceException("IPageDialogService pageDialog == null !");
            _setting = setting ?? throw new NullReferenceException("ISetting setting == null !");
        }
    }
}
