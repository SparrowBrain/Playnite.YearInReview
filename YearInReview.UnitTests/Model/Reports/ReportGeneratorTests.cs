using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FakeItEasy;
using Playnite.SDK;
using Playnite.SDK.Models;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Filters;
using YearInReview.Model.Reports;
using YearInReview.Model.Reports._1970;

namespace YearInReview.UnitTests.Model.Reports
{
	public class ReportGeneratorTests
	{
		[Theory]
		[AutoFakeItEasyData]
		public async Task Generate_YearGiven_GeneratesSpecificYearReport(
			[Frozen] IPlayniteAPI playniteApi,
			[Frozen] IGameDatabaseAPI gameDatabaseApi,
			[Frozen] IGameActivityExtension gameActivityExtension,
			[Frozen] ISpecificYearActivityFilter specificYearActivityFilter,
			[Frozen] IComposer1970 composer,
			TestableItemCollection<Game> games,
			List<Activity> activities,
			List<Activity> filteredActivities,
			Report1970 expected,
			int year,
			ReportGenerator sut)
		{
			// Arrange
			A.CallTo(() => playniteApi.Database).Returns(gameDatabaseApi);
			A.CallTo(() => gameDatabaseApi.Games).Returns(games);
			A.CallTo(() => gameActivityExtension.GetActivityForGames(games)).Returns(activities);
			A.CallTo(() => specificYearActivityFilter.GetActivityForYear(year, activities)).Returns(filteredActivities);
			A.CallTo(() => composer.Compose(year, filteredActivities)).Returns(expected);

			// Act
			var actual = await sut.Generate(year);

			// Assert
			Assert.Equal(expected, actual);
		}
	}
}