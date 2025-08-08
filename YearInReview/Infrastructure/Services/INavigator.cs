using System;

namespace YearInReview.Infrastructure.Services
{
	public interface INavigator
	{
		void ShowGame(Guid id, string name);
	}
}