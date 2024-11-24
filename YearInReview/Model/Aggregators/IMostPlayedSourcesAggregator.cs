using System.Collections.Generic;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators.Data;
using YearInReview.Model.Reports;

namespace YearInReview.Model.Aggregators
{
	public interface IMostPlayedSourcesAggregator
	{
		IReadOnlyCollection<SourceWithTime> GetMostPlayedSources(IReadOnlyCollection<Activity> activities);
	}
}