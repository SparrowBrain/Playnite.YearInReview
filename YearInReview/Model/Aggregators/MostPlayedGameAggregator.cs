using System.Collections.Generic;
using System.Linq;
using Playnite.SDK;
using Playnite.SDK.Models;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.Model.Aggregators
{
	public class MostPlayedGameAggregator : IMostPlayedGameAggregator
	{
		private readonly IPlayniteAPI _playniteApi;

		public MostPlayedGameAggregator(IPlayniteAPI playniteApi)
		{
			_playniteApi = playniteApi;
		}

		public Game GetMostPlayedGame(IReadOnlyCollection<Activity> activities)
		{
			var biggestActivity = activities.OrderByDescending(x => x.Items.Sum(i => (long)i.ElapsedSeconds)).FirstOrDefault();
			if (biggestActivity == null)
			{
				return null;
			}

			return _playniteApi.Database.Games.FirstOrDefault(x => x.Id == biggestActivity.Id);
		}
	}
}