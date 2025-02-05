using System.Collections.Generic;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Filters;

namespace YearInReview.UnitTests.Model.Filters
{
	public class EmptyActivityFilterTests
	{
		[Theory]
		[AutoFakeItEasyData]
		public void RemoveEmpty_AllActivitiesHaveTime_ReturnsAllActivities(
			List<Activity> activities,
			EmptyActivityFilter sut)
		{
			// Act
			var result = sut.RemoveEmpty(activities);

			// Assert
			Assert.Equivalent(activities, result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void RemoveEmpty_ActivitiesHaveNoSessions_ReturnsEmptyCollection(
			List<Activity> activities,
			EmptyActivityFilter sut)
		{
			// Arrange
			activities.ForEach(x => x.Items.Clear());

			// Act
			var result = sut.RemoveEmpty(activities);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void RemoveEmpty_ActivitiesHaveZeroTimeSessions_ReturnsEmptyCollection(
			List<Activity> activities,
			EmptyActivityFilter sut)
		{
			// Arrange
			activities.ForEach(x => x.Items.ForEach(i => i.ElapsedSeconds = 0));

			// Act
			var result = sut.RemoveEmpty(activities);

			// Assert
			Assert.Empty(result);
		}
	}
}