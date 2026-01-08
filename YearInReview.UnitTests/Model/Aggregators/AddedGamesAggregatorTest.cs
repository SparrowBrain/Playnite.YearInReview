using AutoFixture.Xunit2;
using FakeItEasy;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using TestTools.Shared;
using Xunit;
using YearInReview.Model.Aggregators;

namespace YearInReview.UnitTests.Model.Aggregators
{
	public class AddedGamesAggregatorTest
	{
		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_Empty_WhenGamesNull(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			AddedGamesAggregator sut
		)
		{
			// Arrange
			List<Game> games = null;
			MockPlayniteApiGames(gameDatabaseApiFake, playniteApiFake, games);

			// Act
			var result = sut.GetAddedGames(0);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_Empty_WhenContainsNullGame(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			AddedGamesAggregator sut
		)
		{
			// Arrange
			List<Game> games = new List<Game> { null };
			MockPlayniteApiGames(gameDatabaseApiFake, playniteApiFake, games);

			// Act
			var result = sut.GetAddedGames(0);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_Empty_WhenGamesEmpty(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			AddedGamesAggregator sut
		)
		{
			// Arrange
			var games = new List<Game>();
			MockPlayniteApiGames(gameDatabaseApiFake, playniteApiFake, games);

			// Act
			var result = sut.GetAddedGames(0);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_Empty_WhenGameAddedDateNull(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			AddedGamesAggregator sut
		)
		{
			// Arrange
			var game = new Game { Added = null };
			var games = new List<Game> { game };
			MockPlayniteApiGames(gameDatabaseApiFake, playniteApiFake, games);

			// Act
			var result = sut.GetAddedGames(2025);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_ReturnsGame_WhenGameMatching(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			AddedGamesAggregator sut
		)
		{
			// Arrange
			var date = new DateTime(2025, 1, 1);
			var game = new Game { Added = date };
			var games = new List<Game> { game };
			MockPlayniteApiGames(gameDatabaseApiFake, playniteApiFake, games);

			// Act
			var result = sut.GetAddedGames(date.Year);

			// Assert
			var assertedAddedGame = Assert.Single(result);
			Assert.Equal(game, assertedAddedGame.Game);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_Empty_WhenGameNotMatching(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			AddedGamesAggregator sut
		)
		{
			// Arrange
			var games = new List<Game>();
			var game = new Game { Added = new DateTime(2025, 1, 1) };
			games.Add(game);
			MockPlayniteApiGames(gameDatabaseApiFake, playniteApiFake, games);

			// Act
			var result = sut.GetAddedGames(2020);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_ReturnsOneCorrect_WhenOneMatchingOneNotMatchingGame(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			AddedGamesAggregator sut
		)
		{
			// Arrange
			var dateMatching = new DateTime(2025, 1, 1);
			var dateNotMatching = new DateTime(2000, 1, 1);
			var matchingGame = new Game { Added = dateMatching };
			var notMatchingGame = new Game { Added = dateNotMatching };

			var games = new List<Game> { matchingGame, notMatchingGame };
			MockPlayniteApiGames(gameDatabaseApiFake, playniteApiFake, games);

			// Act
			var result = sut.GetAddedGames(dateMatching.Year);

			// Assert
			var assertedAddedGame = Assert.Single(result);
			Assert.Equal(matchingGame, assertedAddedGame.Game);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_ReturnsCorrectGames_WhenMultipleGamesWithSameAddedDate(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			AddedGamesAggregator sut
		)
		{
			// Arrange
			var date = new DateTime(2025, 1, 1);
			var game1 = new Game { Added = date };
			var game2 = new Game { Added = date };
			var game3 = new Game { Added = date };

			var games = new List<Game> { game1, game2, game3 };
			MockPlayniteApiGames(gameDatabaseApiFake, playniteApiFake, games);

			// Act
			var result = sut.GetAddedGames(date.Year);

			// Assert
			Assert.Equal(3, result.Count);
			Assert.Contains(result, r => Equals(r.Game, game1) && r.AddedDate == date);
			Assert.Contains(result, r => Equals(r.Game, game2) && r.AddedDate == date);
			Assert.Contains(result, r => Equals(r.Game, game3) && r.AddedDate == date);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_ReturnsOnlyMatching_WhenIncludesGamesAddedOnBoundaryDates(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			AddedGamesAggregator sut
		)
		{
			// Arrange
			const int year = 2025;
			var firstDayOfYear = new DateTime(year, 1, 1);
			var lastDayOfYear = new DateTime(year, 12, 31);
			var gameOnFirstDay = new Game { Added = firstDayOfYear };
			var gameOnLastDay = new Game { Added = lastDayOfYear };
			var gameOutsideYear1 = new Game { Added = new DateTime(year + 1, 1, 1) };
			var gameOutsideYear2 = new Game { Added = new DateTime(year - 1, 12, 31) };

			var games = new List<Game> { gameOnFirstDay, gameOnLastDay, gameOutsideYear1, gameOutsideYear2 };
			MockPlayniteApiGames(gameDatabaseApiFake, playniteApiFake, games);

			// Act
			var result = sut.GetAddedGames(year);

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Contains(result, r => Equals(r.Game, gameOnFirstDay) && r.AddedDate == firstDayOfYear);
			Assert.Contains(result, r => Equals(r.Game, gameOnLastDay) && r.AddedDate == lastDayOfYear);
			Assert.DoesNotContain(result, r => Equals(r.Game, gameOutsideYear1));
			Assert.DoesNotContain(result, r => Equals(r.Game, gameOutsideYear2));
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_UnknownSource_WhenSourceWithIdNotFound(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			Guid sourceId,
			AddedGamesAggregator sut
		)
		{
			// Arrange
			var date = new DateTime(2025, 1, 1);
			var game = new Game { Added = date, SourceId = sourceId };

			var games = new List<Game> { game };
			MockPlayniteApiGames(gameDatabaseApiFake, playniteApiFake, games);

			// Act
			var result = sut.GetAddedGames(date.Year);

			// Assert
			var addedGame = Assert.Single(result);
			Assert.Equal("???", addedGame.Source.Name);
			Assert.Equal(game.SourceId, addedGame.Source.Id);
		}

		private static void MockPlayniteApiGames(
			IGameDatabaseAPI gameDatabaseApiFake,
			IPlayniteAPI playniteApiFake,
			List<Game> games
		)
		{
			A.CallTo(() => gameDatabaseApiFake.Games).Returns(new TestableItemCollection<Game>(games));
			A.CallTo(() => playniteApiFake.Database).Returns(gameDatabaseApiFake);
		}
	}
}