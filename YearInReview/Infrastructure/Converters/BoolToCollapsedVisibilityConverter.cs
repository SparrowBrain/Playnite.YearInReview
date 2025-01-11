using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace YearInReview.Infrastructure.Converters
{
	public class BoolToCollapsedVisibilityConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!bool.TryParse(value?.ToString(), out var visible))
			{
				return Visibility.Collapsed;
			}

			if (parameter != null && bool.TryParse(parameter.ToString(), out var invert) && invert)
			{
				return visible ? Visibility.Collapsed : Visibility.Visible;
			}

			return visible ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}