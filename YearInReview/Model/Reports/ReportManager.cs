﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Playnite.SDK;
using YearInReview.Infrastructure.Serialization;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Exceptions;
using YearInReview.Model.Reports._1970;
using YearInReview.Model.Reports.Persistence;
using YearInReview.Settings;

namespace YearInReview.Model.Reports
{
	public class ReportManager
	{
		private const string ReportNotificationId = "year_in_review_report_generation";
		
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly IReportPersistence _reportPersistence;
		private readonly IReportGenerator _reportGenerator;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IPlayniteAPI _playniteApi;
		private readonly YearInReviewSettings _settings;

		private Dictionary<Guid, PersistedReport> _reportCache = new Dictionary<Guid, PersistedReport>();

		public ReportManager(
			IReportPersistence reportPersistence,
			IReportGenerator reportGenerator,
			IDateTimeProvider dateTimeProvider,
			IPlayniteAPI playniteApi,
			YearInReviewSettings settings)
		{
			_reportPersistence = reportPersistence;
			_reportGenerator = reportGenerator;
			_dateTimeProvider = dateTimeProvider;
			_playniteApi = playniteApi;
			_settings = settings;
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

		public Report1970 GetReport(Guid id)
		{
			if (!_reportCache.TryGetValue(id, out var persistedReport))
			{
				throw new Exception($"Report {id} not persisted in cache.");
			}

			var report = _reportPersistence.LoadReport(persistedReport.FilePath);
			return report;
		}

		private async Task<IReadOnlyCollection<PersistedReport>> GenerateAll()
		{
			var generatedReports = await _reportGenerator.GenerateAllYears();
			foreach (var report in generatedReports)
			{
				_reportPersistence.SaveReport(report);
			}

			ShowReportsGeneratedNotification(generatedReports);

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

				ShowReportsGeneratedNotification(generatedReports);
				
				reports = _reportPersistence.PreLoadAllReports();
			}

			return reports;
		}

		public IReadOnlyCollection<PersistedReport> GetAllPreLoadedReports()
		{
			return _reportCache.Values.ToList();
		}

		public void ExportReport(Guid id, string exportPath, JsonSerializerSettings serializerSettings)
		{
			if (!_reportCache.TryGetValue(id, out var persistedReport))
			{
				throw new InvalidOperationException($"Report {id} not persisted in cache.");
			}

			var report = _reportPersistence.LoadReport(persistedReport.FilePath);
			_reportPersistence.ExportReport(report, exportPath, serializerSettings);
			_logger.Info($"Report {id} exported to \"{exportPath}\"");
		}

		public Guid ImportReport(Report1970 report)
		{
			ValidateReport(report);

			var persistedReport = _reportPersistence.ImportReport(report);
			var reports = _reportPersistence.PreLoadAllReports();
			_reportCache = reports.ToDictionary(x => x.Id, x => x);

			_logger.Info($"Report {persistedReport.Id} from {persistedReport.Username} imported to \"{persistedReport.FilePath}\"");
			return persistedReport.Id;
		}

		private void ValidateReport(Report1970 report)
		{
			if (report.Metadata == null
				|| report.Metadata.Id == Guid.Empty
				|| report.Metadata.Year == 0
				|| string.IsNullOrEmpty(report.Metadata.Username))
			{
				throw new InvalidReportFileException("Trying to import invalid report file.");
			}

			if (_reportCache.ContainsKey(report.Metadata.Id))
			{
				throw new ReportAlreadyExistsException($"Trying to import report {report.Metadata.Id} that is already in cache.");
			}
		}

		private void ShowReportsGeneratedNotification(IReadOnlyCollection<Report1970> reports)
		{
			if (reports.Count == 0 || !_settings.ShowNewReportNotifications)
			{
				return;
			}
			
			var lastReport = reports.OrderByDescending(x => x.Metadata.Year).First();
			
			var description = string.Format(
				ResourceProvider.GetString("LOC_YearInReview_Notification_ReportGenerated"), 
				lastReport.Metadata.Year
			);
			
			_playniteApi.Notifications.Add(
				new NotificationMessage(
					ReportNotificationId,
					description,
					NotificationType.Info
				)
			);
		}
	}
}