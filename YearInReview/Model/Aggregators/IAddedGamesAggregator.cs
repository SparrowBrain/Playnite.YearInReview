using System.Collections.Generic;
using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Aggregators
{
	public interface IAddedGamesAggregator
	{
		/// <summary>
		/// Aggregates the added games for the given year.
		/// </summary>
		/// <param name="filterYear">year to filter by</param>
		/// <returns>list of all games, which were added in the given year</returns>
		IReadOnlyCollection<AddedGame> GetAddedGames(int filterYear);
	}
}