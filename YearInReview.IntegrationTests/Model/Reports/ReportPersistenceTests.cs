using AutoFixture.Xunit2;
using Newtonsoft.Json;
using System;
using System.IO;
using Xunit;
using YearInReview.Model.Reports._1970;

namespace YearInReview.IntegrationTests.Model.Reports
{
	public class ReportPersistenceTests
	{
		private const string TestDataPath = @"Model\Reports\TestData";
		private const string ExtensionDataPath = @"Model\Reports\ExtensionPath";

		private readonly ReportPersistence _sut;
		private readonly string _reportsPath = Path.Combine(ExtensionDataPath, "Reports");

		public ReportPersistenceTests()
		{
			_sut = new ReportPersistence(ExtensionDataPath);
		}

		[Fact]
		public void SaveReport_WhenReportIsNull_ThrowsArgumentNullException()
		{
			// Act
			var exception = Record.Exception(() => _sut.SaveReport(null));

			// Assert
			Assert.IsType<ArgumentNullException>(exception);
		}

		[Theory]
		[AutoData]
		public void SaveReport_WhenReportIsNotNull_PersistsReport(Report1970 report)
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
		public void DoesAnyUserReportExist_WhenReportDirectoryDoesNotExist_ReturnsFalse()
		{
			// Arrange
			if (Directory.Exists(_reportsPath))
			{
				Directory.Delete(_reportsPath, true);
			}

			// Act
			var result = _sut.DoesAnyUserReportExist();

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void DoesAnyUserReportExist_WhenReportDirectoryExistsButNoUserJson_ReturnsFalse()
		{
			// Arrange
			if (Directory.Exists(_reportsPath))
			{
				Directory.Delete(_reportsPath, true);
			}

			Directory.CreateDirectory(_reportsPath);
			Directory.CreateDirectory(Path.Combine(_reportsPath, "2023"));

			// Act
			var result = _sut.DoesAnyUserReportExist();

			// Assert
			Assert.False(result);
		}

		[Theory]
		[AutoData]
		public void DoesAnyUserReportExist_WhenReportDirectoryExistsAndUserJson_ReturnsTrue(Report1970 report)
		{
			// Arrange
			if (Directory.Exists(_reportsPath))
			{
				Directory.Delete(_reportsPath, true);
			}

			Directory.CreateDirectory(_reportsPath);
			Directory.CreateDirectory(Path.Combine(_reportsPath, "2023"));
			File.WriteAllText(Path.Combine(_reportsPath, "2023", "user.json"), JsonConvert.SerializeObject(report));

			// Act
			var result = _sut.DoesAnyUserReportExist();

			// Assert
			Assert.True(result);
		}
	}

	public class ReportPersistence
	{
		private readonly string _reportsPath;

		public ReportPersistence(string extensionPath)
		{
			_reportsPath = Path.Combine(extensionPath, "Reports");
		}

		public void SaveReport(Report1970 report)
		{
			if (report == null)
			{
				throw new ArgumentNullException(nameof(report));
			}

			var yearDirectory = Path.Combine(_reportsPath, report.Metadata.Year.ToString());
			if (!Directory.Exists(yearDirectory))
			{
				Directory.CreateDirectory(yearDirectory);
			}

			var userReportPath = Path.Combine(yearDirectory, "user.json");
			var json = JsonConvert.SerializeObject(report);
			File.WriteAllText(userReportPath, json);
		}

		public bool DoesAnyUserReportExist()
		{
			if (!Directory.Exists(_reportsPath))
			{
				return false;
			}

			foreach (var yearDirectory in Directory.GetDirectories(_reportsPath))
			{
				if (File.Exists(Path.Combine(yearDirectory, "user.json")))
				{
					return true;
				}
			}

			return false;
		}
	}
}