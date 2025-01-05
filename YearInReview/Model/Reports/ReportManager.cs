using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YearInReview.Extensions.GameActivity;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Filters;
using YearInReview.Model.Reports._1970;
using YearInReview.Model.Reports.Persistence;

namespace YearInReview.Model.Reports
{
	public class ReportManager
	{
		private readonly IPlayniteAPI _playniteApi;
		private readonly IReportPersistence _reportPersistence;
		private readonly IReportGenerator _reportGenerator;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IComposer1970 _composer1970;
		private readonly IGameActivityExtension _gameActivityExtension;
		private readonly ISpecificYearActivityFilter _specificYearActivityFilter;

		// TODO Consider using concurrent dictionary
		private Dictionary<Guid, PersistedReport> _reportCache;

		public ReportManager(
			IPlayniteAPI playniteApi,
			IReportPersistence reportPersistence,
			IReportGenerator reportGenerator,
			IDateTimeProvider dateTimeProvider,
			IComposer1970 composer1970,
			IGameActivityExtension gameActivityExtension,
			ISpecificYearActivityFilter specificYearActivityFilter)
		{
			_playniteApi = playniteApi;
			_reportPersistence = reportPersistence;
			_reportGenerator = reportGenerator;
			_dateTimeProvider = dateTimeProvider;
			_composer1970 = composer1970;
			_gameActivityExtension = gameActivityExtension;
			_specificYearActivityFilter = specificYearActivityFilter;
		}

		public async Task<Report1970> GetReport(Guid year)
		{
			var activities = await _gameActivityExtension.GetActivityForGames(_playniteApi.Database.Games.ToList());
			var filteredActivities = _specificYearActivityFilter.GetActivityForYear(year, activities);

			return _composer1970.Compose(year, filteredActivities);
		}

		// TODO Don't forget. There's probably gonna be race condition between report manager and UI.
		public async Task Init()
		{
			var reports = _reportPersistence.PreLoadAllReports();
			if (!reports.Any())
			{
				reports = await GenerateAll();
			}
			else
			{
				reports = await GenerateNewYears(reports);
			}

			_reportCache = reports.ToDictionary(x => x.Id, x => x);
		}

		private async Task<IReadOnlyCollection<PersistedReport>> GenerateAll()
		{
			var generatedReports = await _reportGenerator.GenerateAllYears();
			foreach (var report in generatedReports)
			{
				_reportPersistence.SaveReport(report);
			}

			var reports = _reportPersistence.PreLoadAllReports();
			return reports;
		}

		private async Task<IReadOnlyCollection<PersistedReport>> GenerateNewYears(
			IReadOnlyCollection<PersistedReport> reports)
		{
			var mostRecentOwnReport = reports.Where(x => x.IsOwn).OrderByDescending(x => x.Year).FirstOrDefault();
			if (mostRecentOwnReport?.Year < _dateTimeProvider.GetNow().Year - 1)
			{
				var yearsToGenerate = new List<int>();
				for (var i = mostRecentOwnReport.Year + 1; i < _dateTimeProvider.GetNow().Year; i++)
				{
					yearsToGenerate.Add(i);
				}

				var generatedReports = await Task.WhenAll(yearsToGenerate.Select(year => _reportGenerator.Generate(year)));
				foreach (var report in generatedReports)
				{
					_reportPersistence.SaveReport(report);
				}

				reports = _reportPersistence.PreLoadAllReports();
			}

			return reports;
		}

		public IReadOnlyCollection<PersistedReport> GetAllPreLoadedReports()
		{
			return _reportCache.Values.ToList();
		}
	}
}