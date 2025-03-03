﻿using System.Collections.Generic;
using Newtonsoft.Json;
using YearInReview.Model.Reports._1970;

namespace YearInReview.Model.Reports.Persistence
{
	public interface IReportPersistence
	{
		void SaveReport(Report1970 report);

		IReadOnlyCollection<PersistedReport> PreLoadAllReports();

		Report1970 LoadReport(string filePath);

		void ExportReport(Report1970 report, string exportPath, JsonSerializerSettings serializerSettings);

		PersistedReport ImportReport(Report1970 importPath);
	}
}