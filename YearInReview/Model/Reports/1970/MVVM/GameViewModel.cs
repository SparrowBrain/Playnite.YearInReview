using System;
using System.Collections.Generic;
using System.Windows.Input;
using Playnite.SDK;

namespace YearInReview.Model.Reports._1970.MVVM
{
	public class GameViewModel : ObservableObject
	{
		private readonly IPlayniteAPI _api;

		public GameViewModel(IPlayniteAPI api, int position, ReportGameWithTime game, int maxPlayTime)
		{
			_api = api;

			Id = game.Id;
			Position = position;
			Name = game.Name;
			CoverImage = game.CoverImage;
			TimePlayed = game.TimePlayed;
			Percentage = (double)game.TimePlayed / maxPlayTime;
		}

		public Guid Id { get; set; }

		public int Position { get; }

		public string Name { get; set; }

		public string CoverImage { get; set; }

		public int TimePlayed { get; set; }

		public double Percentage { get; set; }

		public ICommand OpenDetails =>
			new RelayCommand(() =>
			{
				_api.MainView.SelectGame(Id);
				_api.MainView.SwitchToLibraryView();
			});
	}
}