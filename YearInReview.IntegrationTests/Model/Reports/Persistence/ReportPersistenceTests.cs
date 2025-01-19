using AutoFixture.Xunit2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using YearInReview.Model.Aggregators.Data;
using YearInReview.Model.Reports._1970;
using YearInReview.Model.Reports.Persistence;

namespace YearInReview.IntegrationTests.Model.Reports.Persistence
{
	public class ReportPersistenceTests
	{
		private const string ExtensionDataPath = @"Model\Reports\Persistence";

		private readonly ReportPersistence _sut;
		private readonly string _reportsPath = Path.Combine(ExtensionDataPath, "Reports");

		public ReportPersistenceTests()
		{
			_sut = new ReportPersistence(ExtensionDataPath);
		}

		[Fact]
		public void SaveReport_ThrowsArgumentNullException_WhenReportIsNull()
		{
			// Act
			var exception = Record.Exception(() => _sut.SaveReport(null));

			// Assert
			Assert.IsType<ArgumentNullException>(exception);
		}

		[Theory]
		[AutoData]
		public void SaveReport_PersistsReport_WhenReportIsNotNull(Report1970 report)
		{
			// Act
			_sut.SaveReport(report);

			// Assert
			var userReportPath = Path.Combine(_reportsPath, report.Metadata.Year.ToString(), "user.json");
			Assert.True(File.Exists(userReportPath));

			var fileContent = File.ReadAllText(userReportPath);
			var persistedReport = JsonConvert.DeserializeObject<Report1970>(fileContent);
			Assert.Equivalent(report, persistedReport);
		}

