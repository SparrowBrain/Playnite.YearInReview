using System.Collections.Generic;
using System.Linq;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.Model.Aggregators
{
	public class TotalPlaytimeAggregator : ITotalPlaytimeAggregator
	{
		public int GetTotalPlaytime(IReadOnlyCollection<Activity> activities)
		{
			return activities.Sum(a => a.Items.Sum(s => s.ElapsedSeconds));
		}
	}
}