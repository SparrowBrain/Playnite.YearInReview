using System;

namespace YearInReview.Model.Reports._1970
{
	public class ReportGameWithTime
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		[Image]
		public string CoverImage { get; set; }

		public string FlavourText { get; set; }

		public int TimePlayed { get; set; }
	}
}