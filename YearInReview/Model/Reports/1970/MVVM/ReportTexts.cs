using Playnite.SDK;
using YearInReview.Infrastructure.Services;

namespace YearInReview.Model.Reports._1970.MVVM
{
	public class ReportTexts
	{
		private readonly bool _isOwn;
		private readonly string _username;
		private readonly int _year;
		private readonly int _addedGamesCount;
		private readonly int _totalPlaytime;
		private readonly string _mostPlayedGameName;
		private readonly int _mostPlayedGamePlaytime;
		private readonly string _mostPlayedLibraryName;
		private readonly int _topGameCount;

		public ReportTexts(
			bool isOwn,
			string username,
			int year,
			int addedGamesCount,
			int totalPlaytime,
			string mostPlayedGameName,
			int mostPlayedGamePlaytime,
			string mostPlayedLibraryName,
			int topGameCount)
		{
			_isOwn = isOwn;
			_username = username;
			_year = year;
			_addedGamesCount = addedGamesCount;
			_totalPlaytime = totalPlaytime;
			_mostPlayedGameName = mostPlayedGameName;
			_mostPlayedGamePlaytime = mostPlayedGamePlaytime;
			_mostPlayedLibraryName = mostPlayedLibraryName;
			_topGameCount = topGameCount;
		}

		public string LOC_YearInReview_Report1970_AddedGamesCountHeader => _isOwn
			? ResourceProvider.GetString("LOC_YearInReview_Report1970_AddedGamesCountHeader")
			: string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_AddedGamesCountHeader_Friend"), _username);

		public string LOC_YearInReview_Report1970_AddedGamesCountMessage => _isOwn
			? string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_AddedGamesCountMessage"), _addedGamesCount)
			: string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_AddedGamesCountMessage_Friend"), _username, _addedGamesCount);

		public string LOC_YearInReview_Report1970_HourlyPlaytimeHeader => _isOwn
			? string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_HourlyPlaytimeHeader"))
			: string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_HourlyPlaytimeHeader_Friend"), _username);

		public string LOC_YearInReview_Report1970_Intro =>
			string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_Intro"), _year, _username);

		public string LOC_YearInReview_Report1970_IntroSubtext => _isOwn
			? string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_IntroSubtext"), ReadableTimeFormatter.FormatTime(_totalPlaytime), _year)
			: string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_IntroSubtext_Friend"), _username, ReadableTimeFormatter.FormatTime(_totalPlaytime), _year);

		public string LOC_YearInReview_Report1970_FriendsLeaderboardHeader => _isOwn
			? string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_FriendsLeaderboardHeader"))
			: string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_FriendsLeaderboardHeader_Friend"), _username);

		public string LOC_YearInReview_Report1970_MostPlayedGameMessage => _isOwn
			? string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_MostPlayedGameMessage"), _mostPlayedGameName, ReadableTimeFormatter.FormatTime(_mostPlayedGamePlaytime))
			: string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_MostPlayedGameMessage_Friend"), _username, _mostPlayedGameName, ReadableTimeFormatter.FormatTime(_mostPlayedGamePlaytime));

		public string LOC_YearInReview_Report1970_PlaytimeCalendarHeader => _isOwn
			? string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_PlaytimeCalendarHeader"))
			: string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_PlaytimeCalendarHeader_Friend"), _username);

		public string LOC_YearInReview_Report1970_SingleSourceText => _isOwn
			? string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_SingleSourceText"), _mostPlayedLibraryName)
			: string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_SingleSourceText_Friend"), _mostPlayedLibraryName, _username);

		public string LOC_YearInReview_Report1970_TopGamesHeader => _isOwn
			? string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_TopGamesHeader"), _topGameCount)
			: string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_TopGamesHeader_Friend"), _username, _topGameCount);

		public string LOC_YearInReview_Report1970_TopSourcesHeader => _isOwn
			? string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_TopSourcesHeader"))
			: string.Format(ResourceProvider.GetString("LOC_YearInReview_Report1970_TopSourcesHeader_Friend"), _username);
	}
}