using System;
using System.Collections.Generic;

namespace YearInReview.Model.Reports
{
	public class ReportCalendarDay
	{
		public DateTime Date { get; set; }

		public int TotalPlaytime { get; set; }

		public IReadOnlyList<ReportCalendarGame> Games { get; set; }
	}
}