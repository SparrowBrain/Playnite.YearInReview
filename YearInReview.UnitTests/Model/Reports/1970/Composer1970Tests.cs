using AutoFixture.Xunit2;
using FakeItEasy;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators;
using YearInReview.Model.Aggregators.Data;
using YearInReview.Model.Reports._1970;

namespace YearInReview.UnitTests.Model.Reports._1970
{
	public class Composer1970Tests
	{
		[Theory]
		[AutoFakeItEasyData]
		public void Compose_AssignsMetadata(
			[Frozen] IMetadataProvider metadataProviderFake,
			Metadata metadata,
			Game mostPlayedGame,
			int year,
			List<Activity> activities,
			Composer1970 sut)
		{
			// Arrange
			A.CallTo(() => metadataProviderFake.Get(A<int>._)).Returns(metadata);

			// Act
			var result = sut.Compose(year, activities);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(metadata, result.Metadata);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void Compose_AssignsMostPlayedGames(
			[Frozen] IMostPlayedGamesAggregator mostPlayedGameAggregatorFake,
			List<GameWithTime> mostPlayedGames,
			int year,
			List<Activity> activities,
			Composer1970 sut)
		{
			// Arrange
			A.CallTo(() => mostPlayedGameAggregatorFake.GetMostPlayedGames(A<IReadOnlyCollection<Activity>>._, A<int>._)).Returns(mostPlayedGames);

			// Act
			var result = sut.Compose(year, activities);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(mostPlayedGames.Count, result.MostPlayedGames.Count);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void Compose_AssignsTotalPlaytime(
			[Frozen] ITotalPlaytimeAggregator totalPlaytimeAggregator,
			int totalPlaytime,
			int year,
			List<Activity> activities,
			Composer1970 sut)
		{
			// Arrange
			A.CallTo(() => totalPlaytimeAggregator.GetTotalPlaytime(A<IReadOnlyCollection<Activity>>._)).Returns(totalPlaytime);

			// Act
			var result = sut.Compose(year, activities);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(totalPlaytime, result.TotalPlaytime);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void Compose_AssignsMostPlayedSources(
			[Frozen] IMostPlayedSourcesAggregator mostPlayedSourcesAggregatorFake,
			List<SourceWithTime> mostPlayedSources,
			int year,
			List<Activity> activities,
			Composer1970 sut)
		{
			// Arrange
			A.CallTo(() => mostPlayedSourcesAggregatorFake.GetMostPlayedSources(A<IReadOnlyCollection<Activity>>._)).Returns(mostPlayedSources);

			// Act
			var result = sut.Compose(year, activities);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(mostPlayedSources.Count, result.MostPlayedSources.Count);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void Compose_AssignsCalendarDays(
			[Frozen] IPlaytimeCalendarAggregator playtimeCalendarAggregatorFake,
			Dictionary<DateTime, CalendarDay> calendar,
			int year,
			List<Activity> activities,
			Composer1970 sut)
		{
			// Arrange
			A.CallTo(() => playtimeCalendarAggregatorFake.GetCalendar(year, activities))
				.Returns(calendar);

			// Act
			var result = sut.Compose(year, activities);

			// Assert
			Assert.Equal(calendar.Count, result.PlaytimeCalendarDays.Count);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void Compose_AssignsHourlyPlaytime(
			[Frozen] IHourlyPlaytimeAggregator hourlyPlaytimeAggregatorFake,
			Dictionary<int, int> playtime,
			int year,
			List<Activity> activities,
			Composer1970 sut)
		{
			// Arrange
			A.CallTo(() => hourlyPlaytimeAggregatorFake.GetHours(activities))
				.Returns(playtime);

			// Act
			var result = sut.Compose(year, activities);

			// Assert
			Assert.Equal(playtime.Count, result.HourlyPlaytime.Count);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void Compose_AssignsAddedGames(
			[Frozen] IAddedGamesAggregator addedGamesAggregatorFake,
			List<AddedGame> games,
			int year,
			List<Activity> activities,
			Composer1970 sut)
		{
			// Arrange
			A.CallTo(() => addedGamesAggregatorFake.GetAddedGames(year))
				.Returns(games);

			// Act
			var result = sut.Compose(year, activities);

			// Assert
			Assert.Equal(games.Count, result.AddedGamesCount);
		}
	}
}