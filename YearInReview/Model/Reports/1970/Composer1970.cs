using System.Collections.Generic;
using System.Linq;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators;

namespace YearInReview.Model.Reports._1970
{
	public class Composer1970 : IComposer1970
	{
		private readonly IMetadataProvider _metadataProvider;
		private readonly IMostPlayedGamesAggregator _mostPlayedGameAggregator;

		public Composer1970(
			IMetadataProvider metadataProvider,
			IMostPlayedGamesAggregator mostPlayedGameAggregator)
		{
			_metadataProvider = metadataProvider;
			_mostPlayedGameAggregator = mostPlayedGameAggregator;
		}

		public Report1970 Compose(int year, IReadOnlyCollection<Activity> activities)
		{
			var mostPlayedGames = _mostPlayedGameAggregator.GetMostPlayedGame(activities, 10);

			return new Report1970()
			{
				Metadata = _metadataProvider.Get(year),

				MostPlayedGames = mostPlayedGames.Select(x => new ReportGameWithTime()
				{
					Id = x.Game.Id,
					Name = x.Game.Name,
					CoverImage = x.Game.CoverImage,
					TimePlayed = x.TimePlayed,
					FlavourText = "Most played game of the year"
				}).ToList()
			};
		}
	}
}