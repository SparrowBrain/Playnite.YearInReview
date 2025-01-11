using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using YearInReview.Extensions.GameActivity;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Aggregators;
using YearInReview.Model.Filters;
using YearInReview.Model.Reports;
using YearInReview.Model.Reports._1970;
using YearInReview.Model.Reports.MVVM;
using YearInReview.Model.Reports.Persistence;
using YearInReview.Settings;
using YearInReview.Settings.MVVM;

namespace YearInReview
{
	public class YearInReview : GenericPlugin, IYearInReview
	{
		private static readonly ILogger Logger = LogManager.GetLogger();
		private readonly StartupSettingsValidator _startupSettingsValidator;
		private readonly ReportManager _reportManager;

		private YearInReviewSettingsViewModel _settingsViewModel;
		private MainViewModel _mainViewModel;
		private MainView _mainView;

		public override Guid Id { get; } = Guid.Parse("a22a7611-3023-4ca8-907e-47f883acd1b2");

		public YearInReview(IPlayniteAPI api) : base(api)
		{
			Api = api;

			Properties = new GenericPluginProperties
			{
				HasSettings = true
			};

			var pluginSettingsPersistence = new PluginSettingsPersistence(this);
			_startupSettingsValidator = new StartupSettingsValidator(pluginSettingsPersistence);

			var dateTimeProvider = new DateTimeProvider();
			var metadataProvider = new MetadataProvider(dateTimeProvider, pluginSettingsPersistence);
			var mostPlayedGamesAggregator = new MostPlayedGamesAggregator(Api);
			var mostPlayedSourcesAggregator = new MostPlayedSourcesAggregator(Api);
			var totalPlaytimeAggregator = new TotalPlaytimeAggregator();
			var playtimeCalendarAggregator = new PlaytimeCalendarAggregator(Api);
			var hourlyPlaytimeAggregator = new HourlyPlaytimeAggregator();
			var composer = new Composer1970(
				metadataProvider,
				totalPlaytimeAggregator,
				mostPlayedGamesAggregator,
				mostPlayedSourcesAggregator,
				playtimeCalendarAggregator,
				hourlyPlaytimeAggregator);
			var gameActivityExtension = new GameActivityExtension(Api.Paths.ExtensionsDataPath);
			var specificYearActivityFilter = new SpecificYearActivityFilter();
			var reportPersistence = new ReportPersistence(GetPluginUserDataPath());
			var reportGenerator = new ReportGenerator(Api, dateTimeProvider, gameActivityExtension, specificYearActivityFilter, composer);
			_reportManager = new ReportManager(reportPersistence, reportGenerator, dateTimeProvider);
		}

		public static IPlayniteAPI Api { get; private set; }

		public override IEnumerable<SidebarItem> GetSidebarItems()
		{
			yield return new SidebarItem
			{
				Title = ResourceProvider.GetString("LOC_YearInReview_PluginName"),
				Icon = new TextBlock
				{
					Text = char.ConvertFromUtf32(0xeffe),
					FontFamily = ResourceProvider.GetResource("FontIcoFont") as FontFamily
				},
				Type = SiderbarItemType.View,
				Opened = () =>
				{
					if (_mainView == null || _mainViewModel == null)
					{
						_mainViewModel = new MainViewModel(Api, _reportManager);
						_mainView = new MainView(_mainViewModel);
					}

					return _mainView;
				}
			};
		}

		public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
		{
			_startupSettingsValidator.EnsureCorrectVersionSettingsExist();
			ValidateExtensionStateAndRun();
		}

	

		public override ISettings GetSettings(bool firstRunSettings)
		{
			return _settingsViewModel ?? (_settingsViewModel = new YearInReviewSettingsViewModel(this));
		}

		public override UserControl GetSettingsView(bool firstRunSettings)
		{
			return new YearInReviewSettingsView();
		}

		private void ValidateExtensionStateAndRun()
		{
			if (_settingsViewModel != null)
			{
				_settingsViewModel.SettingsSaved -= HandleSettingsViewModelSettingsSaved;
			}

			var extensionStartupValidator = new ExtensionStartupValidator(this, Api);
			var isOkToRun = extensionStartupValidator.IsOkToRun().Result;
			if (isOkToRun)
			{
				RunInit();
			}
			else
			{
				GetSettings(false);
				if (_settingsViewModel != null)
				{
					_settingsViewModel.SettingsSaved += HandleSettingsViewModelSettingsSaved;
				}
			}
		}

		private void HandleSettingsViewModelSettingsSaved()
		{
			if (_settingsViewModel != null)
			{
				_settingsViewModel.SettingsSaved -= HandleSettingsViewModelSettingsSaved;
			}

			ValidateExtensionStateAndRun();
		}

		private void RunInit()
		{
			Task.Run(async () =>
			{
				try
				{
					await _reportManager.Init();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Failed to init report manager.");
				}
			});
		}
	}
}