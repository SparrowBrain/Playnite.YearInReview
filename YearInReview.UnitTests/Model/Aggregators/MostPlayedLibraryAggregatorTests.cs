using AutoFixture.Xunit2;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators;

namespace YearInReview.UnitTests.Model.Aggregators
{
	public class MostPlayedLibraryAggregatorTests
	{
		[Theory]
		[AutoFakeItEasyData]
		public void GetMostPlayedLibraries_ReturnsLibraryWithMostElapsedTimeAsFirst_WhenActivitiesProvided(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			Guid mostPlayedSourceId,
			List<Activity> activities,
			MostPlayedLibrariesAggregator sut)
		{
			// Arrange
			var mostPlayedGame = activities.Last();
			var biggestSession = new Session { ElapsedSeconds = int.MaxValue / 2, SourceId = mostPlayedSourceId };
			mostPlayedGame.Items.Add(biggestSession);
			var sources = activities.SelectMany(x => x.Items).Select(x => new GameSource() { Id = x.SourceId }).ToList();

			playniteApiFake.CallsTo(x => x.Database).Returns(gameDatabaseApiFake);
			gameDatabaseApiFake.CallsTo(x => x.Sources).Returns(new TestableItemCollection<GameSource>(sources));

			// Act
			var result = sut.GetMostPlayedLibraries(activities);

			// Assert
			Assert.NotNull(result);
			var actualMostPlayed = result.First();
			Assert.Equal(mostPlayedSourceId, actualMostPlayed.Source.Id);
			Assert.Equal(biggestSession.ElapsedSeconds, (long)actualMostPlayed.TimePlayed);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetMostPlayedLibraries_SkipsSource_WhenSourceDoesNotExistInDatabase(
			List<Activity> activities,
			MostPlayedLibrariesAggregator sut)
		{
			// Act
			var result = sut.GetMostPlayedLibraries(activities);

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result);
		}
	}
}