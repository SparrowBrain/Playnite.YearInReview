using Playnite.SDK;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using YearInReview.Infrastructure.Services;
using YearInReview.Infrastructure.UserControls;
using YearInReview.Model.Reports.Persistence;

namespace YearInReview.Model.Reports._1970.MVVM
{
	public class Report1970ViewModel : ObservableObject
	{
		private const int MaxBarWidth = 500;
		private readonly IPlayniteAPI _api;

		public Report1970ViewModel(IPlayniteAPI api, Report1970 report, List<PersistedReport> allYearReports)
		{
			_api = api;

			Year = report.Metadata.Year;
			Username = report.Metadata.Username;

			IntroMessage = string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_Intro"), Year, Username);
			IntroMessageSubtext = string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_IntroSubtext"),
				ReadableTimeFormatter.FormatTime(report.TotalPlaytime),
				Year);

			MostPlayedGame = report.MostPlayedGames.First();

			MostPlayedGames = report.MostPlayedGames
				.Select((t, i) => new GameViewModel(api, i + 1, t, MaxBarWidth, MostPlayedGame.TimePlayed)).ToList()
				.ToObservable();

			var maxSourcePlaytime = report.MostPlayedSources.OrderByDescending(x => x.TimePlayed).FirstOrDefault()?.TimePlayed ?? 0;
			MostPlayedSources = report.MostPlayedSources
				.Select((x, i) => new SourceViewModel(x, i + 1, MaxBarWidth, maxSourcePlaytime)).ToObservable();

			var maxPlaytimeInDay = report.PlaytimeCalendarDays.Max(x => x.TotalPlaytime);
			PlaytimeCalendarDays = report.PlaytimeCalendarDays
				.Select(x => new CalendarDayViewModel(x, maxPlaytimeInDay)).ToObservable();

			var maxHourlyPlaytime = report.HourlyPlaytime.Max(x => x.Playtime);
			HourlyPlaytime = report.HourlyPlaytime
				.Select(x => new HourlyPlaytimeViewModel(x, maxHourlyPlaytime)).ToObservable();

			if (allYearReports.Count > 1)
			{
				var allReports = allYearReports
					.OrderByDescending(x => x.TotalPlaytime)
					.ToList();

				var maxFriendPlaytime = allReports.Max(x => x.TotalPlaytime);

				FriendsPlaytimeLeaderboard = allReports
					.Select((x, i) => new FriendPlaytimeLeaderboardViewModel(i + 1, x.Username, x.TotalPlaytime, maxFriendPlaytime))
					.ToObservable();
			}
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

		public ObservableCollection<CalendarDayViewModel> PlaytimeCalendarDays { get; set; }

		public ObservableCollection<HourlyPlaytimeViewModel> HourlyPlaytime { get; set; }

		public bool ShowFriendLeaderboard => FriendsPlaytimeLeaderboard.Any();

		public ObservableCollection<FriendPlaytimeLeaderboardViewModel> FriendsPlaytimeLeaderboard { get; set; } =
			new ObservableCollection<FriendPlaytimeLeaderboardViewModel>();

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

	public class FriendPlaytimeLeaderboardViewModel
	{
		private const int MaxBarWidth = 500;

		public FriendPlaytimeLeaderboardViewModel(int position, string name, int playtime, int maxPlaytime)
		{
			Position = position;
			Name = name;
			Playtime = playtime;

			BarWidth = (float)playtime * MaxBarWidth / maxPlaytime;
		}

		public int Position { get; }

		public string Name { get; }

		public int Playtime { get; }

		public float BarWidth { get; set; }
	}
}