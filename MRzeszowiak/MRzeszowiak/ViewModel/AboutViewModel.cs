using MRzeszowiak.Interfaces;
using MRzeszowiak.Services;
using Prism.Services;
using System;
using System.Globalization;
using System.Windows.Input;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    class AboutViewModel : BaseViewModel
    {
        protected readonly IPageDialogService _pageDialog;
        private readonly ISetting _setting;

        public ICommand LinkPageTapped { get; private set; }
        public ICommand ButtonVersionCheckTapped { get; private set; }

        private bool buttonCheckEnable;
        public bool ButtonCheckEnabled { get { return buttonCheckEnable;  }
            set
            {
                buttonCheckEnable = value;
                OnPropertyChanged();
            }
        }

        public string AppName { get => _setting.GetAppNameAndVersion; }

        public AboutViewModel(IPageDialogService pageDialog, ISetting setting)
        {
            _pageDialog = pageDialog ?? throw new NullReferenceException("IPageDialogService pageDialog == null !");
            _setting = setting ?? throw new NullReferenceException("ISetting setting == null !");

            LinkPageTapped = new Command(()=>
            {
                Device.OpenUri(new Uri(_setting.GetProjectBaseURL));
            });

            ButtonVersionCheckTapped = new Command(async()=>
            {
                var update = new UpdateRepository(_setting);
                ButtonCheckEnabled = false;
                var checkStatus = await update.CheckUpdate();
                ButtonCheckEnabled = true;
                if (!checkStatus)
                {
                    await pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Wystąpił błąd podczas sprawdzania dostępności nowej wersji.", "OK");
                    return;
                }

                if (update.IsNewVersion)
                {
                    if(await pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Znaleziono nową wersje aplikacji " + _setting?.GetAppName + " " + 
                        update.VersionServer.ToString("0.0", CultureInfo.InvariantCulture) + "\nCzy chcesz przejść do strony z nową wersją aplikacji?", "OK", "Anuluj"))
                        Device.OpenUri(new Uri(update.VersionUpdateUrl));
                }                    
                else
                    await pageDialog.DisplayAlertAsync(_setting.GetAppNameAndVersion, "Posiadasz już najaktualniejszą wersje aplikacji.", "OK");
            });
        }


    }
}
