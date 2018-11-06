using MRzeszowiak.Extends;
using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public class IsCollectionNotEmpty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection<string> lista)
                if (lista.Count > 0) return true;
            if (value is ICollection<KeyValue> lista2)
                if (lista2.Count > 0) return true;
            return false;
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

    public class MailStatusEnumToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MailStatusEnum image = value == null ? MailStatusEnum.email_default : (MailStatusEnum)value;
            switch (image)
            {
                case MailStatusEnum.email_default:
                    return "mail.png";
                case MailStatusEnum.email_creating:
                    return "mail_create.png";
                case MailStatusEnum.email_send:
                    return "mail_send.png";
                case MailStatusEnum.email_sending:
                    goto case MailStatusEnum.email_creating;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class MailStatusEnumNotSending : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MailStatusEnum check = value == null ? MailStatusEnum.email_default : (MailStatusEnum)value;
            if (check != MailStatusEnum.email_sending) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class DateTimeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
                if(dt.IsSend())
                    return dt.GetDateTimeFormated();
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
