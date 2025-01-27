using System;
using System.Globalization;
using System.Windows.Data;

namespace YearInReview.Infrastructure.Converters
{
	public class PercentageWithParameterConverter : BaseConverter, IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length < 2 || values[0] == null || values[1] == null)
			{
				return 0;
			}

			var maxValueOffset = values.Length >= 3 ? values[2] as int? ?? 0 : 0;
			var minValue = values.Length >= 4 ? values[3] as int? ?? 0 : 0;

			if (double.TryParse(values[0].ToString(), out var maxValue)
				&& double.TryParse(values[1].ToString(), out var percentage))
			{
				var intendedValue = (maxValue + maxValueOffset) * percentage;
				return intendedValue > minValue ? intendedValue : minValue;
			}

			return 0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}