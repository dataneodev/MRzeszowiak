using GalaSoft.MvvmLight.Command;
using MRzeszowiak.Model;
using MRzeszowiak.Services;
using Rg.Plugins.Popup.Services;
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
        public ObservableCollection<CatButtonDisplay> ButtonList { get; private set; } = new ObservableCollection<CatButtonDisplay>();

        public ICommand CategoryTappet { get; private set; }
        public ICommand ButtonTappped { get; private set; }
        private Action<Category> callbackCategoryResult;

        protected Category LastSelectedCategory { get; set; }

        public short buttonListViewHeight 
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

        public CategorySelectViewModel(IRzeszowiak RzeszowiakRepository)
        {
            rzeszowiakRepository = RzeszowiakRepository;
            ButtonList.Add(new CatButtonDisplay(1) { Title = "", IsVisible = false, Image = CatSelectImage.none });
            ButtonList.Add(new CatButtonDisplay(2) { Title = "", IsVisible = false, Image = CatSelectImage.none });
            ButtonList.Add(new CatButtonDisplay(3) { Title = "", IsVisible = false, Image = CatSelectImage.none });
            CategoryTappet = new Command<CatDisplay>(ItemTapedAsync);
            ButtonTappped = new Command<CatButtonDisplay>(ButtonTappedAsync);

            MessagingCenter.Subscribe<string, Action<Category>>("MRzeszowiak", "SelectCategory", (sender, action) => {
                StartCategorySelect(action);
            });
            DisplayCategoryAsync(null);
        }

        public void StartCategorySelect(Action<Category> callBack)
        {
            callbackCategoryResult = callBack;
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
            OnPropertyChanged("buttonListViewHeight");
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
                Select(catTapped, null);
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
                    Select(catTapped, category);
                }
            }

            if (catTapped.CategoryObj.GetType() == typeof(ChildCategory))
            {
                Debug.Write("Tapped CategoryObj is ChildCategory");
                var child = catTapped.CategoryObj as ChildCategory;
                Select(catTapped, child?.ParentCategory, child);
            }
        }

        protected CatDisplay lastSelect;
        protected CatSelectImage lastImageState;
        protected void Select(CatDisplay catDisplay, Category selCategory, ChildCategory childCategory = null)
        {
            Debug.Write("Select " + catDisplay?.Title ?? "null");

            if (lastSelect != null)
                lastSelect.Image = lastImageState;
            lastSelect = catDisplay;

            lastImageState = catDisplay?.Image ?? CatSelectImage.none;

            LastSelectedCategory = selCategory;
            if(selCategory != null)
                LastSelectedCategory.SelectedChildCategory = childCategory;
            catDisplay.Image = CatSelectImage.selected;

            PopupNavigation.Instance.PopAsync(true);
            callbackCategoryResult?.Invoke(selCategory);            
        }

        protected async Task DisplayCategoryAsync(object categoryToShow)
        {
            CategoryAction.Clear();
            // displaying main category
            if (categoryToShow == null)
            {
                Debug.Write("Displaing categoryToShow is null");
                int AllViews = 0;
                SetButton(1, "Wszystkie kategorie", true);
                SetButton(2, String.Empty, false);
                SetButton(3, String.Empty, false);

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
            if (categoryToShow != null &&  categoryToShow?.GetType() == typeof(MasterCategory))
            {
                Debug.Write("Displaing categoryToShow is MasterCategory");
                var master = categoryToShow as MasterCategory;

                SetButton(1, "Wszystkie kategorie", true, CatSelectImage.arrowUp, master);
                SetButton(2, master.Title, true, CatSelectImage.none, master);
                SetButton(3, String.Empty, false);

                var categoryList = await rzeszowiakRepository.GetCategoryListAsync();
                foreach (var category in categoryList)
                    if (master.Id == category.Master.Id)
                    {
                        var catDisplay = new CatDisplay
                            {
                                Title = category.Title,
                                Views = category.Views,
                                Image = ((LastSelectedCategory?.Id ?? 0) == category.Id) && (category.SelectedChildCategory == null)
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

                SetButton(1, "Wszystkie kategorie", true, CatSelectImage.arrowUp, category.Master);
                SetButton(2, category.Master.Title, true, CatSelectImage.arrowUp, category.Master);
                SetButton(3, category.Title, true, CatSelectImage.none, category);

                var catDisplay = new CatDisplay
                {
                    Title = $"Wszystkie w {category.Title}",
                    Views = category.Views,
                    Image = ((LastSelectedCategory?.Id ?? 0) == category.Id) && (LastSelectedCategory?.SelectedChildCategory == null) ?
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
                    catDisplay = new CatDisplay
                        {
                            Title = child.Title,
                            Views = child.Views,           
                            Image = ((LastSelectedCategory?.Id ?? 0) == category.Id &&
                                    (LastSelectedCategory?.SelectedChildCategory?.ID ?? String.Empty) == child.ID) ? CatSelectImage.selected : CatSelectImage.none,
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

            MessagingCenter.Send<string>("MRzeszowiak", "MoveToTop");
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
