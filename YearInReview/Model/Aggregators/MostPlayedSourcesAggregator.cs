using System.Collections.Generic;
using System.Linq;
using Playnite.SDK;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators.Data;
using YearInReview.Model.Reports;

namespace YearInReview.Model.Aggregators
{
	public class MostPlayedSourcesAggregator : IMostPlayedSourcesAggregator
	{
		private readonly IPlayniteAPI _playniteApi;

		public MostPlayedSourcesAggregator(IPlayniteAPI playniteApi)
		{
			_playniteApi = playniteApi;
		}

		public IReadOnlyCollection<SourceWithTime> GetMostPlayedSources(IReadOnlyCollection<Activity> activities)
		{
			return activities.SelectMany(x => x.Items)
				.GroupBy(x => x.SourceId)
				.Select(x => new SourceWithTime
				{
					Source = _playniteApi.Database.Sources.FirstOrDefault(s => s.Id == x.Key),
					TimePlayed = x.Sum(s => s.ElapsedSeconds),
				})
				.Where(x => x.Source != null)
				.OrderByDescending(x => x.TimePlayed)
				.ToList();
		}
	}
}