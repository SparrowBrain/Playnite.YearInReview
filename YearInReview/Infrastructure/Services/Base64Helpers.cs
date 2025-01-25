using System;

namespace YearInReview.Infrastructure.Services
{
	public static class Base64Helpers
	{
		public static bool IsBase64(string value)
		{
			if (string.IsNullOrEmpty(value)
				|| value.Length % 4 != 0
				|| value.Contains(" ")
				|| value.Contains("\t")
				|| value.Contains("\r")
				|| value.Contains("\n"))
				return false;

			try
			{
				_ = Convert.FromBase64String(value);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}