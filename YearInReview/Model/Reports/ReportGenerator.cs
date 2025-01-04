using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Playnite.SDK;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Filters;
using YearInReview.Model.Reports._1970;

namespace YearInReview.Model.Reports
{
	public class ReportGenerator : IReportGenerator
	{
		private readonly IPlayniteAPI _playniteApi;
		private readonly IGameActivityExtension _gameActivityExtension;
		private readonly ISpecificYearActivityFilter _specificYearActivityFilter;
		private readonly IComposer1970 _composer1970;

		public ReportGenerator(
			IPlayniteAPI playniteApi,
			IGameActivityExtension gameActivityExtension,
			ISpecificYearActivityFilter specificYearActivityFilter,
			IComposer1970 composer1970)
		{
			_playniteApi = playniteApi;
			_gameActivityExtension = gameActivityExtension;
			_specificYearActivityFilter = specificYearActivityFilter;
			_composer1970 = composer1970;
		}

		public Task<IReadOnlyCollection<Report1970>> GenerateAllYears()
		{
			throw new NotImplementedException();
		}

		public async Task<Report1970> Generate(int year)
		{
			var games = _playniteApi.Database.Games;
			var activities = await _gameActivityExtension.GetActivityForGames(games);
			var filteredActivities = _specificYearActivityFilter.GetActivityForYear(year, activities);

			return _composer1970.Compose(year, filteredActivities);
		}
	}
}