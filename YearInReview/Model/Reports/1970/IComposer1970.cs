using System.Collections.Generic;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.Model.Reports._1970
{
	public interface IComposer1970
	{
		Report1970 Compose(int year, IReadOnlyCollection<Activity> activities);
	}
}