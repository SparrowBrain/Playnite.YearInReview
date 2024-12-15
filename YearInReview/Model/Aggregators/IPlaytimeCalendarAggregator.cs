using System;
using System.Collections.Generic;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Aggregators
{
	public interface IPlaytimeCalendarAggregator
	{
		IDictionary<DateTime, CalendarDay> GetCalendar(int year, IReadOnlyCollection<Activity> activities);
	}
}