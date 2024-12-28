using AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.UnitTests.Extensions.GameActivity
{
	public class SessionExtensionsTests
	{
		[Theory]
		[AutoData]
		public void SplitOverMidnight_SplitsSessionToTwo_WhenSessionGoesOverMidnight(
			DateTime dayWithActivity)
		{
			// Arrange
			dayWithActivity = dayWithActivity.Date;
			var hoursInFirstDay = 1;
			var hoursInNextDay = 2;
			var sessions = new List<Session>()
			{
				new Session()
				{
					DateSession = dayWithActivity.AddHours(24 - hoursInFirstDay),
					ElapsedSeconds = (int)TimeSpan.FromHours(hoursInFirstDay + hoursInNextDay).TotalSeconds,
				}
			};

			// Act
			var result = sessions.SplitOverMidnight();

			// Assert
			var daysWithActivity = result.Where(x => x.ElapsedSeconds > 0).ToList();
			Assert.Equal(2, daysWithActivity.Count);
			Assert.Equal(hoursInFirstDay * 60 * 60, daysWithActivity[0].ElapsedSeconds);
			Assert.Equal(hoursInNextDay * 60 * 60, daysWithActivity[1].ElapsedSeconds);
		}

		[Theory]
		[AutoData]
		public void SplitIntoHourly_ReturnsSameSession_WhenSessionIsWithinTheHour(
			DateTime dayWithActivity)
		{
			// Arrange
			dayWithActivity = dayWithActivity.Date;
			var expectedSession = new Session()
			{
				DateSession = dayWithActivity.AddMinutes(30),
				ElapsedSeconds = (int)TimeSpan.FromMinutes(15).TotalSeconds,
			};
			var sessions = new List<Session>
			{
				expectedSession
			};

			// Act
			var result = sessions.SplitIntoHourly();

			// Assert
			var actualSession = Assert.Single(result);
			Assert.Equal(expectedSession, actualSession);
		}

		[Theory]
		[AutoData]
		public void SplitIntoHourly_SplitsSessionToThree_WhenSessionGoesOverTwoHoursAndStartsHalfPast(
			DateTime dayWithActivity)
		{
			// Arrange
			dayWithActivity = dayWithActivity.Date;
			var sessions = new List<Session>
			{
				new Session()
				{
					DateSession = dayWithActivity.AddMinutes(30),
					ElapsedSeconds = (int)TimeSpan.FromHours(2).TotalSeconds,
				}
			};

			// Act
			var result = sessions.SplitIntoHourly().ToList();

			// Assert
			Assert.Equal(3, result.Count);
			Assert.Equal(30 * 60, result[0].ElapsedSeconds);
			Assert.Equal(60 * 60, result[1].ElapsedSeconds);
			Assert.Equal(30 * 60, result[2].ElapsedSeconds);

			Assert.Equal(dayWithActivity.AddMinutes(30), result[0].DateSession);
			Assert.Equal(dayWithActivity.AddHours(1), result[1].DateSession);
			Assert.Equal(dayWithActivity.AddHours(2), result[2].DateSession);
		}

		[Theory]
		[AutoData]
		public void SplitIntoHourly_SplitsSessionToTwo_WhenSessionGoesOverAnHourAndStartsFifteenPast(
			DateTime dayWithActivity)
		{
			// Arrange
			dayWithActivity = dayWithActivity.Date;
			var sessions = new List<Session>
			{
				new Session()
				{
					DateSession = dayWithActivity.AddMinutes(15),
					ElapsedSeconds = (int)TimeSpan.FromHours(1).TotalSeconds,
				}
			};

			// Act
			var result = sessions.SplitIntoHourly().ToList();

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Equal(45 * 60, result[0].ElapsedSeconds);
			Assert.Equal(15 * 60, result[1].ElapsedSeconds);

			Assert.Equal(dayWithActivity.AddMinutes(15), result[0].DateSession);
			Assert.Equal(dayWithActivity.AddHours(1), result[1].DateSession);
		}
	}
}