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
		private const string NotificationId = "year_in_review_report_generation_";
		
		private readonly IPlayniteAPI _playniteApi;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IGameActivityExtension _gameActivityExtension;
		private readonly ISpecificYearActivityFilter _specificYearActivityFilter;
		private readonly IComposer1970 _composer1970;

		public ReportGenerator(
			IPlayniteAPI playniteApi,
			IDateTimeProvider dateTimeProvider,
			IGameActivityExtension gameActivityExtension,
			ISpecificYearActivityFilter specificYearActivityFilter,
			IComposer1970 composer1970)
		{
			_playniteApi = playniteApi;
			_dateTimeProvider = dateTimeProvider;
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

			var currentYear = _dateTimeProvider.GetNow().Year;
			var reports = new List<Report1970>();
			foreach (var year in allYears.Where(x => x != currentYear))
			{
				var filteredActivities = _specificYearActivityFilter.GetActivityForYear(year, activities);
				var report = _composer1970.Compose(year, filteredActivities);
				reports.Add(report);
				ShowReportGeneratedNotification(year);
			}

			return reports;
		}

		public async Task<Report1970> Generate(int year)
		{
			var games = _playniteApi.Database.Games;
			var activities = await _gameActivityExtension.GetActivityForGames(games);
			var filteredActivities = _specificYearActivityFilter.GetActivityForYear(year, activities);
			
			ShowReportGeneratedNotification(year);

			return _composer1970.Compose(year, filteredActivities);
		}

		private void ShowReportGeneratedNotification(int year)
		{
			var description = string.Format(
				ResourceProvider.GetString("LOC_YearInReview_ReportGeneratedNotification"), 
				year
			);
			
			_playniteApi.Notifications.Add(
				new NotificationMessage(
					NotificationId + year,
					description,
					NotificationType.Info
				)
			);
		}
	}
}