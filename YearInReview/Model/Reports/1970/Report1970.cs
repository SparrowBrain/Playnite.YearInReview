using System;
using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Reports._1970
{
	public class Report1970
	{
		public Metadata Metadata { get; set; }

		public MostPlayedGame MostPlayedGame { get; set; }
	}

	public class MostPlayedGame
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string CoverImage { get; set; }

		public string FlavourText { get; set; }
	}
}