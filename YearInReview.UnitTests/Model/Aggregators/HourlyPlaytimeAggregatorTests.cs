using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators;

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
	}

	public class HourlyPlaytimeAggregator
	{
		public IDictionary<int, int> GetHours(IReadOnlyCollection<Activity> activities)
		{
			var result = new Dictionary<int, int>();
			for (var i = 0; i < 24; i++)
			{
				result.Add(i, 0);
			}

			return result;
		}
	}
}