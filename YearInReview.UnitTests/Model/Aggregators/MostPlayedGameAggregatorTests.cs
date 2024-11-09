﻿using AutoFixture.Xunit2;
using FakeItEasy;
using Playnite.SDK;
using Playnite.SDK.Models;
using System.Collections.Generic;
using System.Linq;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators;

namespace YearInReview.UnitTests.Model.Aggregators
{
	public class MostPlayedGameAggregatorTests
	{
		[Theory]
		[AutoFakeItEasyData]
		public void GetMostPlayedGame_ThrowsArgumentException_WhenNoActivities(
			int gameCount,
			MostPlayedGamesAggregator sut)
		{
			// Arrange
			var activities = new List<Activity>();

			// Act
			var exception = Record.Exception(() => sut.GetMostPlayedGame(activities, gameCount));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<System.ArgumentException>(exception);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetMostPlayedGame_ThrowsArgumentException_WhenActivitiesDoNotHaveSessions(
			int gameCount,
			List<Activity> activities,
			MostPlayedGamesAggregator sut)
		{
			// Arrange
			activities.ForEach(x => x.Items.Clear());

			// Act
			var exception = Record.Exception(() => sut.GetMostPlayedGame(activities, gameCount));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<System.ArgumentException>(exception);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetMostPlayedGame_ThrowsArgumentException_WhenHaveZeroTime(
			int gameCount,
			List<Activity> activities,
			MostPlayedGamesAggregator sut)
		{
			// Arrange
			activities.ForEach(x => x.Items.ForEach(s => s.ElapsedSeconds = 0));

			// Act
			var exception = Record.Exception(() => sut.GetMostPlayedGame(activities, gameCount));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<System.ArgumentException>(exception);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetMostPlayedGame_ReturnsGameWithMostElapsedTimeAsFirst_WhenActivitiesProvided(
			[Frozen] Fake<IGameDatabaseAPI> gameDatabaseApiFake,
			[Frozen] Fake<IPlayniteAPI> playniteApiFake,
			int gameCount,
			List<Activity> activities,
			MostPlayedGamesAggregator sut)
		{
			// Arrange
			var mostPlayedGame = activities.Last();
			mostPlayedGame.Items.Add(new Session { ElapsedSeconds = int.MaxValue });
			var games = activities.Select(x => new Game { Id = x.Id }).ToList();

			gameDatabaseApiFake.CallsTo(x => x.Games).Returns(new TestableItemCollection<Game>(games));
			playniteApiFake.CallsTo(x => x.Database).Returns(gameDatabaseApiFake.FakedObject);

			// Act
			var result = sut.GetMostPlayedGame(activities, gameCount);

			// Assert
			Assert.NotNull(result);
			var actualMostPlayed = result.First();
			Assert.Equal(mostPlayedGame.Id, actualMostPlayed.Game.Id);
			Assert.Equal(mostPlayedGame.Items.Sum(s => (long)s.ElapsedSeconds), (long)actualMostPlayed.TimePlayed);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetMostPlayedGame_ReturnsAtMostGameCountOfGames_WhenMoreGamesHaveActivities(
			[Frozen] Fake<IGameDatabaseAPI> gameDatabaseApiFake,
			[Frozen] Fake<IPlayniteAPI> playniteApiFake,
			List<Activity> activities,
			MostPlayedGamesAggregator sut)
		{
			// Arrange
			var gameCount = 2;
			var mostPlayedGame = activities.Last();
			mostPlayedGame.Items.Add(new Session { ElapsedSeconds = int.MaxValue });
			var games = activities.Select(x => new Game { Id = x.Id }).ToList();

			gameDatabaseApiFake.CallsTo(x => x.Games).Returns(new TestableItemCollection<Game>(games));
			playniteApiFake.CallsTo(x => x.Database).Returns(gameDatabaseApiFake.FakedObject);

			// Act
			var result = sut.GetMostPlayedGame(activities, gameCount);

			// Assert
			Assert.Equal(gameCount, result.Count);
		}
	}
}