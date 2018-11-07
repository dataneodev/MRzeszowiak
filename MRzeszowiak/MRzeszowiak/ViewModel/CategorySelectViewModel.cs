using MRzeszowiak.Interfaces;
using MRzeszowiak.Model;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    public class CategorySelectViewModel : BaseViewModel, INavigationAware
    {
        protected readonly IRzeszowiak _rzeszowiakRepository;
        protected readonly INavigationService _navigationService;

        public ObservableCollection<CatDisplay> CategoryAction { get; private set; } = new ObservableCollection<CatDisplay>();
        public ObservableCollection<CatButtonDisplay> ButtonList { get; private set; } = new ObservableCollection<CatButtonDisplay>();

        public ICommand CategoryTappet { get; private set; }
        public ICommand ButtonTappped { get; private set; }
        public ICommand ButtonCloseTappped { get; private set; }

        protected Category LastSelectedCategory { get; set; }

        public short ButtonListViewHeight 
        {
           get
            {
                short rowHeight = 50;
                short result = 0;
                foreach(var item in ButtonList)
                    if (item.IsVisible) { result += (short)(rowHeight + 0); }
                return result;
            } 
        }

        public Action ScrollToTop { get; set; }

        public CategorySelectViewModel(INavigationService navigationService, IRzeszowiak RzeszowiakRepository)
        {
            _rzeszowiakRepository = RzeszowiakRepository ?? throw new NullReferenceException("ListViewModel => IRzeszowiakRepository RzeszowiakRepository == null !");
            _navigationService = navigationService ?? throw new NullReferenceException("INavigationService navigationService == null !");

            for (int i = 1; i < 4; i++)
                ButtonList.Add(new CatButtonDisplay((byte)i) { Title = "", IsVisible = false, Image = CatSelectImage.none });
            CategoryTappet = new Command<CatDisplay>(ItemTapedAsync);
            ButtonTappped = new Command<CatButtonDisplay>(ButtonTappedAsync);
            ButtonCloseTappped = new Command(async () => { await _navigationService.GoBackAsync(); });
        }

        public override async void OnNavigatingTo(INavigationParameters parameters) 
        {
            if (parameters.ContainsKey("SelectedCategory"))
            {
                Debug.Write("OnNavigatingTo - SelectedCategory");
                var cat = parameters["SelectedCategory"] as Category;
                LastSelectedCategory = cat;
                if(cat?.GetType() == typeof(Category))
                {

                    if(cat.ChildCategory?.Count > 0)
                        await DisplayCategoryAsync(cat);
                    else
                        await DisplayCategoryAsync(cat.Master);
                } else
                {
                    await DisplayCategoryAsync(null);
                }  
            }
            ScrollToTop?.Invoke();
        }

        protected async void ButtonTappedAsync(CatButtonDisplay button)
        {
            Debug.Write("ButtonTapped");
            if (button == null) return;
            switch (button.ButtonNo)
            {
                case 1:
                    if (button.Image == CatSelectImage.arrowUp)
                        await DisplayCategoryAsync(null);
                    break;
                case 2:
                    if (button.Image == CatSelectImage.arrowUp)
                        await DisplayCategoryAsync(button.Category as MasterCategory);
                    break;
                case 3:
                    if (button.Image == CatSelectImage.arrowUp)
                        await DisplayCategoryAsync(button.Category as Category);
                    break;
            }
        }

        protected void SetButton(byte buttonNo, string title, bool isVisible, 
                        CatSelectImage image = CatSelectImage.none, object category = null)
        {
            if (buttonNo < 1 && buttonNo > 3) return;
            var button = ButtonList[buttonNo - 1];
            button.Title = title;
            button.IsVisible = isVisible;
            button.Image = image;
            button.Category = category;
            OnPropertyChanged("ButtonListViewHeight");
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
                await Select(catTapped, null);
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

                if (((category.ChildCategory?.Count ?? 0) > 0) && (catTapped.Title.IndexOf($"Wszystkie w {category.Title}") == -1))
                {
                    await DisplayCategoryAsync(category); // displaing more
                }
                else
                {
                    category.SelectedChildCategory = null;
                    await Select(catTapped, category); 
                }
            }

            if (catTapped.CategoryObj.GetType() == typeof(ChildCategory))
            {
                Debug.Write("Tapped CategoryObj is ChildCategory");
                var child = catTapped.CategoryObj as ChildCategory;
                await Select(catTapped, child?.ParentCategory, child);
            }
        }

        protected CatDisplay lastSelect;
        protected CatSelectImage lastImageState;
        protected async Task Select(CatDisplay catDisplay, Category selCategory, ChildCategory childCategory = null)
        {
            Debug.Write("Select " + catDisplay?.Title ?? "null");
            if(selCategory == null) Debug.Write("Category is null");
            if (lastSelect != null)
                lastSelect.Image = lastImageState;
            lastSelect = catDisplay;

            lastImageState = catDisplay?.Image ?? CatSelectImage.none;

            LastSelectedCategory = selCategory;
            if(selCategory != null)
                LastSelectedCategory.SelectedChildCategory = childCategory;
            catDisplay.Image = CatSelectImage.selected;

            var navigationParams = new NavigationParameters();
            navigationParams.Add("SelectedCategory", selCategory);
            await _navigationService.GoBackAsync(navigationParams);            
        }

        protected async Task DisplayCategoryAsync(object categoryToShow)
        {
            CategoryAction.Clear();
            // displaying main category
            if (categoryToShow == null)
            {
                Debug.Write("Displaing categoryToShow is null");
                int AllViews = 0;
                SetButton(1, Category.TitleForNull, true);
                SetButton(2, String.Empty, false);
                SetButton(3, String.Empty, false);

                var categoryList = await _rzeszowiakRepository.GetMasterCategoryListAsync();
                foreach(var item in categoryList)
                    AllViews += item.Views;

                CategoryAction.Add(new CatDisplay
                {
                    Title = Category.TitleForNull,
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
            if (categoryToShow != null &&  categoryToShow.GetType() == typeof(MasterCategory))
            {
                Debug.Write("Displaing categoryToShow is MasterCategory");
                var master = categoryToShow as MasterCategory;

                SetButton(1, Category.TitleForNull, true, CatSelectImage.arrowUp, master);
                SetButton(2, master.Title, true, CatSelectImage.none, master);
                SetButton(3, String.Empty, false);

                var categoryList = await _rzeszowiakRepository.GetCategoryListAsync();
                foreach (var category in categoryList)
                    if (master == category.Master)
                    {
                        var catDisplay = new CatDisplay
                            {
                                Title = category.Title,
                                Views = category.Views,
                                Image = (LastSelectedCategory == category) && (category.SelectedChildCategory == null)
                                        && ((category.ChildCategory?.Count ?? 0) == 0) ? CatSelectImage.selected :
                                        ((category.ChildCategory?.Count ?? 0) > 0 ? CatSelectImage.arrowDeeper : CatSelectImage.none),
                                CategoryObj = category,
                            };

                        CategoryAction.Add(catDisplay);
                        if (catDisplay.Image == CatSelectImage.selected)
                        {
                            lastSelect = catDisplay;
                            lastImageState = (category.ChildCategory?.Count ?? 0) > 0 ? CatSelectImage.arrowDeeper : CatSelectImage.none;
                        }    
                    }
            }

            //display childCategory
            if (categoryToShow != null && categoryToShow?.GetType() == typeof(Category))
            {
                Debug.Write("Displaing categoryToShow is Category");
                var category = categoryToShow as Category;

                SetButton(1, Category.TitleForNull, true, CatSelectImage.arrowUp, category.Master);
                SetButton(2, category.Master.Title, true, CatSelectImage.arrowUp, category.Master);
                SetButton(3, category.Title, true, CatSelectImage.none, category);

                var catDisplay = new CatDisplay
                {
                    Title = $"Wszystkie w {category.Title}",
                    Views = category.Views,
                    Image = ((LastSelectedCategory) == category) && (LastSelectedCategory?.SelectedChildCategory == null) ?
                            CatSelectImage.selected : CatSelectImage.none,
                    CategoryObj = category,
                };

                CategoryAction.Add(catDisplay);
                if (catDisplay.Image == CatSelectImage.selected)
                {
                    lastSelect = catDisplay;
                    lastImageState = CatSelectImage.none;
                }
                
                foreach (var child in category.ChildCategory)
                {
                    Debug.Write("End Displaing categoryToShow is MasterCategory");
                    catDisplay = new CatDisplay
                        {
                            Title = child.Title,
                            Views = child.Views,           
                            Image = (LastSelectedCategory == category &&
                                    LastSelectedCategory?.SelectedChildCategory == child) ? 
                                    CatSelectImage.selected : CatSelectImage.none,
                            CategoryObj = child, 
                        };
                    
                    CategoryAction.Add(catDisplay);
                    if (catDisplay.Image == CatSelectImage.selected)
                    {
                        lastSelect = catDisplay;
                        lastImageState = CatSelectImage.none;
                    }
                }
            }
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
        public byte ButtonNo { get; private set; }

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

        public object Category { get; set; }

        public CatButtonDisplay(byte buttonNo)
        {
            ButtonNo = buttonNo;
        }

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
