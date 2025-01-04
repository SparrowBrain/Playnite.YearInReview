using AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Filters;

namespace YearInReview.UnitTests.Model.Filters
{
	public class SpecificYearActivityFilterTests
	{
		[Theory]
		[AutoData]
		public void GetActivity_ReturnsEmptyList_WhenNoSessionsForYearExist(
			List<Activity> allActivities,
			SpecificYearActivityFilter sut)
		{
			// Arrange
			var year = 5000;

			// Act
			var activity = sut.GetActivityForYear(year, allActivities);

			// Assert
			Assert.Empty(activity);
		}

		[Theory]
		[AutoData]
		public void GetActivity_ReturnsAllEquivalentActivities_WhenAllSessionsAreInTheYear(
			List<Activity> initialActivities,
			int year,
			SpecificYearActivityFilter sut)
		{
			// Arrange
			foreach (var activity in initialActivities)
			{
				foreach (var session in activity.Items)
				{
					var oldDate = session.DateSession;
					session.DateSession = new DateTime(year, oldDate.Month, oldDate.Day, oldDate.Hour, oldDate.Minute, oldDate.Second);
				}
			}

			// Act
			var resultActivities = sut.GetActivityForYear(year, initialActivities);

			// Assert
			Assert.Equivalent(initialActivities, resultActivities);
		}

		[Theory]
		[AutoData]
		public void GetActivityForYear_ReturnsShorterSession_WhenSessionGoesOverNewYear(
			Activity initialActivity,
			Session initialSession,
			int year,
			SpecificYearActivityFilter sut)
		{
			// Arrange
			initialSession.DateSession = new DateTime(year, 12, 31, 23, 0, 0);
			initialSession.ElapsedSeconds = (int)TimeSpan.FromHours(2).TotalSeconds;
			initialActivity.Items = new List<Session> { initialSession };

			// Act
			var result = sut.GetActivityForYear(year, new List<Activity> { initialActivity });

			// Assert
			var actualActivity = Assert.Single(result);
			var actualSession = Assert.Single(actualActivity.Items);
			Assert.Equal(60 * 60, actualSession.ElapsedSeconds);
		}
	}
}