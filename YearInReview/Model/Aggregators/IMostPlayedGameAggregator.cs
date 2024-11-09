using System.Collections.Generic;
using Playnite.SDK.Models;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.Model.Aggregators
{
	public interface IMostPlayedGameAggregator
	{
		Game GetMostPlayedGame(IReadOnlyCollection<Activity> activities);
	}
}