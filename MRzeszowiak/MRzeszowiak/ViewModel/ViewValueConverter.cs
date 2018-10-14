using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.ViewModel
{
    public class RowColorFromBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as AdvertShort;
            if (item == null)
                return App.normalRow;
            if (item.RowEven)
            {
                if (item.Highlighted)
                    return App.normalPremiumRow;
                else
                    return App.normalRow;
            }
            else
            {
                if (item.Highlighted)
                    return App.highlightPremiumRow;
                else
                    return App.highlightRow;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class ImageFromFavorite : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            { return "star_yellow.png"; }
            else
            { return "star_gray.png"; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class AbsHeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            { return 220; }
            else
            { return 55; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class IsValueNotNull : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            { return true; }
            else
            { return false; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class IsTextNotNull : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || ((value as string)?.Length ?? 0) == 0)
            { return false; }
            else
            { return true; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class CatSelectImageToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CatSelectImage image = value == null ? CatSelectImage.none : (CatSelectImage)value;
            switch (image)
            {
                case CatSelectImage.arrowDeeper:
                    return "inside_category.png";
                case CatSelectImage.selected:
                    return "selected_category.png";
                case CatSelectImage.arrowUp:
                    return "up_category.png";
                case CatSelectImage.none:
                    return null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class CatSelectImageToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CatSelectImage image = value == null ? CatSelectImage.none : (CatSelectImage)value;
            if (image == CatSelectImage.none) return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class NegativeBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            { return false; }
            else
            { return true; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;

        }
    }
}
