﻿using Playnite.SDK;
using Playnite.SDK.Controls;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
using YearInReview.Validation;

namespace YearInReview
{
	public class YearInReview : GenericPlugin, IYearInReview
	{
		private static readonly ILogger Logger = LogManager.GetLogger();
		private readonly StartupSettingsValidator _startupSettingsValidator;
		private readonly PluginSettingsPersistence _pluginSettingsPersistence;

		private YearInReviewSettingsViewModel _settingsViewModel;
		private MainViewModel _mainViewModel;
		private MainView _mainView;
		private ReportManager _reportManager;
		private bool _isStartupValidationSuccess;
		private IReadOnlyCollection<InitValidationError> _initValidationErrors = new List<InitValidationError>();
		private SidebarItem _sidebarItem;

		private const double DefaultDialogWindowHeight = 600;
		private const double DefaultDialogWindowWidth = 1000;

		public override Guid Id { get; } = Guid.Parse("a22a7611-3023-4ca8-907e-47f883acd1b2");

		public YearInReview(IPlayniteAPI api) : base(api)
		{
			Api = api;

			Properties = new GenericPluginProperties
			{
				HasSettings = true
			};

			_pluginSettingsPersistence = new PluginSettingsPersistence(this);
			_startupSettingsValidator = new StartupSettingsValidator(_pluginSettingsPersistence);
		}

		public static IPlayniteAPI Api { get; private set; }

		public override IEnumerable<SidebarItem> GetSidebarItems()
		{
			if (_sidebarItem == null)
			{
				_sidebarItem = new SidebarItem
				{
					Title = ResourceProvider.GetString("LOC_YearInReview_PluginName"),
					Icon = new TextBlock
					{
						Text = char.ConvertFromUtf32(0xeffe),
						FontFamily = ResourceProvider.GetResource("FontIcoFont") as FontFamily
					},
					Type = SiderbarItemType.View,
					Visible = false,
					Opened = () =>
					{
						if (!_isStartupValidationSuccess)
						{
							RunValidationInitialize();
						}

						if (_mainView == null || _mainViewModel == null)
						{
							_mainViewModel = new MainViewModel(Api, GetReportManager(), _settingsViewModel, _initValidationErrors);
							_mainView = new MainView(_mainViewModel);
						}

						return _mainView;
					}
				};
			}

			yield return _sidebarItem;
		}

		public override IEnumerable<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs args)
		{
			var pluginName = ResourceProvider.GetString("LOC_YearInReview_PluginName");
			var pluginMenuSection = "@" + pluginName;

			yield return new MainMenuItem
			{
				Description = ResourceProvider.GetString("LOC_YearInReview_ShowYearInReviewMenuItem"),
				MenuSection = pluginMenuSection,
				Action = _ =>
				{
					var view = new MainView(new MainViewModel(Api, GetReportManager(), _settingsViewModel, _initValidationErrors));
					OpenViewAsDialog(view, pluginName);
				}
			};
		}

		public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
		{
			_startupSettingsValidator.EnsureCorrectVersionSettingsExist();
			RunValidationInitialize();

			GetSettings(false);
			HandleSidebarItemVisibilityAfterSettingsSaved();
		}

		public override ISettings GetSettings(bool firstRunSettings)
		{
			if (_settingsViewModel == null)
			{
				_settingsViewModel = new YearInReviewSettingsViewModel(this);
				_settingsViewModel.SettingsSaved += HandleSidebarItemVisibilityAfterSettingsSaved;
			}

			return _settingsViewModel;
		}

		public override UserControl GetSettingsView(bool firstRunSettings)
		{
			return new YearInReviewSettingsView();
		}

