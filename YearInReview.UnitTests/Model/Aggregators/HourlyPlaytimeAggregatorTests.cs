using System;
using System.Collections.Generic;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.UnitTests.Model.Aggregators
{
	public class HourlyPlaytimeAggregatorTests
	{
		[Theory]
		[AutoFakeItEasyData]
		public void GetHours_EmptyActivities_AllPlaytimeIsZero(
			HourlyPlaytimeAggregator sut)
		{
			// Arrange
			var activities = new List<Activity>();

			// Act
			var result = sut.GetHours(activities);

			// Assert
			Assert.Equal(24, result.Count);
			Assert.All(result.Values, x => Assert.Equal(0, x));
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetHours_ThreeSessionsInTheSameHour_SumsUpSessionsForThatHour(
			Activity activity,
			int hourParam,
			HourlyPlaytimeAggregator sut)
		{
			// Arrange
			var hour = hourParam % 24;
			activity.Items.ForEach(x => x.DateSession = new DateTime(x.DateSession.Year, x.DateSession.Month, x.DateSession.Day, hour, 0, 0));
			activity.Items.ForEach(x => x.ElapsedSeconds = 3600);

			// Act
			var result = sut.GetHours(new List<Activity>() { activity });

			// Assert
			var hourWithActivity = result[hour];
			Assert.Equal(3600 * activity.Items.Count, hourWithActivity);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void GetHours_SessionIsSplitBetweenHours_BothHoursGetElapsedTime(
			Activity activity,
			int hourParam,
			HourlyPlaytimeAggregator sut)
		{
			// Arrange
			var firstHour = hourParam % 24;
			var secondHour = (hourParam + 1) % 24;
			activity.Items.ForEach(x => x.DateSession = new DateTime(x.DateSession.Year, x.DateSession.Month, x.DateSession.Day, firstHour, 15, 0));
			activity.Items.ForEach(x => x.ElapsedSeconds = 3600);

			// Act
			var result = sut.GetHours(new List<Activity>() { activity });

			// Assert
			var firstHourWithActivity = result[firstHour];
			var secondHourWithActivity = result[secondHour];
			Assert.Equal(45 * 60 * activity.Items.Count, firstHourWithActivity);
			Assert.Equal(15 * 60 * activity.Items.Count, secondHourWithActivity);
		}
	}
}