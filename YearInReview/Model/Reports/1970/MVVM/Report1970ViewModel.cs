using Playnite.SDK;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Reports.MVVM;

namespace YearInReview.Model.Reports._1970.MVVM
{
	public class Report1970ViewModel : ObservableObject
	{
		private readonly IPlayniteAPI _api;

		public Report1970ViewModel(IPlayniteAPI api, Report1970 report)
		{
			_api = api;
			Year = report.Metadata.Year;
			Username = report.Metadata.Username;
			MostPlayedGame = report.MostPlayedGames.First();

			MostPlayedGames = report.MostPlayedGames
				.Select((t, i) => new GameViewModel(api, i + 1, t, 500, MostPlayedGame.TimePlayed)).ToList()
				.ToObservable();
		}

		public int Year { get; set; }

		public string Username { get; set; }

		public string IntroMessage => string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_Intro"), Username, Year);

		public ReportGameWithTime MostPlayedGame { get; set; }

		public ObservableCollection<GameViewModel> MostPlayedGames { get; set; }

		public string MostPlayedGameMessage => string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_MostPlayedGameMessage"), MostPlayedGame.Name, ReadableTimeFormatter.FormatTime(MostPlayedGame.TimePlayed));

		public ICommand OpenMostPlayedDetails =>
			new RelayCommand(() =>
			{
				_api.MainView.SelectGame(MostPlayedGame.Id);
				_api.MainView.SwitchToLibraryView();
			});

		public string Top10Header => string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_Top10Header"), MostPlayedGames.Count);
	}
}