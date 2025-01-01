using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using YearInReview.Model.Reports._1970;
using YearInReview.Model.Reports._1970.MVVM;

namespace YearInReview.Model.Reports.MVVM
{
	public class MainViewModel : ObservableObject
	{
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly IPlayniteAPI _api;
		private readonly ReportManager _reportManager;
		private UserControl _activeReport;
		private ObservableCollection<YearButtonViewModel> _yearButtons = new ObservableCollection<YearButtonViewModel>();

		public MainViewModel(IPlayniteAPI api, ReportManager reportManager)
		{
			_api = api;
			_reportManager = reportManager;
			var year = 2024;

			YearButtons.Add(new YearButtonViewModel()
			{
				Year = year,
				DisplayCommand =
					new RelayCommand(async () =>
					{
						try
						{
							var report = await _reportManager.GetReport(year);
							var old = await _reportManager.GetReport(2023);

							DisplayReport(report, new List<Report1970>() { old });
						}
						catch (Exception ex)
						{
							_logger.Error(ex, "Error while trying to display report");
						}
					})
			});
		}

		private void DisplayReport(Report1970 report, List<Report1970> friendReports)
		{
			var viewModel = new Report1970ViewModel(_api, report, friendReports);
			var view = new Report1970View(viewModel);

			ActiveReport = view;
		}

		public ObservableCollection<YearButtonViewModel> YearButtons
		{
			get => _yearButtons;
			set => SetValue(ref _yearButtons, value);
		}

		public UserControl ActiveReport
		{
			get => _activeReport;
			set => SetValue(ref _activeReport, value);
		}
	}
}