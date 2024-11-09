using System.Collections.Generic;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Aggregators
{
	public interface IMostPlayedGamesAggregator
	{
		IReadOnlyCollection<GameWithTime> GetMostPlayedGame(IReadOnlyCollection<Activity> activities, int gameCount);
	}
}