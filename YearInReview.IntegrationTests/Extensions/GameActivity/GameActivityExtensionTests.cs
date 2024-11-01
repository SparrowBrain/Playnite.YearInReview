using AutoFixture.Xunit2;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.IntegrationTests.Extensions.GameActivity
{
	public class GameActivityExtensionTests
	{
		private const string TestDataPath = @"Extensions\GameActivity\TestData";
		private const string ExtensionsDataPath = @"Extensions\GameActivity\ExtensionsData";

		public GameActivityExtensionTests()
		{
			Utils.CopyDirectory(TestDataPath, ExtensionsDataPath, true);
		}

		[Theory, AutoData]
		public async Task GetActivityForGames_ReturnsEmptyCollection_ActivityPathDoesNotExist(
			IEnumerable<Game> games)
		{
			// Arrange
			CleanUpExtensionsDataPath();
			var sut = GameActivityExtension.Create(ExtensionsDataPath);

			// Act
			var activity = await sut.GetActivityForGames(games);

			// Assert
			Assert.Empty(activity);
		}

		[Theory, AutoData]
		public async Task GetActivityForGames_ReturnsEmptyCollection_WhenNoActivityFilesExistsForGames(
			IEnumerable<Game> games)
		{
			// Arrange
			var sut = GameActivityExtension.Create(ExtensionsDataPath);

			// Act
			var activity = await sut.GetActivityForGames(games);

			// Assert
			Assert.Empty(activity);
		}

		[Theory, AutoData]
		public async Task GetActivityForGames_Activity_WhenActivityFileExistsForGame(
			IEnumerable<Game> games)
		{
			// Arrange
			var sut = GameActivityExtension.Create(ExtensionsDataPath);
			var gameWithActivity = games.Last();
			gameWithActivity.Id = Guid.Parse("f1044699-4b97-4968-868b-e871e37ae1f3");

			// Act
			var activity = await sut.GetActivityForGames(games);

			// Assert
			var foundActivity = Assert.Single(activity);
			Assert.Equal(gameWithActivity.Id, foundActivity.Id);
			var item = Assert.Single(foundActivity.Items);
			Assert.Equal(DateTime.Parse("2023-02-12T13:40:18.2429471Z").ToUniversalTime(), item.DateSession);
			Assert.Equal((ulong)67, item.ElapsedSeconds);
		}

		private static void CleanUpExtensionsDataPath()
		{
			foreach (var dir in Directory.GetDirectories(ExtensionsDataPath))
			{
				Directory.Delete(dir, true);
			}
		}
	}
}