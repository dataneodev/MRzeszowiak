using MRzeszowiak.Model;
using MRzeszowiak.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    class ListViewModel
    {
        public ObservableCollection<AdvertShort> AdvertShortList { get; private set; } = new ObservableCollection<AdvertShort>();
        protected IRzeszowiakRepository _rzeszowiakRepository;

        public ICommand LoadTest
        {
            get
            {
                return new Command(() => LoadLast());
            }
        }

        public ListViewModel(IRzeszowiakRepository RzeszowiakRepository)
        {
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("ListViewModel => IRzeszowiakRepository RzeszowiakRepository == null !");
        }

        public void LoadLast()
        {
            var lastAddAdvert = _rzeszowiakRepository.GetAdvertList();

            Task.WaitAll(lastAddAdvert);

            // lastAddAdvert.Wait();
            foreach (var item in lastAddAdvert.Result)
            {
                AdvertShortList.Add(item);
            }
        }
    }
}
