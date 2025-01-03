using AutoFixture.Xunit2;
using FakeItEasy;
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
	public class PlaytimeCalendarAggregatorTests
	{
		[Theory]
		[InlineAutoFakeItEasyData(2024, 366)]
		[InlineAutoFakeItEasyData(2025, 365)]
		public void GetCalendar_ReturnsEmptyDays_WhenNoActivityListIsEmpty(
			int year,
			int expectedDayCount,
			PlaytimeCalendarAggregator sut)
		{
			// Arrange
			var activities = new List<Activity>();

			// Act
			var result = sut.GetCalendar(year, activities);

			// Assert
			Assert.Equal(expectedDayCount, result.Count);
			Assert.All(result.Values, x => Assert.Equal(0, x.TotalPlaytime));
			Assert.All(result.Values, x => Assert.Empty(x.Games));
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetCalendar_ReturnsADayWithPlaytimeAndGames_WhenActivityExistOnThatDay(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			DateTime dayWithActivity,
			List<Game> games,
			PlaytimeCalendarAggregator sut)
		{
			// Arrange
			dayWithActivity = dayWithActivity.Date;
			var year = dayWithActivity.Year;
			var activities = games.Select(x => new Activity()
			{
				Id = x.Id,
				Items = new List<Session>()
								{
									new Session()
									{
										DateSession = dayWithActivity,
										ElapsedSeconds = 100,
									}
								}
			}).ToList();
			A.CallTo(() => gameDatabaseApiFake.Games).Returns(new TestableItemCollection<Game>(games));
			A.CallTo(() => playniteApiFake.Database).Returns(gameDatabaseApiFake);

			// Act
			var result = sut.GetCalendar(year, activities);

			// Assert
			var dayStats = result[dayWithActivity];
			Assert.Equal(activities.Sum(x => x.Items.Sum(i => i.ElapsedSeconds)),
				dayStats.TotalPlaytime);
			Assert.Equal(activities.Count, dayStats.Games.Count);
			Assert.All(dayStats.Games,
				x => Assert.Equal(activities.Single(a => a.Id == x.Game.Id).Items.Sum(i => i.ElapsedSeconds),
					x.TimePlayed));
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetCalendar_OrdersGamesByPlaytime_WhenSeveralActivitiesExistOnThatDay(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			DateTime dayWithActivity,
			List<Game> games,
			PlaytimeCalendarAggregator sut)
		{
			// Arrange
			dayWithActivity = dayWithActivity.Date;
			var year = dayWithActivity.Year;
			var activities = games.Select(x => new Activity()
			{
				Id = x.Id,
				Items = new List<Session>()
								{
									new Session()
									{
										DateSession = dayWithActivity,
										ElapsedSeconds = (int)x.Playtime,
									}
								}
			}).ToList();
			A.CallTo(() => gameDatabaseApiFake.Games).Returns(new TestableItemCollection<Game>(games));
			A.CallTo(() => playniteApiFake.Database).Returns(gameDatabaseApiFake);

			// Act
			var result = sut.GetCalendar(year, activities);

			// Assert
			var dayStats = result[dayWithActivity];
			Assert.Equal(games.OrderByDescending(x => x.Playtime).First().Id, dayStats.Games.First().Game.Id);
			Assert.Equal(games.OrderByDescending(x => x.Playtime).Last().Id, dayStats.Games.Last().Game.Id);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetCalendar_SplitsActivityToTwo_WhenActivityGoesOverMidnight(
			[Frozen] IGameDatabaseAPI gameDatabaseApiFake,
			[Frozen] IPlayniteAPI playniteApiFake,
			DateTime dayWithActivity,
			Game game,
			PlaytimeCalendarAggregator sut)
		{
			// Arrange
			dayWithActivity = dayWithActivity.Date;
			var year = dayWithActivity.Year;
			var hoursInFirstDay = 1;
			var hoursInNextDay = 2;
			var activities = new List<Activity>()
							{
								new Activity()
								{
									Id = game.Id,
									Name = game.Name,
									Items = new List<Session>()
									{
										new Session()
										{
											DateSession = dayWithActivity.AddHours(24 - hoursInFirstDay),
											ElapsedSeconds = (int) TimeSpan.FromHours(hoursInFirstDay + hoursInNextDay).TotalSeconds,
										}
									}
								}
							};
			A.CallTo(() => gameDatabaseApiFake.Games).Returns(new TestableItemCollection<Game>(new List<Game>() { game }));
			A.CallTo(() => playniteApiFake.Database).Returns(gameDatabaseApiFake);

			// Act
			var result = sut.GetCalendar(year, activities);

			// Assert
			var daysWithActivity = result.Where(x => x.Value.TotalPlaytime > 0).ToList();
			Assert.Equal(2, daysWithActivity.Count);
			Assert.Equal(hoursInFirstDay * 60 * 60, daysWithActivity[0].Value.TotalPlaytime);
			Assert.Equal(hoursInNextDay * 60 * 60, daysWithActivity[1].Value.TotalPlaytime);
		}
	}
}