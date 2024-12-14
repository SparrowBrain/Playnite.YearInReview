using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Aggregators
{
	public class PlaytimeCalendarAggregator
	{
		private readonly IPlayniteAPI _api;

		public PlaytimeCalendarAggregator(IPlayniteAPI api)
		{
			_api = api;
		}

		public PlaytimeCalendar GetCalendar(int year, IReadOnlyList<Activity> activities)
		{
			var days = new List<CalendarDay>();
			var startDate = new DateTime(year, 1, 1);
			var date = startDate;
			var splitActivities = activities.Select(x => new Activity()
			{
				Id = x.Id,
				Name = x.Name,
				Items = x.Items.SplitOverMidnight().ToList()
			}).ToList();

			while (date.Year == year)
			{
				var dayActivities = splitActivities.Where(x => x.Items.Any(i => i.DateSession.Date == date))
					.Select(x => new Activity
					{
						Id = x.Id,
						Items = x.Items.Where(i => i.DateSession.Date == date).ToList(),
						Name = x.Name
					}).ToList();

				days.Add(new CalendarDay()
				{
					Date = date,
					TotalPlaytime = dayActivities.SelectMany(x => x.Items).Sum(x => x.ElapsedSeconds),
					Games = dayActivities.Select(x => new GameWithTime()
					{
						TimePlayed = x.Items.Sum(i => i.ElapsedSeconds),
						Game = _api.Database.Games.FirstOrDefault(g => g.Id == x.Id) ?? new Game() { Id = x.Id, Name = x.Name }
					}
					).ToList()
				});

				date = date.AddDays(1);
			}

			return new PlaytimeCalendar()
			{
				Days = days.ToDictionary(x => x.Date, x => x)
			};
		}
	}
}