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
		private readonly IHourlyPlaytimeAggregator _hourlyPlaytimeAggregator;
		private readonly IAddedGamesAggregator _addedGamesAggregator;

		public Composer1970(
			IMetadataProvider metadataProvider,
			ITotalPlaytimeAggregator totalPlaytimeAggregator,
			IMostPlayedGamesAggregator mostPlayedGameAggregator,
			IMostPlayedSourcesAggregator mostPlayedSourcesAggregator,
			IPlaytimeCalendarAggregator playtimeCalendarAggregator,
			IHourlyPlaytimeAggregator hourlyPlaytimeAggregator,
			IAddedGamesAggregator addedGamesAggregator)
		{
			_metadataProvider = metadataProvider;
			_totalPlaytimeAggregator = totalPlaytimeAggregator;
			_mostPlayedGameAggregator = mostPlayedGameAggregator;
			_mostPlayedSourcesAggregator = mostPlayedSourcesAggregator;
			_playtimeCalendarAggregator = playtimeCalendarAggregator;
			_hourlyPlaytimeAggregator = hourlyPlaytimeAggregator;
			_addedGamesAggregator = addedGamesAggregator;
		}

		public Report1970 Compose(int year, IReadOnlyCollection<Activity> activities)
		{
			if (activities.Count == 0)
			{
				return null;
			}

			var mostPlayedGames = _mostPlayedGameAggregator.GetMostPlayedGames(activities, 10);
			var addedGames = _addedGamesAggregator.GetAddedGames(year);

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
				}).ToList(),
				MostPlayedSources = _mostPlayedSourcesAggregator.GetMostPlayedSources(activities).Select(x =>
					new ReportSourceWithTime()
					{
						Id = x.Source.Id,
						Name = x.Source.Name,
						TimePlayed = x.TimePlayed,
					}).ToList(),
				PlaytimeCalendarDays = _playtimeCalendarAggregator.GetCalendar(year, activities).Values.Select(x =>
					new ReportCalendarDay()
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
				HourlyPlaytime = _hourlyPlaytimeAggregator.GetHours(activities).Select(x => new ReportHourlyPlaytime()
				{
					Hour = x.Key,
					Playtime = x.Value
				}).ToList(),
				AddedGamesCount = addedGames.Count,
				NotableAddedGames = addedGames.Where(x => x.Game.CriticScore >= 80)
					.OrderByDescending(x => x.Game.CriticScore).Take(3).Select(x =>
						new ReportAddedGame()
						{
							Id = x.Game.Id,
							Name = x.Game.Name,
							SourceName = x.Source.Name,
							CoverImage = x.Game.CoverImage,
							AddedDate = x.AddedDate,
							CriticScore = x.Game.CriticScore ?? 0
						}).ToList(),
			};
		}
	}
}