using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using YearInReview.Infrastructure.Services;

namespace YearInReview.Model.Reports._1970.MVVM
{
	public class AddedGameViewModel : ObservableObject
	{
		private readonly INavigator _navigator;
		private readonly string _name;

		public AddedGameViewModel(INavigator navigator, ReportAddedGame game)
		{
			_navigator = navigator;

			Id = game.Id;
			_name = game.Name;
			NameWithLibrary = $"{game.Name} ({game.SourceName})";
			CoverImage = game.CoverImage;
			CriticScoreText = string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_AddedGameCriticScoreText"),
				game.CriticScore); ;
			AddedDateText = string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_AddedGameAddedDateText"),
				game.AddedDate.ToShortDateString());
		}

		public Guid Id { get; set; }

		public string NameWithLibrary { get; set; }

		public string CoverImage { get; set; }

		public string CriticScoreText { get; set; }

		public string AddedDateText { get; set; }

		public ICommand OpenDetails =>
			new RelayCommand(() =>
			{
				_navigator.ShowGame(Id, _name);
			});
	}
}