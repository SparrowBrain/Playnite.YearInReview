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
	}
}