using System.Collections.Generic;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.Model.Filters
{
	public interface IEmptyActivityFilter
	{
		IReadOnlyCollection<Activity> RemoveEmpty(IReadOnlyCollection<Activity> activities);
	}
}