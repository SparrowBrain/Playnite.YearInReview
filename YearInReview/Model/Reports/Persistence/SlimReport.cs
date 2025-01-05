using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Reports.Persistence
{
	public class SlimReport : ISlimReport
	{
		public Metadata Metadata { get; set; }

		public int TotalPlaytime { get; set; }
	}
}