using Playnite.SDK;
using System;
using System.Text;

namespace YearInReview.Infrastructure.Services
{
	public class ReadableTimeFormatter
	{
		public static string FormatTime(int seconds)
		{
			if (seconds < 60)
			{
				return string.Format(ResourceProvider.GetString("LOC_YearInReview_ReadableTime_SecondsOnly"), seconds);
			}

			var timeSpan = TimeSpan.FromSeconds(seconds);
			var readableText = new StringBuilder();

			if (timeSpan.Days > 0)
			{
				readableText.Append(string.Format(ResourceProvider.GetString("LOC_YearInReview_ReadableTime_DayPart"), timeSpan.Days));
				readableText.Append(" ");
			}
			if (timeSpan.Hours > 0)
			{
				readableText.Append(string.Format(ResourceProvider.GetString("LOC_YearInReview_ReadableTime_HourPart"), timeSpan.Hours));
				readableText.Append(" ");
			}
			if (timeSpan.Minutes > 0)
			{
				readableText.Append(string.Format(ResourceProvider.GetString("LOC_YearInReview_ReadableTime_MinutePart"), timeSpan.Minutes));
			}

			return readableText.ToString().Trim();
		}
	}
}