using System;
using System.Collections.Generic;
using System.Linq;
using YearInReview.Extensions.GameActivity;

namespace YearInReview.Model.Filters
{
	public class SpecificYearActivityFilter : ISpecificYearActivityFilter
	{
		public IReadOnlyCollection<Activity> GetActivityForYear(int year, IReadOnlyCollection<Activity> allActivities)
		{
			return allActivities.Select(x => new Activity
			{
				Id = x.Id,
				Name = x.Name,
				Items = x.Items.Where(s => s.DateSession.Year == year).Select(i => new Session()
				{
					DateSession = i.DateSession,
					ElapsedSeconds = i.DateSession.AddSeconds(i.ElapsedSeconds).Year == year
						? i.ElapsedSeconds
						: (int)(new DateTime(year + 1, 1, 1) - i.DateSession).TotalSeconds,
					IdConfiguration = i.IdConfiguration,
					PlatformIDs = i.PlatformIDs,
					PlatfromId = i.PlatfromId,
					SourceId = i.SourceId
				}).ToList()
			}).Where(x => x.Items.Any()).ToList();
		}
	}
}