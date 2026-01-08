using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Exceptions;
using YearInReview.Model.Reports._1970;
using YearInReview.Model.Reports.Persistence;
using YearInReview.Settings.MVVM;

namespace YearInReview.Model.Reports
{
	public class ReportManager
	{
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly IReportPersistence _reportPersistence;
		private readonly IReportGenerator _reportGenerator;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly ISettingsViewModel _settingsViewModel;

		private Dictionary<Guid, PersistedReport> _reportCache = new Dictionary<Guid, PersistedReport>();
		private Report1970 _notPersistedReport;

		public ReportManager(
			IReportPersistence reportPersistence,
			IReportGenerator reportGenerator,
			IDateTimeProvider dateTimeProvider,
			ISettingsViewModel settingsViewModel)
		{
			_reportPersistence = reportPersistence;
			_reportGenerator = reportGenerator;
			_dateTimeProvider = dateTimeProvider;
			_settingsViewModel = settingsViewModel;
		}

		public event Action<IReadOnlyCollection<Report1970>> ReportsGenerated;

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
			Report1970 report;
			if (_reportCache.TryGetValue(id, out var persistedReport))
			{
				report = _reportPersistence.LoadReport(persistedReport.FilePath);
			}
			else if (id == _notPersistedReport?.Metadata.Id)
			{
				report = _notPersistedReport;
			}
			else
			{
				throw new InvalidOperationException($"Report {id} not persisted in cache.");
			}

			return report;
		}

		public async Task<Report1970> GenerateNotPersistedReport(int year)
		{
			_notPersistedReport = await _reportGenerator.Generate(year);
			return _notPersistedReport;
		}

		public Report1970 GetNotPersistedReport(int year)
		{
			return _notPersistedReport;
		}

		public IReadOnlyCollection<PersistedReport> GetAllPreLoadedReports()
		{
			return _reportCache.Values.ToList();
		}

		public async Task<Guid> RegenerateReport(Guid id)
		{
			if (!_reportCache.TryGetValue(id, out var persistedReport))
			{
				throw new InvalidOperationException($"Report {id} not persisted in cache.");
			}

			if (!persistedReport.IsOwn)
			{
				throw new InvalidOperationException($"Cannot regenerate not own report (reportId: {id}).");
			}

			var newReport = await _reportGenerator.Generate(persistedReport.Year);
			_reportPersistence.SaveReport(newReport, _settingsViewModel.Settings.SaveWithImages);

			var reports = _reportPersistence.PreLoadAllReports();
			_reportCache = reports.ToDictionary(x => x.Id, x => x);

			_logger.Info($"Report {newReport.Metadata.Id} regenerated for year {newReport.Metadata.Year}.");
			return newReport.Metadata.Id;
		}

		public void ExportReport(Guid id, string exportPath, bool exportWithImages)
		{
			Report1970 report;
			if (_reportCache.TryGetValue(id, out var persistedReport))
			{
				report = _reportPersistence.LoadReport(persistedReport.FilePath);
			}
			else if (id == _notPersistedReport?.Metadata.Id)
			{
				report = _notPersistedReport;
			}
			else
			{
				throw new InvalidOperationException($"Report {id} not persisted in cache.");
			}

			_reportPersistence.ExportReport(report, exportPath, exportWithImages);
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

		private async Task<IReadOnlyCollection<PersistedReport>> GenerateAll()
		{
			var generatedReports = await _reportGenerator.GenerateAllYears();
			foreach (var report in generatedReports)
			{
				_reportPersistence.SaveReport(report, _settingsViewModel.Settings.SaveWithImages);
			}

			OnReportsGenerated(generatedReports);

			var reports = _reportPersistence.PreLoadAllReports();
			return reports;
		}

		private async Task<IReadOnlyCollection<PersistedReport>> GenerateNewYears(IReadOnlyCollection<PersistedReport> reports)
		{
			var mostRecentOwnReport = reports.Where(x => x.IsOwn).OrderByDescending(x => x.Year).FirstOrDefault();
			if (mostRecentOwnReport?.Year < _dateTimeProvider.GetNow().Year - 1)
			{
				var yearsToGenerate = new List<int>();
				for (var i = mostRecentOwnReport.Year + 1; i < _dateTimeProvider.GetNow().Year; i++)
				{
					yearsToGenerate.Add(i);
				}

				var generatedReportsWithEmpty = await Task.WhenAll(yearsToGenerate.Select(year => _reportGenerator.Generate(year)));
				var generatedReports = generatedReportsWithEmpty.Where(x => x != null).ToList();
				foreach (var report in generatedReports)
				{
					_reportPersistence.SaveReport(report, _settingsViewModel.Settings.SaveWithImages);
				}

				OnReportsGenerated(generatedReports);

				reports = _reportPersistence.PreLoadAllReports();
			}

			return reports;
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

		protected virtual void OnReportsGenerated(IReadOnlyCollection<Report1970> reports)
		{
			if (reports.Count > 0)
			{
				ReportsGenerated?.Invoke(reports);
			}
		}
	}
}