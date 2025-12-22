using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YearInReview.Infrastructure.Services;

namespace YearInReview.Infrastructure.Serialization
{
	public class Base64ImageConverter : JsonConverter
	{
		private readonly bool _serializeAsBase64;
		private readonly int? _maxWidth;
		private readonly int? _maxHeight;

		public Base64ImageConverter(bool serializeAsBase64 = false, int? maxWidth = null, int? maxHeight = null)
		{
			_serializeAsBase64 = serializeAsBase64;
			_maxWidth = maxWidth;
			_maxHeight = maxHeight;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (_serializeAsBase64 && value is string uri && !string.IsNullOrEmpty(uri) && !Base64Helpers.IsBase64(uri))
			{
				var physicalUri = UriToPhysicalUriConverter.ConvertUriToPhysicalUri(value);
				if (physicalUri == null || !File.Exists(physicalUri.AbsolutePath))
				{
					writer.WriteValue(value);
					return;
				}

				var imageBytes = File.ReadAllBytes(physicalUri.AbsolutePath);

				if (_maxWidth.HasValue || _maxHeight.HasValue)
				{
					using (var ms = new MemoryStream(imageBytes))
					{
						var bitmapImage = new BitmapImage();
						bitmapImage.BeginInit();
						bitmapImage.StreamSource = ms;
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.EndInit();

						var resizedImage = ResizeImage(bitmapImage, _maxWidth, _maxHeight);
						using (var resizedMs = new MemoryStream())
						{
							var encoder = new PngBitmapEncoder();
							encoder.Frames.Add(BitmapFrame.Create(resizedImage));
							encoder.Save(resizedMs);
							imageBytes = resizedMs.ToArray();
						}
					}
				}

				var base64String = Convert.ToBase64String(imageBytes);
				writer.WriteValue(base64String);
			}
			else
			{
				writer.WriteValue(value);
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(string);
		}

		public override bool CanRead => false;

		private BitmapSource ResizeImage(BitmapSource image, int? maxWidth, int? maxHeight)
		{
			var ratioX = maxWidth.HasValue ? (double)maxWidth.Value / image.PixelWidth : double.MaxValue;
			var ratioY = maxHeight.HasValue ? (double)maxHeight.Value / image.PixelHeight : double.MaxValue;
			var ratio = Math.Min(ratioX, ratioY);

			if (ratio >= 1)
			{
				return image;
			}

			var resizedImage = new TransformedBitmap(image, new ScaleTransform(ratio, ratio));
			return resizedImage;
		}
	}
}