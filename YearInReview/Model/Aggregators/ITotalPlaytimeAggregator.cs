using System.Collections.Generic;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.Model.Aggregators
{
	public interface ITotalPlaytimeAggregator
	{
		int GetTotalPlaytime(IReadOnlyCollection<Activity> activities);
	}
}