		[Fact]
		public void PreLoadAllReports_ReturnsEmptyCollection_WhenReportDirectoryDoesNotExist()
		{
			// Arrange
			if (Directory.Exists(_reportsPath))
			{
				Directory.Delete(_reportsPath, true);
			}

			// Act
			var result = _sut.PreLoadAllReports();

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void PreLoadAllReports_ReturnsEmptyCollection_WhenReportDirectoryExistsButNoUserJson()
		{
			// Arrange
			if (Directory.Exists(_reportsPath))
			{
				Directory.Delete(_reportsPath, true);
			}

			Directory.CreateDirectory(_reportsPath);
			Directory.CreateDirectory(Path.Combine(_reportsPath, "2023"));

			// Act
			var result = _sut.PreLoadAllReports();

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoData]
		public void PreLoadAllReports_ReturnsUserReports_WhenReportDirectoryExistsAndUserJson(Report1970 report)
		{
			// Arrange
			if (Directory.Exists(_reportsPath))
			{
				Directory.Delete(_reportsPath, true);
			}

			Directory.CreateDirectory(_reportsPath);
			Directory.CreateDirectory(Path.Combine(_reportsPath, "2023"));
			var reportFilePath = Path.Combine(_reportsPath, "2023", "user.json");
			File.WriteAllText(reportFilePath, JsonConvert.SerializeObject(report));

			// Act
			var result = _sut.PreLoadAllReports();

			// Assert
			var persistedReport = Assert.Single(result);
			Assert.Equal(report.Metadata.Id, persistedReport.Id);
			Assert.True(persistedReport.IsOwn);
			Assert.Equal(reportFilePath, persistedReport.FilePath);
			Assert.Equal(report.Metadata.Year, persistedReport.Year);
			Assert.Equal(report.Metadata.Username, persistedReport.Username);
			Assert.Equal(report.TotalPlaytime, persistedReport.TotalPlaytime);
		}

		[Theory]
		[AutoData]
		public void PreLoadAllReports_ReturnsFriendReports_WhenFriendReportDirectoryExistsAndJsons(List<Report1970> reports)
		{
			// Arrange
			if (Directory.Exists(_reportsPath))
			{
				Directory.Delete(_reportsPath, true);
			}

			Directory.CreateDirectory(_reportsPath);
			var friendsPath = Path.Combine(_reportsPath, "2023", "Friends");
			Directory.CreateDirectory(friendsPath);
			foreach (var report in reports)
			{
				var reportFilePath = Path.Combine(friendsPath, $"{report.Metadata.Username}_{report.Metadata.Year}.json");
				File.WriteAllText(reportFilePath, JsonConvert.SerializeObject(report));
			}

			// Act
			var result = _sut.PreLoadAllReports();

			// Assert
			Assert.Equal(reports.Count, result.Count);
			Assert.All(result, x => Assert.False(x.IsOwn));
			Assert.Contains(result,
				x => reports.Any(r =>
					Path.Combine(friendsPath, $"{r.Metadata.Username}_{r.Metadata.Year}.json") == x.FilePath
					&& r.Metadata.Id == x.Id
					&& r.Metadata.Year == x.Year
					&& r.Metadata.Username == x.Username
					&& r.TotalPlaytime == x.TotalPlaytime));
		}

		[Theory]
		[AutoData]
		public void LoadReport_ReturnsReport_WhenExistingPathIsGiven(Report1970 expected)
		{
			// Arrange
			Directory.CreateDirectory(_reportsPath);
			var friendsPath = Path.Combine(_reportsPath, "2023", "Friends");
			Directory.CreateDirectory(friendsPath);
			var reportPath = Path.Combine(friendsPath, $"{expected.Metadata.Username}_{expected.Metadata.Year}.json");
			File.WriteAllText(reportPath, JsonConvert.SerializeObject(expected));

			// Act
			var actual = _sut.LoadReport(reportPath);

			// Assert
			Assert.Equivalent(expected, actual);
		}

		[Theory]
		[AutoData]
		public void ExportReport_WritesToGivenPath(Report1970 expected)
		{
			// Arrange
			var exportPath = Path.Combine(Path.GetTempPath(), $"{expected.Metadata.Username}_{expected.Metadata.Year}.json");

			// Act
			_sut.ExportReport(expected, exportPath);

			// Assert
			var exported = File.ReadAllText(exportPath);
			var actual = JsonConvert.DeserializeObject<Report1970>(exported);
			Assert.Equivalent(expected, actual);
			File.Delete(exportPath);
		}

		[Theory]
		[AutoData]
		public void ImportReport_WritesToCorrectFriendFile(Report1970 expected)
		{
			// Arrange
			var importPath = Path.Combine(Path.GetTempPath(), $"{expected.Metadata.Username}_{expected.Metadata.Year}.json");
			File.WriteAllText(importPath, JsonConvert.SerializeObject(expected));

			Directory.CreateDirectory(_reportsPath);
			var friendsPath = Path.Combine(_reportsPath, expected.Metadata.Year.ToString(), "Friends");
			Directory.CreateDirectory(friendsPath);
			var expectedPath = Path.Combine(friendsPath, $"{expected.Metadata.Username}_{expected.Metadata.Year}.json");

			// Act
			var persistedReport = _sut.ImportReport(importPath);

			// Assert
			var imported = File.ReadAllText(expectedPath);
			var actual = JsonConvert.DeserializeObject<Report1970>(imported);
			Assert.Equivalent(expected, actual);
			Assert.Equal(expected.Metadata.Id, persistedReport.Id);
			Assert.False(persistedReport.IsOwn);
			Assert.Equal(expectedPath, persistedReport.FilePath);
			Assert.Equal(expected.Metadata.Year, persistedReport.Year);
			Assert.Equal(expected.Metadata.Username, persistedReport.Username);
			Assert.Equal(expected.TotalPlaytime, persistedReport.TotalPlaytime);
			File.Delete(importPath);
		}

		[Theory]
		[AutoData]
		public void ImportReport_CreatesFriendsDirectories_WhenTheyDoNotExist(Report1970 expected)
		{
			// Arrange
			var importPath = Path.Combine(Path.GetTempPath(), $"{expected.Metadata.Username}_{expected.Metadata.Year}.json");
			File.WriteAllText(importPath, JsonConvert.SerializeObject(expected));

			// Act
			var persistedReport = _sut.ImportReport(importPath);

			// Assert
			Assert.Equal(expected.Metadata.Id, persistedReport.Id);
			File.Delete(importPath);
		}

		[Theory]
		[AutoData]
		public void ImportReport_SanitizesUsernames_WhenUsernamesHaveInvalidChars(Report1970 expected)
		{
			// Arrange
			expected.Metadata.Username = "user/name:?<>";
			var importPath = Path.Combine(Path.GetTempPath(), "to_be_imported.json");
			File.WriteAllText(importPath, JsonConvert.SerializeObject(expected));

			Directory.CreateDirectory(_reportsPath);
			var friendsPath = Path.Combine(_reportsPath, expected.Metadata.Year.ToString(), "Friends");
			Directory.CreateDirectory(friendsPath);
			var expectedPath = Path.Combine(friendsPath, $"user_name_____{expected.Metadata.Year}.json");

			// Act
			var persistedReport = _sut.ImportReport(importPath);

			// Assert
			Assert.Equal(expectedPath, persistedReport.FilePath);
			File.Delete(importPath);
		}

		[Theory]
		[InlineAutoData(nameof(Report1970.Metadata))]
		[InlineAutoData(nameof(Metadata.Id))]
		[InlineAutoData(nameof(Metadata.Year))]
		[InlineAutoData(nameof(Metadata.Username))]
		public void ImportReport_ThrowsException_WhenReportIsInvalid(
			string propertyName,
			Report1970 report)
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

			var importPath = Path.Combine(Path.GetTempPath(), "some_file.json");
			File.WriteAllText(importPath, JsonConvert.SerializeObject(report));

			// Act
			var exception = Record.Exception(() => _sut.ImportReport(importPath));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<InvalidDataException>(exception);
			Assert.Equal($"File \"{importPath}\" is not a valid report file.", exception.Message);
			File.Delete(importPath);
		}
	}
}