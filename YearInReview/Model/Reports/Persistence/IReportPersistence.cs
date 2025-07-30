using System.Collections.Generic;
using YearInReview.Model.Reports._1970;

namespace YearInReview.Model.Reports.Persistence
{
	public interface IReportPersistence
	{
		void SaveReport(Report1970 report, bool saveWithImages);

		IReadOnlyCollection<PersistedReport> PreLoadAllReports();

		Report1970 LoadReport(string filePath);

		void ExportReport(Report1970 report, string exportPath, bool exportWithImages);

		PersistedReport ImportReport(Report1970 importPath);
	}
}