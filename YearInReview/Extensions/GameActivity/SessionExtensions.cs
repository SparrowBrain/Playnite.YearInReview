using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace YearInReview.Extensions.GameActivity
{
	public static class SessionExtensions
	{
		public static IEnumerable<Session> SplitOverMidnight(this IEnumerable<Session> sessions)
		{
			foreach (var session in sessions)
			{
				var startTime = session.DateSession;
				var endTime = startTime.AddSeconds(session.ElapsedSeconds);

				if (startTime.Date == endTime.Date)
				{
					yield return session;
					continue;
				}

				var secondsRemaining = session.ElapsedSeconds;
				while (startTime.Date <= endTime.Date)
				{
					var secondsTillMidnight = startTime.Date.AddDays(1) - startTime;
					var secondsInSession = Math.Min(secondsRemaining, (int)secondsTillMidnight.TotalSeconds);
					yield return new Session
					{
						DateSession = startTime,
						ElapsedSeconds = secondsInSession,
						IdConfiguration = session.IdConfiguration,
						PlatformIDs = session.PlatformIDs,
						PlatfromId = session.PlatfromId,
						SourceId = session.SourceId
					};

					secondsRemaining -= secondsInSession;
					startTime = startTime.Date.AddDays(1);
				}
			}
		}

		public static IEnumerable<Session> SplitIntoHourly(this IEnumerable<Session> sessions)
		{
			foreach (var session in sessions)
			{
				var startTime = session.DateSession;
				var endTime = startTime.AddSeconds(session.ElapsedSeconds);
				if (startTime.Hour == endTime.Hour 
				    && startTime.Day == endTime.Day 
				    && startTime.Month == endTime.Month 
				    && startTime.Year == endTime.Year)
				{
					yield return session;
					continue;
				}

				var secondsRemaining = session.ElapsedSeconds;
				while (startTime < endTime)
				{
					var secondsRemainingInHour = 3600 - (startTime.Minute * 60 + startTime.Second);
					var secondsInSession = Math.Min(secondsRemaining, secondsRemainingInHour);
					yield return new Session
					{
						DateSession = startTime,
						ElapsedSeconds = secondsInSession,
						IdConfiguration = session.IdConfiguration,
						PlatformIDs = session.PlatformIDs,
						PlatfromId = session.PlatfromId,
						SourceId = session.SourceId
					};
					secondsRemaining -= secondsInSession;
					startTime = startTime.AddSeconds(secondsInSession);
				}
			}
		}
	}
}