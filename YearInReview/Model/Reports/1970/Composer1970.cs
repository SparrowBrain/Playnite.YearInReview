using System.Collections.Generic;
using System.Linq;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators;

namespace YearInReview.Model.Reports._1970
{
	public class Composer1970 : IComposer1970
	{
		private readonly IMetadataProvider _metadataProvider;
		private readonly ITotalPlaytimeAggregator _totalPlaytimeAggregator;
		private readonly IMostPlayedGamesAggregator _mostPlayedGameAggregator;
		private readonly IMostPlayedSourcesAggregator _mostPlayedSourcesAggregator;

		public Composer1970(
			IMetadataProvider metadataProvider,
			ITotalPlaytimeAggregator totalPlaytimeAggregator,
			IMostPlayedGamesAggregator mostPlayedGameAggregator,
			IMostPlayedSourcesAggregator mostPlayedSourcesAggregator)
		{
			_metadataProvider = metadataProvider;
			_totalPlaytimeAggregator = totalPlaytimeAggregator;
			_mostPlayedGameAggregator = mostPlayedGameAggregator;
			_mostPlayedSourcesAggregator = mostPlayedSourcesAggregator;
		}

		public Report1970 Compose(int year, IReadOnlyCollection<Activity> activities)
		{
			var mostPlayedGames = _mostPlayedGameAggregator.GetMostPlayedGames(activities, 10);

			return new Report1970()
			{
				Metadata = _metadataProvider.Get(year),
				TotalPlaytime = _totalPlaytimeAggregator.GetTotalPlaytime(activities),
				MostPlayedGames = mostPlayedGames.Select(x => new ReportGameWithTime()
				{
					Id = x.Game.Id,
					Name = x.Game.Name,
					CoverImage = x.Game.CoverImage,
					TimePlayed = x.TimePlayed,
					FlavourText = "Most played game of the year"
				}).ToList(),
				MostPlayedSources = _mostPlayedSourcesAggregator.GetMostPlayedSources(activities).Select(x => new ReportSourceWithTime()
				{
					Id = x.Source.Id,
					Name = x.Source.Name,
					TimePlayed = x.TimePlayed,
				}).ToList(),
			};
		}
	}
}