using Playnite.SDK;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using YearInReview.Infrastructure.Services;

namespace YearInReview.Infrastructure.Converters
{
	public class Base64OrUriToBitmapImageConverter : BaseConverter, IValueConverter
	{
		private readonly ILogger _logger = LogManager.GetLogger();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string base64String && Base64Helpers.IsBase64(base64String))
			{
				try
				{
					var imageBytes = System.Convert.FromBase64String(base64String);
					return CreateBmp(imageBytes, null, parameter);
				}
				catch (Exception ex)
				{
					_logger.Error(ex, "Failed to load image from base64 string");
				}
			}
			else
			{
				var uri = UriToPhysicalUriConverter.ConvertUriToPhysicalUri(value);
				if (uri != null)
				{
					try
					{
						return CreateBmp(null, uri, parameter);
					}
					catch (Exception ex)
					{
						_logger.Error(ex, $"Failed to load image at {uri.OriginalString}");
					}
				}
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		private static object CreateBmp(byte[] imageBytes, Uri uri, object parameter)
		{
			if (imageBytes != null && uri != null)
			{
				throw new ArgumentException("Both imageBytes and uri cannot be provided at the same time.");
			}

			if (imageBytes == null && uri == null)
			{
				throw new ArgumentException("Either imageBytes or uri must be provided.");
			}

			var bmp = new BitmapImage();
			bmp.BeginInit();
			bmp.CreateOptions = BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.DelayCreation;
			bmp.CacheOption = BitmapCacheOption.OnLoad;
			if (parameter is string maxHeightString && int.TryParse(maxHeightString, out var maxHeight))
			{
				bmp.DecodePixelHeight = maxHeight;
			}

			if (imageBytes != null)
			{
				bmp.StreamSource = new MemoryStream(imageBytes);
			}

			if (uri != null)
			{
				bmp.UriSource = uri;
			}

			bmp.EndInit();
			bmp.Freeze();
			return bmp;
		}
	}
}