using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Playnite.SDK;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Filters;
using YearInReview.Model.Reports._1970;

namespace YearInReview.Model.Reports
{
	public class ReportManager
	{
		private readonly IPlayniteAPI _playniteApi;
		private readonly IComposer1970 _composer1970;
		private readonly IGameActivityExtension _gameActivityExtension;
		private readonly ISpecificYearActivityFilter _specificYearActivityFilter;

		public ReportManager(IPlayniteAPI playniteApi, IComposer1970 composer1970, IGameActivityExtension gameActivityExtension, ISpecificYearActivityFilter specificYearActivityFilter)
		{
			_playniteApi = playniteApi;
			_composer1970 = composer1970;
			_gameActivityExtension = gameActivityExtension;
			_specificYearActivityFilter = specificYearActivityFilter;
		}

		public async Task<Report1970> GetReport(int year)
		{
			var activities = await _gameActivityExtension.GetActivityForGames(_playniteApi.Database.Games.ToList());
			var filteredActivities = _specificYearActivityFilter.GetActivityForYear(year, activities);

			return _composer1970.Compose(year, filteredActivities);
		}
	}
}