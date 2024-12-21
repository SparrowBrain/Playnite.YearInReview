﻿using System;
using System.Collections.Generic;

namespace YearInReview.Model.Reports._1970
{
	public class ReportCalendarDay
	{
		public DateTime Date { get; set; }

		public int TotalPlaytime { get; set; }

		public IReadOnlyList<ReportCalendarGame> Games { get; set; }
	}
}