		private void RunValidationInitialize()
		{
			Task.Run(async () =>
			{
				try
				{
					if (_settingsViewModel != null)
					{
						_settingsViewModel.SettingsSaved -= HandleValidationAfterSettingsSaved;
					}

					var extensionStartupValidator = new ExtensionStartupValidator(
						this,
						Api,
						new ReportPersistence(GetPluginUserDataPath()),
						new GameActivityExtension(Api.Paths.ExtensionsDataPath),
						new DateTimeProvider());

					_initValidationErrors = await extensionStartupValidator.IsOkToRun();
					_mainViewModel?.SetValidationErrors(_initValidationErrors);

					if (_initValidationErrors.Count == 0)
					{
						_isStartupValidationSuccess = true;
						RunInit();
						return;
					}

					if (_initValidationErrors.Any(x => x.Id == InitValidationError.UsernameNotSetError))
					{
						GetSettings(false);
						if (_settingsViewModel != null)
						{
							_settingsViewModel.SettingsSaved += HandleValidationAfterSettingsSaved;
						}
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "Failed to run validation or initialize.");
				}
			});
		}

		private void HandleSidebarItemVisibilityAfterSettingsSaved()
		{
			_sidebarItem.Visible = _settingsViewModel.Settings.ShowSidebarItem;
		}

		private void HandleValidationAfterSettingsSaved()
		{
			if (_settingsViewModel != null)
			{
				_settingsViewModel.SettingsSaved -= HandleValidationAfterSettingsSaved;
			}

			if (_isStartupValidationSuccess)
			{
				return;
			}

			RunValidationInitialize();
		}

		private void RunInit()
		{
			Task.Run(async () =>
			{
				try
				{
					await GetReportManager().Init();
					_mainViewModel?.Init();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Failed to init report manager.");
				}
			});
		}

		private ReportManager GetReportManager()
		{
			if (_reportManager != null)
			{
				return _reportManager;
			}

			var dateTimeProvider = new DateTimeProvider();
			var metadataProvider = new MetadataProvider(dateTimeProvider, _pluginSettingsPersistence);
			var mostPlayedGamesAggregator = new MostPlayedGamesAggregator(Api);
			var mostPlayedSourcesAggregator = new MostPlayedSourcesAggregator(Api);
			var totalPlaytimeAggregator = new TotalPlaytimeAggregator();
			var playtimeCalendarAggregator = new PlaytimeCalendarAggregator(Api);
			var hourlyPlaytimeAggregator = new HourlyPlaytimeAggregator();
			var addedGamesAggregator = new AddedGamesAggregator(Api);
			var composer = new Composer1970(
				metadataProvider,
				totalPlaytimeAggregator,
				mostPlayedGamesAggregator,
				mostPlayedSourcesAggregator,
				playtimeCalendarAggregator,
				hourlyPlaytimeAggregator,
				addedGamesAggregator);
			var gameActivityExtension = new GameActivityExtension(Api.Paths.ExtensionsDataPath);
			var specificYearActivityFilter = new SpecificYearActivityFilter();
			var reportPersistence = new ReportPersistence(GetPluginUserDataPath());
			var emptyActivityFilter = new EmptyActivityFilter();
			var reportGenerator = new ReportGenerator(
				Api,
				dateTimeProvider,
				gameActivityExtension,
				specificYearActivityFilter,
				emptyActivityFilter,
				composer);
			_reportManager = new ReportManager(reportPersistence, reportGenerator, dateTimeProvider, Api, _settingsViewModel.Settings);

			return _reportManager;
		}

		private void OpenViewAsDialog(
			PluginUserControl view,
			string title,
			double height = DefaultDialogWindowHeight,
			double width = DefaultDialogWindowWidth
		)
		{
			var window = PlayniteApi.Dialogs.CreateWindow(new WindowCreationOptions
			{
				ShowMinimizeButton = false
			});
			window.Height = height;
			window.Width = width;
			window.Title = title;
			window.Content = view;
			window.Owner = PlayniteApi.Dialogs.GetCurrentAppWindow();
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.ShowDialog();
		}
	}
}