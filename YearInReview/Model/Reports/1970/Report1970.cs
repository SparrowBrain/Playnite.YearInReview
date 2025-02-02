﻿using System.Collections.Generic;
using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Reports._1970
{
	public class Report1970
	{
		public Metadata Metadata { get; set; }

		public int TotalPlaytime { get; set; }

		public IReadOnlyList<ReportGameWithTime> MostPlayedGames { get; set; }

		public IReadOnlyList<ReportSourceWithTime> MostPlayedSources { get; set; }

		public IReadOnlyList<ReportCalendarDay> PlaytimeCalendarDays { get; set; }

		public IReadOnlyList<ReportHourlyPlaytime> HourlyPlaytime { get; set; }

		public int AddedGamesCount { get; set; }

		public IReadOnlyList<ReportAddedGame> NotableAddedGames { get; set; }
	}
}