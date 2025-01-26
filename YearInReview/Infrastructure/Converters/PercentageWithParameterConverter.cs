using System;
using System.Globalization;
using System.Windows.Data;

namespace YearInReview.Infrastructure.Converters
{
	public class PercentageWithParameterConverter : BaseConverter, IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length < 3 || values[0] == null || values[1] == null || values[2] == null)
			{
				return 0;
			}

			if (double.TryParse(values[0].ToString(), out var maxValue)
				&& double.TryParse(values[1].ToString(), out var percentage)
				&& int.TryParse(values[2].ToString(), out var maxValueOffset))
			{
				var intendedValue = (maxValue + maxValueOffset) * percentage;
				return intendedValue > 0 ? intendedValue : 0;
			}

			return 0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}