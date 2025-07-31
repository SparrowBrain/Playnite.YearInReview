using Newtonsoft.Json;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YearInReview.Model.Exceptions;
using YearInReview.Model.Reports._1970;
using YearInReview.Model.Reports._1970.MVVM;
using YearInReview.Model.Reports.Persistence;
using YearInReview.Progress.MVVM;
using YearInReview.Settings.MVVM;
using YearInReview.Validation;
using YearInReview.Validation.MVVM;

namespace YearInReview.Model.Reports.MVVM
{
	public class MainViewModel : ObservableObject
	{
		public const int MaxImageHeight = 400;
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly IPlayniteAPI _api;
		private readonly ReportManager _reportManager;
		private readonly YearInReviewSettingsViewModel _settingsViewModel;
		private readonly Func<ProgressViewModel> _progressViewModelFactory;

		private Report1970View _activeReport;
		private ObservableCollection<YearButtonViewModel> _yearButtons = new ObservableCollection<YearButtonViewModel>();
		private ObservableCollection<ReportButtonViewModel> _reportButtons = new ObservableCollection<ReportButtonViewModel>();
		private ObservableCollection<ValidationErrorViewModel> _validationErrors;
		private bool _showOwnReportActionButtons;

		public MainViewModel(
			IPlayniteAPI api,
			ReportManager reportManager,
			YearInReviewSettingsViewModel settingsViewModel,
			Func<ProgressViewModel> progressViewModelFactory,
			IReadOnlyCollection<InitValidationError> validationErrors)
		{
			_api = api;
			_reportManager = reportManager;
			_settingsViewModel = settingsViewModel;
			_progressViewModelFactory = progressViewModelFactory;
			SetValidationErrors(validationErrors);

			if (!HasErrors)
			{
				Init();
			}
		}

		public void Init()
		{
			_api.MainView.UIDispatcher.Invoke(() =>
			{
				CreateReportButtons();
				DisplayMostRecentUserReport();
			});
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

		public bool ShowOwnReportActionButtons
		{
			get => _showOwnReportActionButtons;
			set => SetValue(ref _showOwnReportActionButtons, value);
		}

		public ICommand RegenerateReport => new RelayCommand(() =>
		{
			try
			{
				var reportId = ActiveReportId;
				Task.Run(async () =>
				{
					try
					{
						using (var _ = _progressViewModelFactory.Invoke())
						{
							var newReportId = await _reportManager.RegenerateReport(reportId);
							_api.MainView.UIDispatcher.Invoke(() => RefreshButtonsAndDisplayReport(newReportId));
						}
					}
					catch (Exception ex)
					{
						_logger.Error(ex, "Error while trying to regenerate report");
						_api.Dialogs.ShowErrorMessage(
							ResourceProvider.GetString("LOC_YearInReview_Notification_ReportRegenerationError"));
					}
				});
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error while trying to regenerate report");
				_api.Dialogs.ShowErrorMessage(
					ResourceProvider.GetString("LOC_YearInReview_Notification_ReportRegenerationError"));
			}
		});

		public ICommand ExportReport => new RelayCommand(() =>
		{
			var settings = _settingsViewModel.Settings;
			switch (settings.ExportWithImages)
			{
				case RememberedChoice.Ask:
					ShowExportDialog();
					break;

				case RememberedChoice.Never:
					ExportActiveReport(false);
					break;

				case RememberedChoice.Always:
					ExportActiveReport(true);
					break;

				default:
					goto case RememberedChoice.Ask;
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

				RefreshButtonsAndDisplayReport(importedReportId);
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

		public ObservableCollection<ValidationErrorViewModel> ValidationErrors
		{
			get => _validationErrors;
			set
			{
				SetValue(ref _validationErrors, value);
				OnPropertyChanged(nameof(HasErrors));
			}
		}

		public bool HasErrors => ValidationErrors.Any();

		public void SetValidationErrors(IReadOnlyCollection<InitValidationError> validationErrors)
		{
			_api.MainView.UIDispatcher.Invoke(() =>
			{
				ValidationErrors = validationErrors
					.Select(x => new ValidationErrorViewModel(x.Message, x.CallToAction))
					.ToObservable();
			});
		}

		private Guid ActiveReportId => ((Report1970ViewModel)ActiveReport.DataContext).Id;

		private void CreateReportButtons()
		{
			var preLoadedReports = _reportManager.GetAllPreLoadedReports();
			var years = preLoadedReports.Select(x => x.Year).Distinct().OrderByDescending(x => x);

			YearButtons = years.Select(year => new YearButtonViewModel()
			{
				Year = year,
				SwitchYearCommand = new RelayCommand(() =>
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
										ActivateReport(report, x.IsOwn, allYearReports);
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
			}).ToObservable();
		}

		private void ActivateReport(Report1970 report, bool isOwn, List<PersistedReport> allYearReports)
		{
			var viewModel = new Report1970ViewModel(_api, report, isOwn, allYearReports);
			var view = new Report1970View(viewModel);

			ShowOwnReportActionButtons = isOwn;
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

		private void ShowExportDialog()
		{
			var viewModel = new ExportWithImagesViewModel(_settingsViewModel, ExportActiveReport);
			var view = new ExportWithImagesView(viewModel);

			var window = _api.Dialogs.CreateWindow(new WindowCreationOptions()
			{
				ShowCloseButton = true,
				ShowMaximizeButton = false,
				ShowMinimizeButton = false,
			});

			window.Height = 200;
			window.Width = 500;
			window.Title = ResourceProvider.GetString("LOC_YearInReview_ExportWithImages_Title");

			window.Content = view;
			viewModel.AssociateWindow(window);

			window.Owner = _api.Dialogs.GetCurrentAppWindow();
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			window.ShowDialog();
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

				var reportId = ((Report1970ViewModel)ActiveReport.DataContext).Id;
				_reportManager.ExportReport(reportId, exportPath, exportWithImages);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error while trying to export report");
				_api.Dialogs.ShowErrorMessage(ResourceProvider.GetString("LOC_YearInReview_Notification_ExportError"));
			}
		}

		private void RefreshButtonsAndDisplayReport(Guid reportId)
		{
			CreateReportButtons();

			var report = _reportManager.GetReport(reportId);
			DisplaySpecificUserReport(report.Metadata.Year, report.Metadata.Username);
		}
	}
}