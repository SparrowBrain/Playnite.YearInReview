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
		private readonly IPlaytimeCalendarAggregator _playtimeCalendarAggregator;

		public Composer1970(
			IMetadataProvider metadataProvider,
			ITotalPlaytimeAggregator totalPlaytimeAggregator,
			IMostPlayedGamesAggregator mostPlayedGameAggregator,
			IMostPlayedSourcesAggregator mostPlayedSourcesAggregator,
			IPlaytimeCalendarAggregator playtimeCalendarAggregator)
		{
			_metadataProvider = metadataProvider;
			_totalPlaytimeAggregator = totalPlaytimeAggregator;
			_mostPlayedGameAggregator = mostPlayedGameAggregator;
			_mostPlayedSourcesAggregator = mostPlayedSourcesAggregator;
			_playtimeCalendarAggregator = playtimeCalendarAggregator;
		}

		public Report1970 Compose(int year, IReadOnlyCollection<Activity> activities)
		{
			var mostPlayedGames = _mostPlayedGameAggregator.GetMostPlayedGames(activities, 25);

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
				PlaytimeCalendarDays = _playtimeCalendarAggregator.GetCalendar(year, activities).Values.Select(x => new ReportCalendarDay()
				{
					Date = x.Date,
					TotalPlaytime = x.TotalPlaytime,
					Games = x.Games.Select(g => new ReportCalendarGame()
					{
						Id = g.Game.Id,
						Name = g.Game.Name,
						TimePlayed = g.TimePlayed
					}).ToList()
				}).ToList(),
			};
		}
	}
}