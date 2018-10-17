using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.ViewModel
{
    class MenuMasterDetailViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;


        public MenuMasterDetailViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;


        }
    }

    public class MasterPageItem
    {
        public string Title { get; set; }
        public string IconSource { get; set; }
        public Type TargetType { get; set; }
    }
}
