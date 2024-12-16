using System;
using System.Collections.Generic;
using System.Linq;
using YearInReview.Model.Reports._1970;

namespace YearInReview.Infrastructure.UserControls
{
	public class CalendarDayViewModel : ObservableObject
	{
		public CalendarDayViewModel(ReportCalendarDay day, int maxPlaytime)
		{
			Date = day.Date;
			TotalPlaytime = day.TotalPlaytime;
			Games = day.Games.Select(a => new ReportCalendarGame() { Id = a.Id, Name = a.Name, TimePlayed = a.TimePlayed }).ToList();
			Opacity = TotalPlaytime > 0 ? TotalPlaytime / (float)maxPlaytime * 0.8f + 0.2f : 0;
		}

		public DateTime Date { get; }

		public int TotalPlaytime { get; }

		public IReadOnlyCollection<ReportCalendarGame> Games { get; }

		public float Opacity { get; }
	}
}