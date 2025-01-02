using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Reports
{
	public class SlimReport : ISlimReport
	{
		public Metadata Metadata { get; set; }

		public int TotalPlaytime { get; set; }
	}
}