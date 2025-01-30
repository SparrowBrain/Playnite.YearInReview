using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using FakeItEasy;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Reports.Persistence;
using YearInReview.Settings;
using YearInReview.Validation;

namespace YearInReview.UnitTests.Validation
{
	public class ExtensionStartupValidatorTests
	{
		private readonly IYearInReview _plugin;
		private readonly IPlayniteAPI _playniteApi;
		private readonly IAddons _addons;
		private readonly IReportPersistence _reportPersistence;
		private readonly IGameActivityExtension _gameActivityExtension;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly YearInReviewSettings _settings;
		private readonly List<Plugin> _loadedPlugins;
		private readonly TestablePlugin _gameActivityPlugin;
		private readonly List<PersistedReport> _persistedReports;
		private readonly TestableItemCollection<Game> _gameCollection;
		private readonly ExtensionStartupValidator _sut;

		public ExtensionStartupValidatorTests()
		{
			var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
			_plugin = fixture.Freeze<IYearInReview>();
			_playniteApi = fixture.Freeze<IPlayniteAPI>();
			_addons = fixture.Freeze<IAddons>();
			_reportPersistence = fixture.Freeze<IReportPersistence>();
			_gameActivityExtension = fixture.Freeze<IGameActivityExtension>();
			_dateTimeProvider = fixture.Freeze<IDateTimeProvider>();
			_settings = fixture.Create<YearInReviewSettings>();
			_loadedPlugins = fixture.Create<List<TestablePlugin>>().Cast<Plugin>().ToList();
			_gameActivityPlugin = fixture.Create<TestablePlugin>();
			_persistedReports = fixture.Create<List<PersistedReport>>();
			_gameCollection = new TestableItemCollection<Game>(fixture.Create<List<Game>>());
			_sut = fixture.Create<ExtensionStartupValidator>();
		}

		[Theory]
		[InlineAutoFakeItEasyData(null)]
		[InlineAutoFakeItEasyData("")]
		public async Task Validate_CreatesNotification_WhenUsernameIsNotSet(string username)
		{
			// Arrange
			SetupSuccessfulValidation();
			_settings.Username = username;

			// Act
			var result = await _sut.IsOkToRun();

			// Assert
			Assert.Contains(result, x => x.Id == InitValidationError.UsernameNotSetError);
			A.CallTo(() => _playniteApi.Notifications.Add(A<NotificationMessage>.That.Matches(x => x.Id == InitValidationError.UsernameNotSetError))).MustHaveHappened();
		}

		[Fact]
		public async Task Validate_CreatesNotification_WhenGameActivityExtensionIsNotInstalled()
		{
			// Arrange
			SetupSuccessfulValidation();
			_loadedPlugins.Remove(_gameActivityPlugin);

			// Act
			var result = await _sut.IsOkToRun();

			// Assert
			Assert.Contains(result, x => x.Id == InitValidationError.GameActivityExtensionNotInstalled);
			A.CallTo(() => _playniteApi.Notifications.Add(A<NotificationMessage>.That.Matches(x => x.Id == InitValidationError.GameActivityExtensionNotInstalled))).MustHaveHappened();
		}

		[Theory]
		[AutoFakeItEasyData]
		public async Task Validate_AddsError_WhenNoReportsFoundAndNoGameActivityForPreviousYearsExists(
			int currentYear,
			List<Activity> activities)
		{
			// Arrange
			activities.ForEach(x => x.Items.ForEach(y => y.DateSession = new DateTime(currentYear, y.DateSession.Month, y.DateSession.Day)));
			SetupSuccessfulValidation();
			A.CallTo(() => _reportPersistence.PreLoadAllReports()).Returns(new List<PersistedReport>());
			A.CallTo(() => _gameActivityExtension.GetActivityForGames(An<IEnumerable<Game>>._)).Returns(activities);
			A.CallTo(() => _dateTimeProvider.GetNow()).Returns(new DateTime(currentYear, 1, 1));

			// Act
			var result = await _sut.IsOkToRun();

			// Assert
			Assert.Contains(result, x => x.Id == InitValidationError.NoActivityInPreviousYears);
		}

		[Theory]
		[AutoFakeItEasyData]
		public async Task Validate_ValidationSuccess_WhenNoReportsFoundAndGameActivityForPreviousYearsExists(
			int currentYear,
			List<Activity> activities)
		{
			// Arrange
			SetupSuccessfulValidation();
			activities.ForEach(x => x.Items.ForEach(y => y.DateSession = new DateTime(currentYear - 1, y.DateSession.Month, y.DateSession.Day)));
			A.CallTo(() => _reportPersistence.PreLoadAllReports()).Returns(new List<PersistedReport>());
			A.CallTo(() => _gameActivityExtension.GetActivityForGames(An<IEnumerable<Game>>._)).Returns(activities);
			A.CallTo(() => _dateTimeProvider.GetNow()).Returns(new DateTime(currentYear, 1, 1));

			// Act
			var result = await _sut.IsOkToRun();

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public async Task Validate_ReturnsEmpty_WhenValidationSucceeds()
		{
			// Arrange
			SetupSuccessfulValidation();

			// Act
			var result = await _sut.IsOkToRun();

			// Assert
			Assert.Empty(result);
		}

		private void SetupSuccessfulValidation()
		{
			_gameActivityPlugin.SetId(GameActivityExtension.ExtensionId);
			_loadedPlugins.Add(_gameActivityPlugin);
			A.CallTo(() => _plugin.LoadPluginSettings<YearInReviewSettings>()).Returns(_settings);
			A.CallTo(() => _playniteApi.Addons).Returns(_addons);
			A.CallTo(() => _playniteApi.Database.Games).Returns(_gameCollection);
			A.CallTo(() => _addons.Plugins).Returns(_loadedPlugins);
			A.CallTo(() => _reportPersistence.PreLoadAllReports()).Returns(_persistedReports);
		}
	}
}