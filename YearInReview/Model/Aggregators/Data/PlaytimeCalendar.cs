using System;
using System.Collections.Generic;

namespace YearInReview.Model.Aggregators.Data
{
	public class PlaytimeCalendar
	{
		public IDictionary<DateTime, CalendarDay> Days { get; set; }
	}
}