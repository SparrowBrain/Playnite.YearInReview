using Playnite.SDK;
using System;
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
		private readonly INavigator _navigator;

		public Report1970ViewModel(INavigator navigator, Report1970 report, bool isOwn, List<PersistedReport> allYearReports)
		{
			_navigator = navigator;

			Id = report.Metadata.Id;
			Year = report.Metadata.Year;
			Username = report.Metadata.Username;

			MostPlayedGame = report.MostPlayedGames.First();

			AddedGamesCount = report.AddedGamesCount;

			NotableAddedGames = report.NotableAddedGames.Select(x => new AddedGameViewModel(navigator, x)).ToList().ToObservable();

			MostPlayedGames = report.MostPlayedGames
				.Select((t, i) => new GameViewModel(navigator, i + 1, t, MostPlayedGame.TimePlayed)).ToList()
				.ToObservable();

			var maxSourcePlaytime = report.MostPlayedSources.OrderByDescending(x => x.TimePlayed).FirstOrDefault()?.TimePlayed ?? 0;
			MostPlayedSources = report.MostPlayedSources
				.Select((x, i) => new SourceViewModel(x, i + 1, maxSourcePlaytime)).ToObservable();

			var maxPlaytimeInDay = report.PlaytimeCalendarDays.Max(x => x.TotalPlaytime);
			PlaytimeCalendarDays = report.PlaytimeCalendarDays
				.Select(x => new CalendarDayViewModel(x, maxPlaytimeInDay)).ToObservable();

			HourlyPlaytime = report.HourlyPlaytime.OrderBy(x => x.Hour)
				.Select(x => x.Playtime).ToObservable();

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

			Texts = new ReportTexts(
				isOwn,
				Username,
				Year, AddedGamesCount,
				report.TotalPlaytime,
				MostPlayedGame.Name,
				MostPlayedGame.TimePlayed,
				MostPlayedSources.FirstOrDefault()?.Name,
				MostPlayedGames.Count);
		}

		public Guid Id { get; set; }

		public int Year { get; set; }

		public string Username { get; set; }

		public int AddedGamesCount { get; set; }

		public ReportGameWithTime MostPlayedGame { get; set; }

		public ObservableCollection<GameViewModel> MostPlayedGames { get; set; }

		public bool ShowTopSources => MostPlayedSources.Count > 1;

		public bool ShowSingleSource => MostPlayedSources.Count == 1;

		public ObservableCollection<SourceViewModel> MostPlayedSources { get; set; }

		public ObservableCollection<CalendarDayViewModel> PlaytimeCalendarDays { get; set; }

		public ObservableCollection<int> HourlyPlaytime { get; set; }

		public bool ShowFriendLeaderboard => FriendsPlaytimeLeaderboard.Any();

		public ObservableCollection<FriendPlaytimeLeaderboardViewModel> FriendsPlaytimeLeaderboard { get; set; } =
			new ObservableCollection<FriendPlaytimeLeaderboardViewModel>();

		public bool ShowNotableAddedGames => NotableAddedGames.Any();

		public ObservableCollection<AddedGameViewModel> NotableAddedGames { get; set; }

		public ICommand OpenMostPlayedDetails =>
			new RelayCommand(() =>
			{
				_navigator.ShowGame(MostPlayedGame.Id, MostPlayedGame.Name);
			});

		public ReportTexts Texts { get; set; }

		public bool PromptToExportReport => FriendsPlaytimeLeaderboard.Count < 2;

		public RelayCommand ShowSharingHelp => new RelayCommand(() =>
			System.Diagnostics.Process.Start("https://github.com/SparrowBrain/Playnite.YearInReview?tab=readme-ov-file#sharing-with-friends"));
	}
}