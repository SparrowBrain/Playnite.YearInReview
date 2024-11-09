using System;
using System.Globalization;
using System.Windows.Data;
using YearInReview.Infrastructure.Services;

namespace YearInReview.Infrastructure.Converters
{
	internal class SecondsToReadableTextConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int seconds)
			{
				return ReadableTimeFormatter.FormatTime(seconds);
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}