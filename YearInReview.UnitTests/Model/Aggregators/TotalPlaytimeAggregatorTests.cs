using System.Collections.Generic;
using System.Linq;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators;

namespace YearInReview.UnitTests.Model.Aggregators
{
	public class TotalPlaytimeAggregatorTests
	{
		[Theory]
		[AutoFakeItEasyData]
		public void GetTotalPlaytime_ReturnsTotalPlaytime(
			List<Activity> activities,
			TotalPlaytimeAggregator sut)
		{
			// Arrange
			var expected = activities.Sum(a => a.Items.Sum(s => s.ElapsedSeconds));

			// Act
			var result = sut.GetTotalPlaytime(activities);

			// Assert
			Assert.Equal(expected, result);
		}
	}
}