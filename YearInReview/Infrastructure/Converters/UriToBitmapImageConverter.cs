using Playnite.SDK;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace YearInReview.Infrastructure.Converters
{
	public class UriToBitmapImageConverter : BaseConverter, IValueConverter
	{
		private readonly ILogger _logger = LogManager.GetLogger(nameof(UriToBitmapImageConverter));

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Uri uri = null;

			if (value is Uri imageUri)
			{
				uri = imageUri;
			}
			else if (value is string uriString
					 && uriString.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
					 && Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var imageUri2))
			{
				uri = imageUri2;
			}
			else if (value is string databasePath)
			{
				if (File.Exists(databasePath))
				{
					uri = new Uri(databasePath);
				}
				else
				{
					var localPath = YearInReview.Api.Database.GetFullFilePath(databasePath);
					if (File.Exists(localPath))
					{
						uri = new Uri(localPath);
					}
				}
			}

			if (uri != null)
			{
				try
				{
					var bmp = new BitmapImage();
					bmp.BeginInit();
					bmp.CreateOptions = BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.DelayCreation;
					bmp.CacheOption = BitmapCacheOption.OnLoad;
					if (parameter is string maxHeightString && int.TryParse(maxHeightString, out var maxHeight))
					{
						bmp.DecodePixelHeight = maxHeight;
					}
					bmp.UriSource = uri;
					bmp.EndInit();
					bmp.Freeze();
					return bmp;
				}
				catch (Exception ex)
				{
					_logger.Error(ex, $"Failed to load image at {uri.OriginalString}");
				}
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}