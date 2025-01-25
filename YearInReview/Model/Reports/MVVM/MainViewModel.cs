using Newtonsoft.Json;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using YearInReview.Infrastructure.Serialization;
using YearInReview.Model.Exceptions;
using YearInReview.Model.Reports._1970;
using YearInReview.Model.Reports._1970.MVVM;
using YearInReview.Model.Reports.Persistence;
using YearInReview.Settings;

namespace YearInReview.Model.Reports.MVVM
{
	public class MainViewModel : ObservableObject
	{
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly IPlayniteAPI _api;
		private readonly ReportManager _reportManager;
		private readonly IPluginSettingsPersistence _pluginSettingsPersistence;

		private Report1970View _activeReport;
		private ObservableCollection<YearButtonViewModel> _yearButtons = new ObservableCollection<YearButtonViewModel>();
		private ObservableCollection<ReportButtonViewModel> _reportButtons = new ObservableCollection<ReportButtonViewModel>();

		public MainViewModel(IPlayniteAPI api, ReportManager reportManager, IPluginSettingsPersistence pluginSettingsPersistence)
		{
			_api = api;
			_reportManager = reportManager;
			_pluginSettingsPersistence = pluginSettingsPersistence;

			CreateReportButtons();
			DisplayMostRecentUserReport();
		}

		public ObservableCollection<YearButtonViewModel> YearButtons
		{
			get => _yearButtons;
			set => SetValue(ref _yearButtons, value);
		}

		public ObservableCollection<ReportButtonViewModel> ReportButtons
		{
			get => _reportButtons;
			set => SetValue(ref _reportButtons, value);
		}

		public ICommand ExportReport => new RelayCommand(() =>
		{
			var settings = _pluginSettingsPersistence.LoadPluginSettings<YearInReviewSettings>();
			switch (settings.ExportWithImages)
			{
				case RememberedChoice.Ask:
					// show dialog
					break;

				case RememberedChoice.Never:
					ExportActiveReport(false);
					break;

				case RememberedChoice.Always:
					ExportActiveReport(true);
					break;

				default:
					goto case RememberedChoice.Ask;
					break;
			}
		});

		public ICommand ImportReport => new RelayCommand(() =>
		{
			var importPath = _api.Dialogs.SelectFile("Json files|*.json");
			try
			{
				if (string.IsNullOrEmpty(importPath))
				{
					return;
				}

				var contents = File.ReadAllText(importPath);
				var reportToImport = JsonConvert.DeserializeObject<Report1970>(contents);

				var importedReportId = _reportManager.ImportReport(reportToImport);

				_yearButtons.Clear();
				_reportButtons.Clear();

				CreateReportButtons();

				var report = _reportManager.GetReport(importedReportId);
				DisplaySpecificUserReport(report.Metadata.Year, report.Metadata.Username);
			}
			catch (ReportAlreadyExistsException ex)
			{
				_logger.Warn(ex, $"Trying to import report {importPath} that is already persisted.");
				_api.Dialogs.ShowMessage(ResourceProvider.GetString("LOC_YearInReview_Notification_ImportError_AlreadyExists"));
			}
			catch (InvalidReportFileException ex)
			{
				_logger.Warn(ex, $"Trying to import invalid report file {importPath}.");
				_api.Dialogs.ShowErrorMessage(ResourceProvider.GetString("LOC_YearInReview_Notification_ImportError_InvalidFile"));
			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"Error while trying to import report {importPath}");
				_api.Dialogs.ShowErrorMessage(ResourceProvider.GetString("LOC_YearInReview_Notification_ImportError"));
			}
		});

		public Report1970View ActiveReport
		{
			get => _activeReport;
			set => SetValue(ref _activeReport, value);
		}

		private void CreateReportButtons()
		{
			var preLoadedReports = _reportManager.GetAllPreLoadedReports();
			var years = preLoadedReports.Select(x => x.Year).Distinct().OrderByDescending(x => x);

			foreach (var year in years)
			{
				YearButtons.Add(new YearButtonViewModel()
				{
					Year = year,
					SwitchYearCommand =
						new RelayCommand(() =>
						{
							try
							{
								ReportButtons = preLoadedReports
									.Where(x => x.Year == year)
									.OrderByDescending(x => x.IsOwn)
									.ThenBy(x => x.Username)
									.Select(x => new ReportButtonViewModel()
									{
										Username = x.Username,
										DisplayCommand = new RelayCommand(() =>
										{
											try
											{
												var report = _reportManager.GetReport(x.Id);
												var allYearReports =
													preLoadedReports.Where(p => p.Year == year).ToList();
												DisplayReport(report, allYearReports);
											}
											catch (Exception ex)
											{
												_logger.Error(ex, "Error while trying to display report");
											}
										})
									}).ToObservable();
								ReportButtons.FirstOrDefault()?.DisplayCommand.Execute(null);
							}
							catch (Exception ex)
							{
								_logger.Error(ex, "Error while trying to display report");
							}
						})
				});
			}
		}

		private void DisplayReport(Report1970 report, List<PersistedReport> allYearReports)
		{
			var viewModel = new Report1970ViewModel(_api, report, allYearReports);
			var view = new Report1970View(viewModel);

			ActiveReport = view;
		}

		private void DisplayMostRecentUserReport()
		{
			YearButtons.FirstOrDefault()?.SwitchYearCommand.Execute(null);
			ReportButtons.FirstOrDefault()?.DisplayCommand.Execute(null);
		}

		private void DisplaySpecificUserReport(int year, string username)
		{
			YearButtons.FirstOrDefault(x => x.Year == year)?.SwitchYearCommand.Execute(null);
			ReportButtons.FirstOrDefault(x => x.Username == username)?.DisplayCommand.Execute(null);
		}

		private void ExportActiveReport(bool exportWithImages)
		{
			try
			{
				var exportPath = _api.Dialogs.SaveFile("Json files|*.json");
				if (string.IsNullOrEmpty(exportPath))
				{
					return;
				}

				var serializerSettings = new JsonSerializerSettings
				{
					ContractResolver = new ImageContractResolver(new Base64ImageConverter(exportWithImages, null, 400))
				};

				_reportManager.ExportReport(((Report1970ViewModel)ActiveReport.DataContext).Id, exportPath, serializerSettings);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error while trying to export report");
				_api.Dialogs.ShowErrorMessage(ResourceProvider.GetString("LOC_YearInReview_Notification_ExportError"));
			}
		}
	}

	public enum RememberedChoice
	{
		Ask,
		Never,
		Always,
	}
}