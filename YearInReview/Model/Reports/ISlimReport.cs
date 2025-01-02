using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Reports
{
	public interface ISlimReport
	{
		Metadata Metadata { get; set; }

		int TotalPlaytime { get; set; }
	}
}