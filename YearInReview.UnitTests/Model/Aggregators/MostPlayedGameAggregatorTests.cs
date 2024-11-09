using AutoFixture.Xunit2;
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
		public void GetMostPlayedGame_ReturnsNull_WhenNoActivities(
			MostPlayedGameAggregator sut)
		{
			// Arrange
			var activities = new List<Activity>();

			// Act
			var result = sut.GetMostPlayedGame(activities);

			// Assert
			Assert.Null(result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetMostPlayedGame_ReturnsGameWithMostElapsedTime_WhenActivitiesProvided(
			[Frozen] Fake<IGameDatabaseAPI> gameDatabaseApiFake,
			[Frozen] Fake<IPlayniteAPI> playniteApiFake,
			List<Activity> activities,
			MostPlayedGameAggregator sut)
		{
			// Arrange
			var mostPlayedGame = activities.Last();
			mostPlayedGame.Items.Add(new Session { ElapsedSeconds = int.MaxValue });
			var games = activities.Select(x => new Game { Id = x.Id }).ToList();

			gameDatabaseApiFake.CallsTo(x => x.Games).Returns(new TestableItemCollection<Game>(games));
			playniteApiFake.CallsTo(x => x.Database).Returns(gameDatabaseApiFake.FakedObject);

			// Act
			var result = sut.GetMostPlayedGame(activities);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(mostPlayedGame.Id, result.Id);
		}
	}
}