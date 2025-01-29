using System;
using System.IO;

namespace YearInReview.Infrastructure.Services
{
	public static class UriToPhysicalUriConverter
	{
		public static Uri ConvertUriToPhysicalUri(object value)
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

			return uri;
		}
	}
}