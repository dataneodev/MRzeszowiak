using GalaSoft.MvvmLight.Command;
using MRzeszowiak.Model;
using MRzeszowiak.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MRzeszowiak.ViewModel
{
    public class CategorySelectViewModel : INotifyPropertyChanged
    {
        protected IRzeszowiak rzeszowiakRepository;

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<CatDisplay> CategoryAction { get; private set; } = new ObservableCollection<CatDisplay>();
        public CatButtonDisplay FirstCatButton { get; set; } = new CatButtonDisplay { Title = "All", IsVisible = false, Image = CatSelectImage.none };
        public CatButtonDisplay SecondCatButton { get; set; } = new CatButtonDisplay { Title = "All", IsVisible = true, Image = CatSelectImage.arrowUp };
        public CatButtonDisplay ThirdCatButton { get; set; } = new CatButtonDisplay { Title = "All", IsVisible = true, Image = CatSelectImage.arrowUp };
        public ICommand CategoryTappet;

        protected Category LastSelectedCategory { get; set; }
        

        public CategorySelectViewModel(IRzeszowiak RzeszowiakRepository)
        {
            rzeszowiakRepository = RzeszowiakRepository;
            CategoryTappet = new RelayCommand<object>(async (taped) => { await ItemTapedAsync(taped); });
            FillCategoryActionAsync();
        }

        protected async Task ItemTapedAsync(object catTapped)
        {
            if(catTapped is MasterCategory)
            {
                var master = catTapped as MasterCategory;
                await DisplayCategoryAsync(master);
            }
        }

        protected async Task DisplayCategoryAsync(object categoryToShow)
        {
            FirstCatButton.IsVisible = true;
            FirstCatButton.Title = "Wszystkie kategorie";

            CategoryAction.Clear();
            // displaying main category
            if (categoryToShow == null)
            {
                int AllViews = 0;
                FirstCatButton.Image = CatSelectImage.none;
                FirstCatButton.masterCategory = null;
                SecondCatButton.IsVisible = false;
                ThirdCatButton.IsVisible = false;

                var categoryList = await rzeszowiakRepository.GetMasterCategoryListAsync();
                foreach(var item in categoryList)
                    AllViews += item.Views;

                CategoryAction.Add(new CatDisplay
                {
                    Title = "Wszystkie kategorie",
                    Views = AllViews,
                    Image = LastSelectedCategory == null ? CatSelectImage.selected : CatSelectImage.none,
                    CategoryObj = null,
                });

                foreach (var category in categoryList)
                    CategoryAction.Add(
                        new CatDisplay
                        {
                            Title = category.Title,
                            Views = category.Views,
                            Image = CatSelectImage.arrowDeeper,
                            CategoryObj = category,
                        });
            }

            // display category
            if (categoryToShow is MasterCategory)
            {
                var master = categoryToShow as MasterCategory;

                FirstCatButton.Image = CatSelectImage.arrowUp;
                FirstCatButton.masterCategory = master;
                SecondCatButton.IsVisible = true;
                SecondCatButton.Title = master.Title;
                SecondCatButton.Image = CatSelectImage.none;
                SecondCatButton.masterCategory = null;
                ThirdCatButton.IsVisible = false;

                var categoryList = await rzeszowiakRepository.GetCategoryListAsync();
                foreach (var category in categoryList)
                    if (master.Id == category.Master.Id)
                        CategoryAction.Add(
                            new CatDisplay
                            {
                                Title = category.Title,
                                Views = category.Views,
                                Image = (LastSelectedCategory?.Id ?? 0) == category.Id ? CatSelectImage.selected :
                                        ((category.ChildCategory?.Count ?? 0) > 0 ? CatSelectImage.arrowDeeper : CatSelectImage.none),
                                CategoryObj = category,
                            });
            }

            //display childCategory
            if (categoryToShow is Category)
            {
                var category = categoryToShow as Category;

                FirstCatButton.Image = CatSelectImage.arrowUp;
                FirstCatButton.masterCategory = category;
                SecondCatButton.IsVisible = true;
                SecondCatButton.Title = category.Master.Title;
                SecondCatButton.Image = CatSelectImage.arrowUp;
                SecondCatButton.masterCategory = category.Master;
                ThirdCatButton.IsVisible = true;
                ThirdCatButton.Title = category.Title;
                ThirdCatButton.Image = CatSelectImage.none;

                CategoryAction.Add(new CatDisplay
                {
                    Title = $"Wszystkie w {category.Title}",
                    Views = category.Views,
                    Image = (LastSelectedCategory?.Id ?? 0) == category.Id ? CatSelectImage.selected : CatSelectImage.none,
                });

                foreach (var child in category.ChildCategory)
                {
                    Debug.Write("category.ChildCategory length: " + child.Title);
                    CategoryAction.Add(
                        new CatDisplay
                        {
                            Title = child.Title,
                            Views = child.Views,
                            Image = ((LastSelectedCategory?.Id ?? 0) == category.Id &&
                                    (LastSelectedCategory?.SelectedChildCategory?.ID ?? String.Empty) == child.ID) ? CatSelectImage.selected : CatSelectImage.none,
                            CategoryObj = child, 
                        });
                }
            }        
        }

        protected async Task FillCategoryActionAsync()
        {
            
            await DisplayCategoryAsync(null);
        }

        // Create the OnPropertyChanged method to raise the event
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class CatDisplay
    {
        public string Title { get; set; }
        public int Views { get; set; }
        public CatSelectImage Image { get; set; }
        public object CategoryObj { get; set; }
    }

    public class CatButtonDisplay
    {
        public string Title { get; set; }
        public bool IsVisible { get; set; }
        public CatSelectImage Image { get; set; }
        public MasterCategory masterCategory { get; set; }
    }

    public enum CatSelectImage
    {
        none,
        arrowDeeper,
        selected,
        arrowUp
    }
}
