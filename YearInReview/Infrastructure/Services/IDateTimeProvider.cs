using System;

namespace YearInReview.Infrastructure.Services
{
	public interface IDateTimeProvider
	{
		DateTime GetNow();
	}
}