using AutoFixture.Xunit2;
using FakeItEasy;
using Playnite.SDK;
using System.Threading.Tasks;
using TestTools.Shared;
using Xunit;
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
			YearInReviewSettings settings,
			ExtensionStartupValidator sut)
		{
			// Arrange
			settings.Username = username;
			A.CallTo(() => plugin.LoadPluginSettings<YearInReviewSettings>()).Returns(settings);

			// Act
			await sut.IsOkToRun();

			// Assert
			A.CallTo(() => playniteApi.Notifications.Add(A<NotificationMessage>.That.Matches(x => x.Id == "year_in_review_missing_username"))).MustHaveHappened();
		}

		[Theory]
		[InlineAutoFakeItEasyData(null)]
		[InlineAutoFakeItEasyData("")]
		public async Task Validate_ReturnsFalse_WhenUsernameIsNotSet(
			string username,
			[Frozen] IYearInReview plugin,
			YearInReviewSettings settings,
			ExtensionStartupValidator sut)
		{
			// Arrange
			settings.Username = username;
			A.CallTo(() => plugin.LoadPluginSettings<YearInReviewSettings>()).Returns(settings);

			// Act
			var result = await sut.IsOkToRun();

			// Assert
			Assert.False(result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public async Task Validate_ReturnsTrue_WhenValidationSucceeds(
			[Frozen] IYearInReview plugin,
			YearInReviewSettings settings,
			string username,
			ExtensionStartupValidator sut)
		{
			// Arrange
			settings.Username = username;
			A.CallTo(() => plugin.LoadPluginSettings<YearInReviewSettings>()).Returns(settings);

			// Act
			var result = await sut.IsOkToRun();

			// Assert
			Assert.True(result);
		}
	}
}