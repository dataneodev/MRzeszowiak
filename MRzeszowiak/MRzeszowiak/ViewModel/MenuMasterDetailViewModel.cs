using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRzeszowiak.ViewModel
{
    class MenuMasterDetailViewModel
    {
        private readonly INavigationService _navigationService;

        public DelegateCommand<string> NavigateCommand { get; set; }

        public MenuMasterDetailViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void Navigate(string name)
        {
            var res = _navigationService.NavigateAsync(name);
        }
    }
}
