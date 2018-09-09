using MRzeszowiak.Model;
using MRzeszowiak.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    class ListViewModel
    {
        public ObservableCollection<AdvertShort> AdvertShortList { get; private set; } = new ObservableCollection<AdvertShort>();
        protected IRzeszowiakRepository _rzeszowiakRepository;

        public ListViewModel(IRzeszowiakRepository RzeszowiakRepository)
        {
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("ListViewModel => IRzeszowiakRepository RzeszowiakRepository == null !");

            var lastAddAdvert = _rzeszowiakRepository.GetAdvertList();
            //lastAddAdvert.Start();
            lastAddAdvert.Wait();
            foreach (var item in lastAddAdvert.Result)
            {
                AdvertShortList.Add(item);
            }
        }
    }
}
