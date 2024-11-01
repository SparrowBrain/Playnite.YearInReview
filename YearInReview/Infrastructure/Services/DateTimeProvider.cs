using System;

namespace YearInReview.Infrastructure.Services
{
	internal class DateTimeProvider : IDateTimeProvider
	{
		public DateTime GetNow()
		{
			return DateTime.Now;
		}
	}
}