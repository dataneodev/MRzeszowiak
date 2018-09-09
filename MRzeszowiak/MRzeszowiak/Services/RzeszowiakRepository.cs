using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MRzeszowiak.Services
{
    class RzeszowiakRepository : IRzeszowiakRepository
    {
        protected IList<Category> _lastCategoryList = new List<Category>();

        // from inteface
        public Task<IList<Category>> GetCategoryList(bool ForceReload = false)
        {
            return new Task<IList<Category>>(() => new List<Category>() { new Category() });
        }
        public Task<IList<AdvertShort>> GetAdvertList(AdvertSearch searchParams)
        {
            var result = new List<AdvertShort>
            {
                new AdvertShort()
                {
                    AdverIDinRzeszowiak = 6,
                    Title = "Auto – Naprawa Jerzy Szeliga - 37-114 Białobrzegi",
                    DescriptionShort = "sprzedam przepiękna szklaną paterę w kolorze niebieskim[wyrób włoski].bardzo ciekawy kształt,3 cm,",
                    DateAddString = "dziś",
                    Price = 126,
                    Highlighted = false,
                    ThumbnailUrl = "http://www.rzeszowiak.pl/img/ogl/105/mini/l_10577949_0.jpg?re=1149847778",
                }                
            };

            return Task.FromResult<IList<AdvertShort>>(result);
        }
        public Task<IList<AdvertShort>> GetAdvertList()
        {
            return GetAdvertList(new AdvertSearch());
        }
        public Task<Advert> GetAdvert(AdvertShort advertShort)
        {
            return new Task<Advert>(() => new Advert() );
        }
        public Task<Advert> GetAdvert(int advertId)
        {
            return new Task<Advert>(() =>  new Advert());
        }
    }
}
