using System;
using System.Globalization;
using System.Windows.Data;

namespace YearInReview.Infrastructure.Converters
{
	internal class HourToRegionalConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int hour)
			{
				var dateTime = new DateTime(1970, 1, 1, hour, 0, 0);
				return dateTime.ToString("t");
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}