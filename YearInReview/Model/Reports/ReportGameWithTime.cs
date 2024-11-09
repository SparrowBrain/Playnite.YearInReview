using System;

namespace YearInReview.Model.Reports
{
	public class ReportGameWithTime
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string CoverImage { get; set; }

		public string FlavourText { get; set; }
		public int TimePlayed { get; set; }
	}
}