using System;

namespace YearInReview.Model.Aggregators.Data
{
	public class Metadata
	{
		public Guid Id { get; set; }

		public int Year { get; set; }

		public string Username { get; set; }

		public DateTime GeneratedTimestamp { get; set; }
	}
}