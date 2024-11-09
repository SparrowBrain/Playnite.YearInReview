using AutoFixture.Xunit2;
using FakeItEasy;
using System;
using TestTools.Shared;
using Xunit;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Aggregators;
using YearInReview.Settings;

namespace YearInReview.UnitTests.Model.Aggregators
{
	public class MetadataProviderTests
	{
		[Theory]
		[AutoFakeItEasyData]
		public void Aggregate_ReturnsMetadataWithCorrectYear(
			int year,
			MetadataProvider sut)
		{
			// Act
			var result = sut.Get(year);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(year, result.Year);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void Aggregate_ReturnsMetadataWithTimestamp(
			[Frozen] IDateTimeProvider dateTimeProviderFake,
			DateTime generatedTimestamp,
			int year,
			MetadataProvider sut)
		{
			// Arrange
			A.CallTo(() => dateTimeProviderFake.GetNow()).Returns(generatedTimestamp);

			// Act
			var result = sut.Get(year);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(generatedTimestamp, result.GeneratedTimestamp);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void Aggregate_ReturnsMetadataUsername(
			[Frozen] IPluginSettingsPersistence pluginSettingsPersistenceFake,
			YearInReviewSettings settings,
			string username,
			int year,
			MetadataProvider sut)
		{
			// Arrange
			settings.Username = username;
			A.CallTo(() => pluginSettingsPersistenceFake.LoadPluginSettings<YearInReviewSettings>()).Returns(settings);

			// Act
			var result = sut.Get(year);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(username, result.Username);
		}
	}
}