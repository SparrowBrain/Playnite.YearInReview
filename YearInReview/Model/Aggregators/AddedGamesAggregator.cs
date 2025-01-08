using System.Collections.Generic;
using System.Linq;
using Playnite.SDK;
using Playnite.SDK.Models;
using YearInReview.Model.Aggregators.Data;

namespace YearInReview.Model.Aggregators
{
	public class AddedGamesAggregator : IAddedGamesAggregator
	{
		
		private readonly IPlayniteAPI _playniteApi;

		public AddedGamesAggregator(IPlayniteAPI playniteApi)
		{
			_playniteApi = playniteApi;
		}
		
		public IReadOnlyCollection<AddedGame> GetAddedGames(IReadOnlyCollection<Game> allGames, int filterYear)
		{
			var filteredGames = new List<AddedGame>();
			if (allGames == null || allGames.Count <= 0) return filteredGames;
			
			filteredGames = allGames
				.Where(g => g?.Added != null && g.Added.Value.Year == filterYear)
				.Select(g => new AddedGame { AddedDate = g.Added.Value, Game = g })
				.ToList();

			return filteredGames;
		}
		
		public IReadOnlyCollection<AddedGame> GetAddedGames(int filterYear)
		{
			var allGames = _playniteApi.Database.Games;
			return GetAddedGames(allGames.ToList(), filterYear);
		}
	}
}