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
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    public class CategorySelectViewModel : INotifyPropertyChanged
    {
        protected IRzeszowiak rzeszowiakRepository;

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<CatDisplay> CategoryAction { get; private set; } = new ObservableCollection<CatDisplay>();

        protected CatButtonDisplay firstCatButton;
        public CatButtonDisplay FirstCatButton
        {
            get { return firstCatButton;  }
            set
            {
                firstCatButton = value;
                OnPropertyChanged();
            }
        }

        protected CatButtonDisplay secondCatButton;
        public CatButtonDisplay SecondCatButton
        {
            get { return secondCatButton; }
            set
            {
                secondCatButton = value;
                OnPropertyChanged();
            }
        }

        protected CatButtonDisplay thirdCatButton;
        public CatButtonDisplay ThirdCatButton
        {
            get { return thirdCatButton; }
            set
            {
                thirdCatButton = value;
                OnPropertyChanged();
            }
        }

        public ICommand CategoryTappet { get; private set; }
        public ICommand FirstButtontapped { get; private set; }
        public ICommand SecondButtontapped { get; private set; }

        protected Category LastSelectedCategory { get; set; }
        
        public CategorySelectViewModel(IRzeszowiak RzeszowiakRepository)
        {
            rzeszowiakRepository = RzeszowiakRepository;
            FirstCatButton = new CatButtonDisplay { Title = "", IsVisible = false, Image = CatSelectImage.none };
            SecondCatButton = new CatButtonDisplay { Title = "", IsVisible = false, Image = CatSelectImage.none };
            ThirdCatButton = new CatButtonDisplay { Title = "", IsVisible = false, Image = CatSelectImage.none };

            CategoryTappet = new Command<CatDisplay>(ItemTapedAsync);
            FillCategoryActionAsync();
        }

        protected async void ItemTapedAsync(CatDisplay catTapped)
        {
            Debug.Write("ItemTapedAsync");
            if (catTapped == null)
            {
                Debug.Write("ItemTapedAsync catTapped == null");
                return;
            }

            // clicked on Wszystkie kategorie
            if (catTapped.CategoryObj == null)
            {
                Debug.Write("Tapped all category");
                LastSelectedCategory = null;
                if(SecondCatButton.masterCategory != null)
                {
                   // LastSelectedCategory = SecondCatButton.masterCategory;
                }
                //close
                return;
            }

            if (catTapped.CategoryObj.GetType() == typeof(MasterCategory))
            {
                Debug.Write("Tapped CategoryObj is MasterCategory");
                var master = catTapped.CategoryObj as MasterCategory;
                await DisplayCategoryAsync(master);
            }

            if (catTapped.CategoryObj.GetType() == typeof(Category))
            {
                Debug.Write("Tapped CategoryObj is Category");
                var category = catTapped.CategoryObj as Category;

                if ((category.ChildCategory?.Count ?? 0) > 0)
                {
                    // displaing more
                    await DisplayCategoryAsync(category);
                }
                else
                {
                    category.SelectedChildCategory = null;
                    LastSelectedCategory = category;
                    catTapped.Image = CatSelectImage.selected;
                }
            }

            if (catTapped.CategoryObj.GetType() == typeof(ChildCategory))
            {
                Debug.Write("Tapped CategoryObj is ChildCategory");
                var child = catTapped.CategoryObj as ChildCategory;
                LastSelectedCategory = child?.ParentCategory;
                LastSelectedCategory.SelectedChildCategory = child;
                catTapped.Image = CatSelectImage.selected;
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
                Debug.Write("Displaing categoryToShow is null");
                int AllViews = 0;
                FirstCatButton.Image = CatSelectImage.none;
                FirstCatButton.masterCategory = null;
                SecondCatButton.masterCategory = null;
                ThirdCatButton.masterCategory = null;
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
            if (categoryToShow.GetType() == typeof(MasterCategory))
            {
                Debug.Write("Displaing categoryToShow is MasterCategory");
                var master = categoryToShow as MasterCategory;

                FirstCatButton.Image = CatSelectImage.arrowUp;
                FirstCatButton.masterCategory = master;
                SecondCatButton.IsVisible = true;
                SecondCatButton.Title = master.Title;
                SecondCatButton.Image = CatSelectImage.none;
                SecondCatButton.masterCategory = master;
                ThirdCatButton.IsVisible = false;
                ThirdCatButton.masterCategory = null;

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
            if (categoryToShow.GetType() == typeof(Category))
            {
                Debug.Write("Displaing categoryToShow is Category");
                var category = categoryToShow as Category;

                FirstCatButton.Image = CatSelectImage.arrowUp;
                FirstCatButton.masterCategory = category.Master;
                SecondCatButton.IsVisible = true;
                SecondCatButton.Title = category.Master.Title;
                SecondCatButton.Image = CatSelectImage.arrowUp;
                SecondCatButton.masterCategory = category.Master;
                ThirdCatButton.IsVisible = true;
                ThirdCatButton.Title = category.Title;
                ThirdCatButton.Image = CatSelectImage.none;
                ThirdCatButton.masterCategory = category;

                CategoryAction.Add(new CatDisplay
                {
                    Title = $"Wszystkie w {category.Title}",
                    Views = category.Views,
                    Image = (LastSelectedCategory?.Id ?? 0) == category.Id ? CatSelectImage.selected : CatSelectImage.none,
                    CategoryObj = null,
                });

                foreach (var child in category.ChildCategory)
                {
                    CategoryAction.Add(
                        new CatDisplay
                        {
                            Title = child.Title,
                            Views = child.Views,
                            Image = CatSelectImage.none,
                            //Image = ((LastSelectedCategory?.Id ?? 0) == category.Id &&
                            //        (LastSelectedCategory?.SelectedChildCategory?.ID ?? String.Empty) == child.ID) ? CatSelectImage.selected : CatSelectImage.none,
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

    public class CatDisplay : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Title { get; set; }
        public int Views { get; set; }
        private CatSelectImage image;
        public CatSelectImage Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }
        public object CategoryObj { get; set; }
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class CatButtonDisplay : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged();
            }
        }

        private CatSelectImage image;
        public CatSelectImage Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }

        public MasterCategory masterCategory { get; set; }
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public enum CatSelectImage
    {
        none,
        arrowDeeper,
        selected,
        arrowUp
    }
}
