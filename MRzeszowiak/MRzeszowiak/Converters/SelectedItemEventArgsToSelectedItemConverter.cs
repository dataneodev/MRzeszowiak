using System;
using System.Globalization;
using Xamarin.Forms;

namespace MRzeszowiak.Converters
{
	public class SelectedItemEventArgsToSelectedItemConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			var eventArgs = value as ItemTappedEventArgs;
            if (eventArgs == null)
                throw new ArgumentException("Expected eventArgs as value", "value");
            return eventArgs.Item;
		}

		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException ();
		}
	}
}

