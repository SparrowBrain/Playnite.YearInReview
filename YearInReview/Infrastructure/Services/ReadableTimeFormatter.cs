using Playnite.SDK;
using System;
using System.Text;

namespace YearInReview.Infrastructure.Services
{
	public static class ReadableTimeFormatter
	{
		private const string NonBreakableSpace = "\u00A0";
		private const string BreakableSpace = " ";

		public static bool DisplayLargeTimeInHours { get; set; }

		public static string FormatTime(int seconds, bool nonLineBreaking = false)
		{
			if (seconds < 60)
			{
				return string.Format(ResourceProvider.GetString("LOC_YearInReview_ReadableTime_SecondsOnly"), seconds);
			}

			var timeSpan = TimeSpan.FromSeconds(seconds);
			var readableText = new StringBuilder();

			if (DisplayLargeTimeInHours)
			{
				if (Math.Floor(timeSpan.TotalHours) > 0)
				{
					readableText.Append(string.Format(
						ResourceProvider.GetString("LOC_YearInReview_ReadableTime_HourPart"),
						Math.Floor(timeSpan.TotalHours)));
					readableText.Append(BreakableSpace);
				}
			}
			else
			{
				if (timeSpan.Days > 0)
				{
					readableText.Append(string.Format(
						ResourceProvider.GetString("LOC_YearInReview_ReadableTime_DayPart"), timeSpan.Days));
					readableText.Append(BreakableSpace);
				}

				if (timeSpan.Hours > 0)
				{
					readableText.Append(string.Format(
						ResourceProvider.GetString("LOC_YearInReview_ReadableTime_HourPart"), timeSpan.Hours));
					readableText.Append(BreakableSpace);
				}
			}

			if (timeSpan.Minutes > 0)
			{
				readableText.Append(string.Format(ResourceProvider.GetString("LOC_YearInReview_ReadableTime_MinutePart"), timeSpan.Minutes));
			}

			// Replace breakable spaces with non-breakable spaces, if nonLineBreaking is set
			if (nonLineBreaking)
			{
				readableText.Replace(BreakableSpace, NonBreakableSpace);
			}

			return readableText.ToString().Trim();
		}
	}
}