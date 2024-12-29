using System.Collections.Generic;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.Model.Aggregators
{
	public interface IHourlyPlaytimeAggregator
	{
		IDictionary<int, int> GetHours(IReadOnlyCollection<Activity> activities);
	}
}