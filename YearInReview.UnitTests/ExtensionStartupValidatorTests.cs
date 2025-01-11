using AutoFixture.Xunit2;
using FakeItEasy;
using Playnite.SDK;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Settings;

namespace YearInReview.UnitTests
{
	public class ExtensionStartupValidatorTests
	{
		[Theory]
		[InlineAutoFakeItEasyData(null)]
		[InlineAutoFakeItEasyData("")]
		public async Task Validate_CreatesNotification_WhenUsernameIsNotSet(
			string username,
			[Frozen] IYearInReview plugin,
			[Frozen] IPlayniteAPI playniteApi,
			[Frozen] IAddons addons,
			YearInReviewSettings settings,
			List<TestablePlugin> loadedPlugins,
			TestablePlugin gameActivityPlugin,
			ExtensionStartupValidator sut)
		{
			// Arrange
			settings.Username = username;
			gameActivityPlugin.SetId(GameActivityExtension.ExtensionId);
			loadedPlugins.Add(gameActivityPlugin);
			A.CallTo(() => plugin.LoadPluginSettings<YearInReviewSettings>()).Returns(settings);
			A.CallTo(() => playniteApi.Addons).Returns(addons);
			A.CallTo(() => addons.Plugins).Returns(loadedPlugins.Select(x => (Plugin)x).ToList());

			// Act
			var result = await sut.IsOkToRun();

			// Assert
			Assert.False(result);
			A.CallTo(() => playniteApi.Notifications.Add(A<NotificationMessage>.That.Matches(x => x.Id == "year_in_review_missing_username"))).MustHaveHappened();
		}

		[Theory]
		[AutoFakeItEasyData]
		public async Task Validate_CreatesNotification_WhenGameActivityExtensionIsNotInstalled(
			[Frozen] IYearInReview plugin,
			[Frozen] IPlayniteAPI playniteApi,
			[Frozen] IAddons addons,
			YearInReviewSettings settings,
			List<TestablePlugin> loadedPlugins,
			ExtensionStartupValidator sut)
		{
			// Arrange
			A.CallTo(() => plugin.LoadPluginSettings<YearInReviewSettings>()).Returns(settings);
			A.CallTo(() => playniteApi.Addons).Returns(addons);
			A.CallTo(() => addons.Plugins).Returns(loadedPlugins.Select(x => (Plugin)x).ToList());

			// Act
			var result = await sut.IsOkToRun();

			// Assert
			Assert.False(result);
			A.CallTo(() => playniteApi.Notifications.Add(A<NotificationMessage>.That.Matches(x => x.Id == "year_in_review_game_activity_not_installed"))).MustHaveHappened();
		}

		[Theory]
		[AutoFakeItEasyData]
		public async Task Validate_ReturnsTrue_WhenValidationSucceeds(
			[Frozen] IYearInReview plugin,
			[Frozen] IPlayniteAPI playniteApi,
			[Frozen] IAddons addons,
			YearInReviewSettings settings,
			List<TestablePlugin> loadedPlugins,
			TestablePlugin gameActivityPlugin,
			ExtensionStartupValidator sut)
		{
			// Arrange
			gameActivityPlugin.SetId(GameActivityExtension.ExtensionId);
			loadedPlugins.Add(gameActivityPlugin);
			A.CallTo(() => plugin.LoadPluginSettings<YearInReviewSettings>()).Returns(settings);
			A.CallTo(() => playniteApi.Addons).Returns(addons);
			A.CallTo(() => addons.Plugins).Returns(loadedPlugins.Select(x => (Plugin)x).ToList());

			// Act
			var result = await sut.IsOkToRun();

			// Assert
			Assert.True(result);
		}
	}
}

public class TestablePlugin : Plugin
{
	private Guid _id;

	public TestablePlugin(IPlayniteAPI playniteApi) : base(playniteApi)
	{
	}

	public override Guid Id => _id;

	public void SetId(Guid id)
	{
		_id = id;
	}
}