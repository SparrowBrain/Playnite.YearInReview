using Playnite.SDK;
using System;
using System.Linq;

namespace YearInReview.Infrastructure.Services
{
	public class Navigator : INavigator
	{
		private readonly IPlayniteAPI _api;

		public Navigator(IPlayniteAPI api)
		{
			_api = api;
		}

		public void ShowGame(Guid id, string name)
		{
			var game = _api.Database.Games.Get(id);
			if (game != null)
			{
				ShowGame(id);
				return;
			}

			game = _api.Database.Games
				.Where(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				.OrderBy(x => x.Hidden)
				.FirstOrDefault();
			if (game != null)
			{
				ShowGame(game.Id);
			}
		}

		private void ShowGame(Guid id)
		{
			_api.MainView.SelectGame(id);
			_api.MainView.SwitchToLibraryView();
		}
	}
}