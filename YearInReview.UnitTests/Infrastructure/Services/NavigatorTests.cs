using AutoFixture.Xunit2;
using FakeItEasy;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TestTools.Shared;
using Xunit;
using YearInReview.Infrastructure.Services;

namespace YearInReview.UnitTests.Infrastructure.Services
{
	public class NavigatorTests
	{
		[Theory]
		[AutoFakeItEasyData]
		public void ShowGame_ShowsGame_WhenGameWithIdExists(
			[Frozen] IPlayniteAPI playniteApi,
			[Frozen] IGameDatabaseAPI gameDatabaseApi,
			List<Game> games,
			string name,
			Navigator sut)
		{
			// Arrange
			var id = games.Last().Id;
			MockPlayniteApiGames(gameDatabaseApi, playniteApi, games);

			// Act
			sut.ShowGame(id, name);

			// Assert
			A.CallTo(() => playniteApi.MainView.SelectGame(id)).MustHaveHappenedOnceExactly();
			A.CallTo(() => playniteApi.MainView.SwitchToLibraryView()).MustHaveHappenedOnceExactly();
		}

		[Theory]
		[AutoFakeItEasyData]
		public void ShowGame_ShowsGame_WhenGameWithNameExists(
			[Frozen] IPlayniteAPI playniteApi,
			[Frozen] IGameDatabaseAPI gameDatabaseApi,
			List<Game> games,
			Guid id,
			Navigator sut)
		{
			// Arrange
			var expectedGame = games.Last();
			var name = expectedGame.Name;
			MockPlayniteApiGames(gameDatabaseApi, playniteApi, games);

			// Act
			sut.ShowGame(id, name);

			// Assert
			A.CallTo(() => playniteApi.MainView.SelectGame(expectedGame.Id)).MustHaveHappenedOnceExactly();
			A.CallTo(() => playniteApi.MainView.SwitchToLibraryView()).MustHaveHappenedOnceExactly();
		}

		[Theory]
		[AutoFakeItEasyData]
		public void ShowGame_PrefersNotHiddenGames_WhenMultipleGamesWithNameExists(
			[Frozen] IPlayniteAPI playniteApi,
			[Frozen] IGameDatabaseAPI gameDatabaseApi,
			List<Game> games,
			Guid id,
			string name,
			Navigator sut)
		{
			// Arrange
			games.ForEach(x =>
			{
				x.Name = name;
				x.Hidden = true;
			});
			var expectedGame = games.Skip(1).Take(1).Single();
			expectedGame.Hidden = false;
			MockPlayniteApiGames(gameDatabaseApi, playniteApi, games);

			// Act
			sut.ShowGame(id, name);

			// Assert
			A.CallTo(() => playniteApi.MainView.SelectGame(expectedGame.Id)).MustHaveHappenedOnceExactly();
			A.CallTo(() => playniteApi.MainView.SwitchToLibraryView()).MustHaveHappenedOnceExactly();
		}

		[Theory]
		[AutoFakeItEasyData]
		public void ShowGame_DoesNotNavigate_WhenNoGameExists(
			[Frozen] IPlayniteAPI playniteApi,
			[Frozen] IGameDatabaseAPI gameDatabaseApi,
			List<Game> games,
			Guid id,
			string name,
			Navigator sut)
		{
			// Arrange
			MockPlayniteApiGames(gameDatabaseApi, playniteApi, games);

			// Act
			sut.ShowGame(id, name);

			// Assert
			A.CallTo(() => playniteApi.MainView.SwitchToLibraryView()).MustNotHaveHappened();
		}

		private static void MockPlayniteApiGames(
			IGameDatabaseAPI gameDatabaseApi,
			IPlayniteAPI playniteApi,
			List<Game> games)
		{
			A.CallTo(() => gameDatabaseApi.Games).Returns(new TestableItemCollection<Game>(games));
			A.CallTo(() => playniteApi.Database).Returns(gameDatabaseApi);
		}
	}
}