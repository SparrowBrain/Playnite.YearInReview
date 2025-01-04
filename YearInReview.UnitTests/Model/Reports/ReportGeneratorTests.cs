using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

		[Theory]
		[AutoFakeItEasyData]
		public async Task GenerateAllYears_GenerateAllYearReportExceptCurrent(
			[Frozen] IPlayniteAPI playniteApi,
			[Frozen] IGameDatabaseAPI gameDatabaseApi,
			[Frozen] IGameActivityExtension gameActivityExtension,
			[Frozen] ISpecificYearActivityFilter specificYearActivityFilter,
			[Frozen] IComposer1970 composer,
			TestableItemCollection<Game> games,
			List<Activity> activities,
			List<Activity> filteredActivities1,
			List<Activity> filteredActivities2,
			Report1970 expected1,
			Report1970 expected2,
			int year1,
			int year2,
			ReportGenerator sut)
		{
			// Arrange
			foreach (var activity in activities)
			{
				for (var i = 0; i < activity.Items.Count; i++)
				{
					var year = i % 2 == 0 ? year1 : year2;
					var session = activity.Items[i];
					session.DateSession = new DateTime(year, session.DateSession.Month, session.DateSession.Day);
				}
			}

			A.CallTo(() => playniteApi.Database).Returns(gameDatabaseApi);
			A.CallTo(() => gameDatabaseApi.Games).Returns(games);
			A.CallTo(() => gameActivityExtension.GetActivityForGames(games)).Returns(activities);
			A.CallTo(() => specificYearActivityFilter.GetActivityForYear(year1, activities)).Returns(filteredActivities1);
			A.CallTo(() => specificYearActivityFilter.GetActivityForYear(year2, activities)).Returns(filteredActivities2);
			A.CallTo(() => composer.Compose(year1, filteredActivities1)).Returns(expected1);
			A.CallTo(() => composer.Compose(year2, filteredActivities2)).Returns(expected2);

			// Act
			var actual = await sut.GenerateAllYears();

			// Assert
			Assert.Equal(2, actual.Count);
			Assert.Contains(expected1, actual);
			Assert.Contains(expected2, actual);
		}
	}
}