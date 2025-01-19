using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using YearInReview.Model.Reports._1970;
using YearInReview.Model.Reports._1970.MVVM;
using YearInReview.Model.Reports.Persistence;

namespace YearInReview.Model.Reports.MVVM
{
	public class MainViewModel : ObservableObject
	{
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly IPlayniteAPI _api;
		private readonly ReportManager _reportManager;

		private UserControl _activeReport;
		private ObservableCollection<YearButtonViewModel> _yearButtons = new ObservableCollection<YearButtonViewModel>();
		private ObservableCollection<ReportButtonViewModel> _reportButtons = new ObservableCollection<ReportButtonViewModel>();

		public MainViewModel(IPlayniteAPI api, ReportManager reportManager)
		{
			_api = api;
			_reportManager = reportManager;

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
			var exportPath = _api.Dialogs.SaveFile("Json files|*.json");
			if (string.IsNullOrEmpty(exportPath))
			{
				return;
			}

			_reportManager.ExportReport(((Report1970ViewModel)ActiveReport.DataContext).Id, exportPath);
		});

		public ICommand ImportReport => new RelayCommand(() =>
		{
			var importPath = _api.Dialogs.SelectFile("Json files|*.json");
			if (string.IsNullOrEmpty(importPath))
			{
				return;
			}

			var importedReportId = _reportManager.ImportReport(importPath);

			_yearButtons.Clear();
			_reportButtons.Clear();

			CreateReportButtons();

			var report = _reportManager.GetReport(importedReportId);
			var preLoadedReports = _reportManager.GetAllPreLoadedReports();
			var allYearReports = preLoadedReports.Where(p => p.Year == report.Metadata.Year).ToList();
			DisplayReport(report, allYearReports);
		});

		public UserControl ActiveReport
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
	}
}