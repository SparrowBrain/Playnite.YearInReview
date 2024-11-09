using Playnite.SDK;
using System.Collections.Generic;
using System.Linq;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Aggregators
{
	public class MostPlayedGamesAggregator : IMostPlayedGamesAggregator
	{
		private readonly IPlayniteAPI _playniteApi;

		public MostPlayedGamesAggregator(IPlayniteAPI playniteApi)
		{
			_playniteApi = playniteApi;
		}

		public IReadOnlyCollection<GameWithTime> GetMostPlayedGame(IReadOnlyCollection<Activity> activities,
			int gameCount)
		{
			if (activities.Count == 0)
			{
				throw new System.ArgumentException("No activities provided.");
			}

			if (activities.Any(x => x.Items.Count == 0))
			{
				throw new System.ArgumentException("Some activities do not have sessions.");
			}

			if (activities.Any(x => x.Items.Sum(i => (long)i.ElapsedSeconds) == 0))
			{
				throw new System.ArgumentException("Some activities have zero time.");
			}

			var orderedActivities = activities
				.OrderByDescending(x => x.Items.Sum(i => (long)i.ElapsedSeconds))
				.Take(gameCount)
				.ToList();

			return orderedActivities.Select(x => new GameWithTime()
			{
				Game = _playniteApi.Database.Games.FirstOrDefault(g => g.Id == x.Id),
				TimePlayed = x.Items.Sum(i => i.ElapsedSeconds)
			}).ToList();
		}
	}
}