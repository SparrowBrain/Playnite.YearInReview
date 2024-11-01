using System.IO;

namespace YearInReview.IntegrationTests
{
	internal class Utils
	{
		public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
		{
			var dir = new DirectoryInfo(sourceDir);
			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
			}

			if (!Directory.Exists(destinationDir))
			{
				Directory.CreateDirectory(destinationDir);
			}

			foreach (var file in dir.GetFiles())
			{
				var targetFilePath = Path.Combine(destinationDir, file.Name);
				if (!File.Exists(targetFilePath))
				{
					file.CopyTo(targetFilePath);
				}
			}

			var dirs = dir.GetDirectories();
			if (recursive)
			{
				foreach (var subDir in dirs)
				{
					var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
					CopyDirectory(subDir.FullName, newDestinationDir, true);
				}
			}
		}
	}
}