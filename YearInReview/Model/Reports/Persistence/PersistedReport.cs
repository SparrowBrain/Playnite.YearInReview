using System;

namespace YearInReview.Model.Reports.Persistence
{
	public class PersistedReport
	{
		public Guid Id { get; set; }

		public bool IsOwn { get; set; }

		public string FilePath { get; set; }

		public string Username { get; set; }

		public int Year { get; set; }

		public int TotalPlaytime { get; set; }
	}
}