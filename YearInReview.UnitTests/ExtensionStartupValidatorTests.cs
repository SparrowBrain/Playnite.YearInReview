using AutoFixture.Xunit2;
using FakeItEasy;
using Playnite.SDK;
using Playnite.SDK.Plugins;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Settings;
using YearInReview.Validation;

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
			Assert.Contains(result, x => x.Id == InitValidationError.UsernameNotSetError);
			A.CallTo(() => playniteApi.Notifications.Add(A<NotificationMessage>.That.Matches(x => x.Id == InitValidationError.UsernameNotSetError))).MustHaveHappened();
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
			Assert.Contains(result, x => x.Id == InitValidationError.GameActivityExtensionNotInstalled);
			A.CallTo(() => playniteApi.Notifications.Add(A<NotificationMessage>.That.Matches(x => x.Id == InitValidationError.GameActivityExtensionNotInstalled))).MustHaveHappened();
		}

		[Theory]
		[AutoFakeItEasyData]
		public async Task Validate_ReturnsEmpty_WhenValidationSucceeds(
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
			Assert.Empty(result);
		}
	}
}