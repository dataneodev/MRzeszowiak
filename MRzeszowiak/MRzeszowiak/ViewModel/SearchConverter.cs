using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    public class AddTypeToStringTranslate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AddType covert = (value is AddType) ? (AddType)value : AddType.all;
            switch (covert)
            {
                case AddType.all:
                    return "ostatnie 30dni";
                case AddType.last14days:
                    return "ostatnie 14dni";
                case AddType.last7days:
                    return "ostatnie 7dni";
                case AddType.last3days:
                    return "ostatnie 3dni";
                case AddType.last24h:
                    return "ostatnie 24h";
                default:
                    goto case AddType.all;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class SortTypeToStringTranslate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SortType covert = (value is SortType) ? (SortType)value : SortType.dateadd;
            switch (covert)
            {
                case SortType.dateadd:
                    return "data - rosnąco";
                case SortType.dateaddDesc:
                    return "data - malejąco";
                case SortType.price:
                    return "cena - rosnąco";
                case SortType.priceDesc:
                    return "cena - malejąco";
                default:
                    goto case SortType.dateadd;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class CategoryToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           if(value is Category category)
                return category.getFullTitle;
           return Category.TitleForNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class CategoryToActiveControls : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class IntToEntryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return String.Empty;
            if((int)value == 0) return String.Empty;
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value as String;
            if (input == null || input.Length == 0 || input == "0") return null;
            return input;
        }
    }
}
