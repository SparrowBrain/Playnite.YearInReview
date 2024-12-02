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

			IntroMessage = string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_Intro"), Username);
			IntroMessageSubtext = string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_IntroSubtext"),
				ReadableTimeFormatter.FormatTime(report.TotalPlaytime),
				Year);

			MostPlayedGame = report.MostPlayedGames.First();

			MostPlayedGames = report.MostPlayedGames
				.Select((t, i) => new GameViewModel(api, i + 1, t, 500, MostPlayedGame.TimePlayed)).ToList()
				.ToObservable();

			var maxSourcePlaytime = report.MostPlayedSources.OrderByDescending(x => x.TimePlayed).FirstOrDefault()?.TimePlayed ?? 0;
			MostPlayedSources = report.MostPlayedSources
				.Select((x, i) => new SourceViewModel(x, i + 1, 500, maxSourcePlaytime)).ToObservable();
		}

		public int Year { get; set; }

		public string Username { get; set; }

		public string IntroMessage { get; set; }

		public string IntroMessageSubtext { get; set; }

		public ReportGameWithTime MostPlayedGame { get; set; }

		public ObservableCollection<GameViewModel> MostPlayedGames { get; set; }

		public bool ShowTopSources => MostPlayedSources.Count > 1;

		public bool ShowSingleSource => MostPlayedSources.Count == 1;

		public ObservableCollection<SourceViewModel> MostPlayedSources { get; set; }

		public string MostPlayedGameMessage => string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_MostPlayedGameMessage"), MostPlayedGame.Name, ReadableTimeFormatter.FormatTime(MostPlayedGame.TimePlayed));

		public ICommand OpenMostPlayedDetails =>
			new RelayCommand(() =>
			{
				_api.MainView.SelectGame(MostPlayedGame.Id);
				_api.MainView.SwitchToLibraryView();
			});

		public string TopGamesHeader => string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_TopGamesHeader"), MostPlayedGames.Count);

		public string TopSourcesHeader => string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_TopSourcesHeader"));

		public string SingleSourceText => string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_SingleSourceText"), MostPlayedSources.FirstOrDefault()?.Name);
	}
}