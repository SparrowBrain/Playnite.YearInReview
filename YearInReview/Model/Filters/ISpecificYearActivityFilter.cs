using System.Collections.Generic;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.Model.Filters
{
	public interface ISpecificYearActivityFilter
	{
		IReadOnlyCollection<Activity> GetActivityForYear(int year, IReadOnlyCollection<Activity> allActivities);
	}
}