using System;

namespace YearInReview.Model.Reports._1970
{
	public class ReportAddedGame
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string SourceName { get; set; }

		public string CoverImage { get; set; }

		public DateTime AddedDate { get; set; }

		public int CriticScore { get; set; }
	}
}