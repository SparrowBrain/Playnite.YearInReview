using System.Collections.Generic;
using YearInReview.Model.Reports._1970;

namespace YearInReview.Infrastructure.UserControls
{
	public class HourlyPlaytimeViewModel : ObservableObject
	{
		public HourlyPlaytimeViewModel(ReportHourlyPlaytime reportHourlyPlaytime, int maxPlaytime)
		{
			Hour = reportHourlyPlaytime.Hour;
			Playtime = reportHourlyPlaytime.Playtime;
			Height = (float)reportHourlyPlaytime.Playtime / maxPlaytime * 400;
		}

		public int Hour { get; set; }

		public int Playtime { get; set; }

		public float Height { get; set; }
	}
}