using System.Collections.Generic;
using System.Linq;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.Model.Filters
{
	public class EmptyActivityFilter : IEmptyActivityFilter
	{
		public IReadOnlyCollection<Activity> RemoveEmpty(IReadOnlyCollection<Activity> activities)
		{
			return activities.Where(x => x.Items.Sum(i => (long)i.ElapsedSeconds) > 0).ToList();
		}
	}
}