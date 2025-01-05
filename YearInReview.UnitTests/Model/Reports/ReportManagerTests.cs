using AutoFixture.Xunit2;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTools.Shared;
using Xunit;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Reports;
using YearInReview.Model.Reports._1970;
using YearInReview.Model.Reports.Persistence;

namespace YearInReview.UnitTests.Model.Reports
{
	public class ReportManagerTests
	{
		[Theory]
		[AutoFakeItEasyData]
		public async Task Init_LoadsReports_WhenReportsExist(
			[Frozen] IReportPersistence reportPersistence,
			List<PersistedReport> persistedReports,
			ReportManager sut)
		{
			// Arrange
			var ownReport = persistedReports.Last();
			ownReport.IsOwn = true;
			A.CallTo(() => reportPersistence.PreLoadAllReports()).Returns(persistedReports);

			// Act
			await sut.Init();

			// Assert
			var reports = sut.GetAllPreLoadedReports();
			Assert.Equivalent(persistedReports, reports);
		}

		[Theory]
		[AutoFakeItEasyData]
		public async Task Init_GeneratesReportsAndSavesThem_WhenNoOwnReportExists(
			[Frozen] IReportPersistence reportPersistence,
			[Frozen] IReportGenerator reportGenerator,
			List<Report1970> generatedReports,
			List<PersistedReport> persistedReports,
			ReportManager sut)
		{
			// Arrange

			A.CallTo(() => reportPersistence.PreLoadAllReports()).ReturnsNextFromSequence(
				new List<PersistedReport>(),
				persistedReports);
			A.CallTo(() => reportGenerator.GenerateAllYears()).Returns(generatedReports);

			// Act
			await sut.Init();

			// Assert
			generatedReports.ForEach(report => A.CallTo(() => reportPersistence.SaveReport(report)).MustHaveHappened());
			var reports = sut.GetAllPreLoadedReports();
			Assert.Equivalent(persistedReports, reports);
		}

		[Theory]
		[AutoFakeItEasyData]
		public async Task Init_GeneratesNewReports_WhenLastGeneratedReportIsOlderThanPreviousYear(
			[Frozen] IReportPersistence reportPersistence,
			[Frozen] IReportGenerator reportGenerator,
			[Frozen] IDateTimeProvider dateTimeProvider,
			int currentYear,
			DateTime now,
			List<PersistedReport> persistedReports1,
			List<PersistedReport> persistedReports2,
			Report1970 generatedReport1,
			Report1970 generatedReport2,
			ReportManager sut)
		{
			// Arrange
			persistedReports1.ForEach(x => x.IsOwn = false);

			var ownReport = persistedReports1.Last();
			ownReport.IsOwn = true;
			ownReport.Year = currentYear - 3;
			A.CallTo(() => reportPersistence.PreLoadAllReports()).ReturnsNextFromSequence(persistedReports1, persistedReports2);

			now = new DateTime(currentYear, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			A.CallTo(() => dateTimeProvider.GetNow()).Returns(now);

			A.CallTo(() =>
				reportGenerator.Generate(An<int>.That.Matches(x =>
					x == currentYear - 1 || x == currentYear - 2))).ReturnsNextFromSequence(
				generatedReport1,
				generatedReport2);

			// Act
			await sut.Init();

			// Assert
			A.CallTo(() => reportPersistence.SaveReport(generatedReport1)).MustHaveHappened();
			A.CallTo(() => reportPersistence.SaveReport(generatedReport2)).MustHaveHappened();
			var reports = sut.GetAllPreLoadedReports();
			Assert.Equivalent(persistedReports2, reports);
		}

		[Theory]
		[AutoFakeItEasyData]
		public async Task GetReport_LoadReportFromPeristence_WhenReportsExist(
			[Frozen] IReportPersistence reportPersistence,
			List<PersistedReport> persistedReports,
			Report1970 expected,
			ReportManager sut)
		{
			// Arrange
			var ownReport = persistedReports.Last();
			ownReport.IsOwn = true;
			A.CallTo(() => reportPersistence.PreLoadAllReports()).Returns(persistedReports);
			A.CallTo(() => reportPersistence.LoadReport(ownReport.FilePath)).Returns(expected);
			await sut.Init();
			
			// Act
			var actual = sut.GetReport(ownReport.Id);

			// Assert
			Assert.Equal(expected, actual);
		}
	}
}