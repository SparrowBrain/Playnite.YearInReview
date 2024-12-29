using System.Collections.Generic;
using System.Linq;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.Model.Aggregators
{
	public class HourlyPlaytimeAggregator : IHourlyPlaytimeAggregator
	{
		public IDictionary<int, int> GetHours(IReadOnlyCollection<Activity> activities)
		{

			var splitSessions = activities
				.SelectMany(x => x.Items)
				.SplitIntoHourly()
				.ToList();

			var result = new Dictionary<int, int>();
			for (var hour = 0; hour < 24; hour++)
			{
				var elapsedSeconds = splitSessions
					.Where(x => x.DateSession.Hour == hour)
					.Sum(x => x.ElapsedSeconds);

				result.Add(hour, elapsedSeconds);
			}

			return result;
		}
	}
}