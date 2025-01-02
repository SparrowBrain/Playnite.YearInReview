using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using YearInReview.Model.Reports._1970;

namespace YearInReview.Model.Reports.Persistence
{
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
	}
}