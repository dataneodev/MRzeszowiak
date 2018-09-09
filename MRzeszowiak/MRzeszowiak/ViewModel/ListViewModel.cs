using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MRzeszowiak.ViewModel
{
    class ListViewModel
    {
        public ObservableCollection<AdvertShort> AdvertShortList { get; private set; } = new ObservableCollection<AdvertShort>();
        public string MyProperty { get; set; } = "HUJ";

        public ListViewModel()
        {
            AdvertShortList.Add(new AdvertShort
            {
                AdverIDinRzeszowiak = 6,
                Title = "Auto – Naprawa Jerzy Szeliga - 37-114 Białobrzegi",
                DescriptionShort = "sprzedam przepiękna szklaną paterę w kolorze niebieskim[wyrób włoski].bardzo ciekawy kształt,3 cm,",
                DateAddString = "dziś",
                Price = 126,
                Highlighted = false,
                ThumbnailUrl = "http://www.rzeszowiak.pl/img/ogl/105/mini/l_10577949_0.jpg?re=1149847778",
            });
        }
    }
}
