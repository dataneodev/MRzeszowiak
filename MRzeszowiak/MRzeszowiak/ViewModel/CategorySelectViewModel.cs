using MRzeszowiak.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace MRzeszowiak.ViewModel
{
    public class CategorySelectViewModel
    {
        protected IRzeszowiak rzeszowiakRepository;
        public ObservableCollection<CatSelect> CategoryAction { get; private set; } = new ObservableCollection<CatSelect>();
        public string SecCatButtonText { get; set; }

        public CategorySelectViewModel(IRzeszowiak RzeszowiakRepository)
        {
            rzeszowiakRepository = RzeszowiakRepository;
            FillCategoryActionAsync();
        }

        protected async System.Threading.Tasks.Task FillCategoryActionAsync()
        {
            var categoryList = await rzeszowiakRepository.GetCategoryListAsync();
            foreach(var category in categoryList)
            {
                CategoryAction.Add(new CatSelect { Title = category.Title, Views = category.Views  });
            }
        }
    }

    public class CatSelect
    {
        public string Title { get; set; }
        public short Views { get; set; }
        public CatSelectImage MyProperty { get; set; }
    }
    public enum CatSelectImage
    {
        none,
        arrowDeeper,
        selected,
    }
}
