﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using YearInReview.Model.Reports._1970;

namespace YearInReview.Model.Reports.Persistence
{
	public class ReportPersistence : IReportPersistence
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

		public IReadOnlyCollection<PersistedReport> PreLoadAllReports()
		{
			if (!Directory.Exists(_reportsPath))
			{
				return new List<PersistedReport>();
			}

			var persistedReports = new List<PersistedReport>();
			foreach (var yearDirectory in Directory.GetDirectories(_reportsPath))
			{
				var userReportFilePath = Path.Combine(yearDirectory, "user.json");
				if (File.Exists(userReportFilePath))
				{
					var report = JsonConvert.DeserializeObject<SlimReport>(File.ReadAllText(userReportFilePath));
					persistedReports.Add(new PersistedReport()
					{
						Id = report.Metadata.Id,
						IsOwn = true,
						FilePath = userReportFilePath,
						Username = report.Metadata.Username,
						Year = report.Metadata.Year,
						TotalPlaytime = report.TotalPlaytime,
					});
				}

				var friendsDirectory = Path.Combine(yearDirectory, "Friends");
				if (Directory.Exists(friendsDirectory))
				{
					foreach (var friendReportFilePath in Directory.GetFiles(friendsDirectory))
					{
						var report = JsonConvert.DeserializeObject<SlimReport>(File.ReadAllText(friendReportFilePath));
						persistedReports.Add(new PersistedReport()
						{
							Id = report.Metadata.Id,
							IsOwn = false,
							FilePath = friendReportFilePath,
							Username = report.Metadata.Username,
							Year = report.Metadata.Year,
							TotalPlaytime = report.TotalPlaytime,
						});
					}
				}
			}

			return persistedReports;
		}

		public Report1970 LoadReport(string filePath)
		{
			var contents = File.ReadAllText(filePath);
			return JsonConvert.DeserializeObject<Report1970>(contents);
		}

		// TODO: Don't forget to sanitize username
		public void ExportReport(Report1970 report, string exportPath)
		{
			var serialized = JsonConvert.SerializeObject(report);
			File.WriteAllText(exportPath, serialized);
		}

		public PersistedReport ImportReport(string importPath)
		{
			var contents = File.ReadAllText(importPath);
			var report = JsonConvert.DeserializeObject<Report1970>(contents);
			ValidateImportedReport(report, importPath);

			var friendsPath = Path.Combine(_reportsPath, report.Metadata.Year.ToString(), "Friends");
			var importedFilePath = Path.Combine(friendsPath, GetSanitizedFriendFileName(report));

			if (!Directory.Exists(friendsPath))
			{
				Directory.CreateDirectory(friendsPath);
			}

			File.WriteAllText(importedFilePath, contents);

			return new PersistedReport()
			{
				Id = report.Metadata.Id,
				IsOwn = false,
				FilePath = importedFilePath,
				Username = report.Metadata.Username,
				Year = report.Metadata.Year,
				TotalPlaytime = report.TotalPlaytime,
			};
		}

		private static void ValidateImportedReport(Report1970 report, string importPath)
		{
			if (report.Metadata == null
				|| report.Metadata.Id == Guid.Empty
				|| report.Metadata.Year == 0
				|| string.IsNullOrEmpty(report.Metadata.Username))
			{
				throw new InvalidDataException($"File \"{importPath}\" is not a valid report file.");
			}
		}

		private static string GetSanitizedFriendFileName(Report1970 report)
		{
			var fileName = $"{report.Metadata.Username}_{report.Metadata.Year}.json";
			foreach (var invalidChar in Path.GetInvalidFileNameChars())
			{
				fileName = fileName.Replace(invalidChar, '_');
			}

			return fileName;
		}
	}
}