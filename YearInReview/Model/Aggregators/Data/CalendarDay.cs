using System;
using System.Collections.Generic;

namespace YearInReview.Model.Aggregators.Data
{
	public class CalendarDay
	{
		public DateTime Date { get; set; }

		public int TotalPlaytime { get; set; }

		public List<GameWithTime> Games { get; set; }
	}
}