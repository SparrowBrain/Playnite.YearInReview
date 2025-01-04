using Playnite.SDK;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

		// TODO what happens when no activity / sessions?
		public async Task<IReadOnlyCollection<Report1970>> GenerateAllYears()
		{
			var games = _playniteApi.Database.Games;
			var activities = await _gameActivityExtension.GetActivityForGames(games);
			var allYears = activities.SelectMany(x => x.Items).Select(x => x.DateSession.Year).Distinct();

			var reports = new List<Report1970>();
			foreach (var year in allYears)
			{
				var filteredActivities = _specificYearActivityFilter.GetActivityForYear(year, activities);
				var report = _composer1970.Compose(year, filteredActivities);
				reports.Add(report);
			}

			return reports;
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