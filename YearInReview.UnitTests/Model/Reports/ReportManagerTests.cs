﻿using AutoFixture.Xunit2;
using FakeItEasy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTools.Shared;
using Xunit;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Aggregators.Data;
using YearInReview.Model.Exceptions;
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
		public async Task GetReport_LoadReportFromPersistence_WhenReportsExist(
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

		[Theory]
		[AutoFakeItEasyData]
		public void ExportReport_ThrowsException_WhenReportNotFound(
			Guid reportId,
			string reportPath,
			JsonSerializerSettings settings,
			ReportManager sut)
		{
			// Act
			var exception = Record.Exception(() => sut.ExportReport(reportId, reportPath, settings));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<InvalidOperationException>(exception);
			Assert.Equal($"Report {reportId} not persisted in cache.", exception.Message);
		}

		[Theory]
		[AutoFakeItEasyData]
		public async Task ExportReport_PersistsReport_WhenReportExists(
			[Frozen] IReportPersistence reportPersistence,
			List<PersistedReport> persistedReports,
			Report1970 report,
			string reportPath,
			JsonSerializerSettings settings,
			ReportManager sut)
		{
			// Arrange
			var persistedReport = persistedReports.Last();
			A.CallTo(() => reportPersistence.PreLoadAllReports()).Returns(persistedReports);
			A.CallTo(() => reportPersistence.LoadReport(persistedReport.FilePath)).Returns(report);
			await sut.Init();

			// Act
			sut.ExportReport(persistedReport.Id, reportPath, settings);

			// Assert
			A.CallTo(() => reportPersistence.ExportReport(report, reportPath, settings)).MustHaveHappened();
		}

		[Theory]
		[AutoFakeItEasyData]
		public void ImportReport_ReturnsImportedReportId(
			[Frozen] IReportPersistence reportPersistence,
			Report1970 report,
			PersistedReport persistedReport,
			ReportManager sut)
		{
			// Arrange
			A.CallTo(() => reportPersistence.ImportReport(report)).Returns(persistedReport);

			// Act
			var actual = sut.ImportReport(report);

			// Assert
			Assert.Equal(persistedReport.Id, actual);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void ImportReport_PersistsReportInCache(
			[Frozen] IReportPersistence reportPersistence,
			PersistedReport persistedReport,
			Report1970 expected,
			ReportManager sut)
		{
			// Arrange
			A.CallTo(() => reportPersistence.ImportReport(expected)).Returns(persistedReport);
			A.CallTo(() => reportPersistence.PreLoadAllReports()).Returns(new List<PersistedReport> { persistedReport });
			A.CallTo(() => reportPersistence.LoadReport(persistedReport.FilePath)).Returns(expected);

			// Act
			var actualId = sut.ImportReport(expected);

			// Assert
			var actual = sut.GetReport(actualId);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineAutoFakeItEasyData(nameof(Report1970.Metadata))]
		[InlineAutoFakeItEasyData(nameof(Metadata.Id))]
		[InlineAutoFakeItEasyData(nameof(Metadata.Year))]
		[InlineAutoFakeItEasyData(nameof(Metadata.Username))]
		public void ImportReport_ThrowsException_WhenReportIsInvalid(
			string propertyName,
			Report1970 report,
			ReportManager sut)
		{
			// Arrange
			if (propertyName == nameof(Report1970.Metadata))
			{
				report.Metadata = null;
			}
			else
			{
				var metadataProperty = typeof(Report1970).GetProperty(nameof(Report1970.Metadata));
				var property = metadataProperty.PropertyType.GetProperty(propertyName);
				property.SetValue(report.Metadata, default);
			}

			// Act
			var exception = Record.Exception(() => sut.ImportReport(report));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<InvalidReportFileException>(exception);
			Assert.Equal("Trying to import invalid report file.", exception.Message);
		}

		[Theory]
		[AutoFakeItEasyData]
		public async Task ImportReport_ThrowsException_WhenReportIsAlreadyInCache(
			[Frozen] IReportPersistence reportPersistence,
			List<PersistedReport> persistedReports,
			Report1970 report,
			ReportManager sut)
		{
			// Arrange
			report.Metadata.Id = persistedReports.Last().Id;
			A.CallTo(() => reportPersistence.PreLoadAllReports()).Returns(persistedReports);
			await sut.Init();

			// Act
			var exception = Record.Exception(() => sut.ImportReport(report));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<ReportAlreadyExistsException>(exception);
			Assert.Equal($"Trying to import report {report.Metadata.Id} that is already in cache.", exception.Message);
		}
	}
}