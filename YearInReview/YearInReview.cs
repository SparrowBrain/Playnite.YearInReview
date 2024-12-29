using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using YearInReview.Extensions.GameActivity;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Aggregators;
using YearInReview.Model.Filters;
using YearInReview.Model.Reports;
using YearInReview.Model.Reports._1970;
using YearInReview.Model.Reports.MVVM;
using YearInReview.Settings;
using YearInReview.Settings.MVVM;

namespace YearInReview
{
	public class YearInReview : GenericPlugin
	{
		private static readonly ILogger logger = LogManager.GetLogger();
		private readonly StartupSettingsValidator _startupSettingsValidator;

		private YearInReviewSettingsViewModel settings { get; set; }

		public override Guid Id { get; } = Guid.Parse("a22a7611-3023-4ca8-907e-47f883acd1b2");

		public YearInReview(IPlayniteAPI api) : base(api)
		{
			Api = api;
			settings = new YearInReviewSettingsViewModel(this);
			Properties = new GenericPluginProperties
			{
				HasSettings = true
			};

			var pluginSettingsPersistence = new PluginSettingsPersistence(this);
			_startupSettingsValidator = new StartupSettingsValidator(pluginSettingsPersistence);
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
					//if (_playNextMainView == null || _playNextMainViewModel == null)
					//{
					// _playNextMainViewModel = new PlayNextMainViewModel(this);
					// _playNextMainView = new PlayNextMainView(_playNextMainViewModel);
					// RefreshPlayNextData();
					//}

					//return _playNextMainView;

					var dateTimeProvider = new DateTimeProvider();
					var pluginSettingsPersistence = new PluginSettingsPersistence(this);
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
					var reportManager = new ReportManager(Api, composer, gameActivityExtension, specificYearActivityFilter);
					var viewModel = new MainViewModel(Api, reportManager);
					var view = new MainView(viewModel);

					return view;
				}
			};
		}

		public override void OnGameInstalled(OnGameInstalledEventArgs args)
		{
			// Add code to be executed when game is finished installing.
		}

		public override void OnGameStarted(OnGameStartedEventArgs args)
		{
			// Add code to be executed when game is started running.
		}

		public override void OnGameStarting(OnGameStartingEventArgs args)
		{
			// Add code to be executed when game is preparing to be started.
		}

		public override void OnGameStopped(OnGameStoppedEventArgs args)
		{
			// Add code to be executed when game is preparing to be started.
		}

		public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
		{
			// Add code to be executed when game is uninstalled.
		}

		public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
		{
			_startupSettingsValidator.EnsureCorrectVersionSettingsExist();
		}

		public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
		{
			// Add code to be executed when Playnite is shutting down.
		}

		public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
		{
			// Add code to be executed when library is updated.
		}

		public override ISettings GetSettings(bool firstRunSettings)
		{
			return settings;
		}

		public override UserControl GetSettingsView(bool firstRunSettings)
		{
			return new YearInReviewSettingsView();
		}
	}
}