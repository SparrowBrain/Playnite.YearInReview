using Playnite.SDK;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YearInReview.Extensions.GameActivity;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Filters;
using YearInReview.Model.Reports._1970;

namespace YearInReview.Model.Reports
{
	public class ReportGenerator : IReportGenerator
	{
		private readonly IPlayniteAPI _playniteApi;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IGameActivityExtension _gameActivityExtension;
		private readonly ISpecificYearActivityFilter _specificYearActivityFilter;
		private readonly IEmptyActivityFilter _emptyActivityFilter;
		private readonly IComposer1970 _composer1970;

		public ReportGenerator(
			IPlayniteAPI playniteApi,
			IDateTimeProvider dateTimeProvider,
			IGameActivityExtension gameActivityExtension,
			ISpecificYearActivityFilter specificYearActivityFilter,
			IEmptyActivityFilter emptyActivityFilter,
			IComposer1970 composer1970)
		{
			_playniteApi = playniteApi;
			_dateTimeProvider = dateTimeProvider;
			_gameActivityExtension = gameActivityExtension;
			_specificYearActivityFilter = specificYearActivityFilter;
			_emptyActivityFilter = emptyActivityFilter;
			_composer1970 = composer1970;
		}

		public async Task<IReadOnlyCollection<Report1970>> GenerateAllYears()
		{
			var games = _playniteApi.Database.Games;
			var activities = await _gameActivityExtension.GetActivityForGames(games);
			var allYears = activities.SelectMany(x => x.Items).Select(x => x.DateSession.Year).Distinct();

			var currentYear = _dateTimeProvider.GetNow().Year;
			var reports = new List<Report1970>();
			foreach (var year in allYears.Where(x => x != currentYear))
			{
				var specificYearActivities = _specificYearActivityFilter.GetActivityForYear(year, activities);
				var nonEmptyActivities = _emptyActivityFilter.RemoveEmpty(specificYearActivities);
				var report = _composer1970.Compose(year, nonEmptyActivities);
				reports.Add(report);
			}

			return reports;
		}

		public async Task<Report1970> Generate(int year)
		{
			var games = _playniteApi.Database.Games;
			var activities = await _gameActivityExtension.GetActivityForGames(games);
			var specificYearActivities = _specificYearActivityFilter.GetActivityForYear(year, activities);
			var nonEmptyActivities = _emptyActivityFilter.RemoveEmpty(specificYearActivities);

			return _composer1970.Compose(year, nonEmptyActivities);
		}
	}
}