using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Aggregators
{
	public interface IMetadataProvider
	{
		Metadata Get(int year);
	}
}