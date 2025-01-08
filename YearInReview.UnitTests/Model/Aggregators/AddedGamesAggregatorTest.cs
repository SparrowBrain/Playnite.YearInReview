using System;
using System.Collections.Generic;
using System.Linq;
using Playnite.SDK.Models;
using TestTools.Shared;
using Xunit;
using YearInReview.Model.Aggregators;

namespace YearInReview.UnitTests.Model.Aggregators
{
	public class AddedGamesAggregatorTest
	{
		private static IEnumerable<DateTime> GetAllDatesOfYear(int year)
		{
			return Enumerable.Range(1, 12).SelectMany(month => Enumerable.Range(1, DateTime.DaysInMonth(year, month)).Select(day => new DateTime(year, month, day)));
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_Empty_WhenGamesNull(AddedGamesAggregator sut)
		{
			// Arrange
			List<Game> games = null;

			// Act
			var result = sut.GetAddedGames(games, 0);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_Empty_WhenContainsNullGame(AddedGamesAggregator sut)
		{
			// Arrange
			List<Game> games = new List<Game>() { null };

			// Act
			var result = sut.GetAddedGames(games, 0);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_Empty_WhenGamesEmpty(AddedGamesAggregator sut)
		{
			// Arrange
			var games = new List<Game>();

			// Act
			var result = sut.GetAddedGames(games, 0);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAdded_Empty_WhenYearNegative(AddedGamesAggregator sut)
		{
			// Arrange
			var games = new List<Game>();

			// Act
			var result = sut.GetAddedGames(games, -1);

			// Assert
			Assert.Empty(result);
		}
		
		[Theory] 
		[AutoFakeItEasyData] 
		public void GetAddedGames_Empty_WhenGameAddedDateNull(AddedGamesAggregator sut)
		{
			// Arrange
			var games = new List<Game>();
			var game = new Game { Added = null };
			games.Add(game);

			// Act
			var result = sut.GetAddedGames(games, 2025);

			// Assert
			Assert.Empty(result);
		}

		[Theory] 
		[AutoFakeItEasyData] 
		public void GetAddedGames_ReturnsGame_WhenGameMatching(AddedGamesAggregator sut)
		{
			// Arrange
			var date = new DateTime(2025, 1, 1);
			var games = new List<Game>();
			var game = new Game { Added = date };
			games.Add(game);

			// Act
			var result = sut.GetAddedGames(games, date.Year);

			// Assert
			Assert.Single(result);
			Assert.True(game.Added == result.First().AddedDate);
			Assert.True(Equals(game, result.First().Game));
		}

		[Theory] 
		[AutoFakeItEasyData] 
		public void GetAddedGames_Empty_WhenGameNotMatching(AddedGamesAggregator sut)
		{
			// Arrange
			var games = new List<Game>();
			var game = new Game { Added = new DateTime(2025, 1, 1) };
			games.Add(game);

			// Act
			var result = sut.GetAddedGames(games, 2020);

			// Assert
			Assert.Empty(result);
		}

		[Theory] 
		[AutoFakeItEasyData] 
		public void GetAddedGames_ReturnsOneCorrect_WhenOneMatchingOneNotMatchingGame(AddedGamesAggregator sut)
		{
			// Arrange
			var dateMatching = new DateTime(2025, 1, 1);
			var dateNotMatching = new DateTime(2000, 1, 1);
			var matchingGame = new Game { Added = dateMatching };
			var notMatchingGame = new Game { Added = dateNotMatching };
			
			var games = new List<Game> { matchingGame, notMatchingGame };

			// Act
			var result = sut.GetAddedGames(games, dateMatching.Year);

			// Assert
			Assert.Single(result);
			Assert.True(dateMatching == result.First().AddedDate);
			Assert.True(Equals(matchingGame, result.First().Game));
		}

		[Theory] 
		[AutoFakeItEasyData] 
		public void GetAddedGames_ReturnsAllCorrect_WhenAllGamesMatching_AllDaysInYear(AddedGamesAggregator sut)
		{
			// Arrange
			const int year = 2025;
			// all dates of a full year in one list
			var allDatesOfYear = GetAllDatesOfYear(year);
			// create games with added date for each day of the year
			var games = allDatesOfYear.Select(date => new Game { Added = date }).ToList();
			
			// Act
			var result = sut.GetAddedGames(games, year);
			
			// Assert
			Assert.Equal(games.Count, result.Count);
		}

		[Theory] 
		[AutoFakeItEasyData] 
		public void GetAddedGames_ReturnsAllCorrect_WhenAllGamesMatching_AllDaysInLeapYear(AddedGamesAggregator sut)
		{
			// Arrange
			const int year = 2024;
			// all dates of a full leap year in one list
			var allDatesOfYear = GetAllDatesOfYear(year);
			// create games with added date for each day of the year
			var games = allDatesOfYear.Select(date => new Game { Added = date }).ToList();
			
			// Act
			var result = sut.GetAddedGames(games, year);
			
			// Assert
			Assert.Equal(games.Count, result.Count);
		}
		
		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_ReturnsCorrectGames_WhenMultipleGamesWithSameAddedDate(AddedGamesAggregator sut)
		{
			// Arrange
			var date = new DateTime(2025, 1, 1);
			var game1 = new Game { Added = date };
			var game2 = new Game { Added = date };
			var game3 = new Game { Added = date };

			var games = new List<Game> { game1, game2, game3 };

			// Act
			var result = sut.GetAddedGames(games, date.Year);

			// Assert
			Assert.Equal(3, result.Count);
			Assert.Contains(result, r => Equals(r.Game, game1) && r.AddedDate == date);
			Assert.Contains(result, r => Equals(r.Game, game2) && r.AddedDate == date);
			Assert.Contains(result, r => Equals(r.Game, game3) && r.AddedDate == date);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_ReturnsOnlyMatching_WhenIncludesGamesAddedOnBoundaryDates(AddedGamesAggregator sut)
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

			// Act
			var result = sut.GetAddedGames(games, year);

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Contains(result, r => Equals(r.Game, gameOnFirstDay) && r.AddedDate == firstDayOfYear);
			Assert.Contains(result, r => Equals(r.Game, gameOnLastDay) && r.AddedDate == lastDayOfYear);
			Assert.DoesNotContain(result, r => Equals(r.Game, gameOutsideYear1));
			Assert.DoesNotContain(result, r => Equals(r.Game, gameOutsideYear2));
		}
		
		[Theory]
		[AutoFakeItEasyData]
		public void GetAddedGames_ReturnsCorrectGames_WhenIncludesDifferentNullValues(AddedGamesAggregator sut)
		{
			// Arrange
			const int year = 2025;
			var game1 = new Game { Added = null };
			var game2 = new Game { Added = null };
			Game game3 = null;
			var date = new DateTime(year, 1, 1);
			var game4 = new Game { Added = date };
			var game5 = new Game { Added = date };

			var games = new List<Game> { game1, game2, game3, game4, game5 };

			// Act
			var result = sut.GetAddedGames(games, year);

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Contains(result, r => Equals(r.Game, game4) && r.AddedDate == date);
			Assert.Contains(result, r => Equals(r.Game, game5) && r.AddedDate == date);
			Assert.DoesNotContain(result, r => Equals(r.Game, game1));
			Assert.DoesNotContain(result, r => Equals(r.Game, game2));
		}
	}
}