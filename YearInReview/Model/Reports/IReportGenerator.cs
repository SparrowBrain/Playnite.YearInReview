using System.Collections.Generic;
using YearInReview.Model.Reports._1970;

namespace YearInReview.Model.Reports
{
	public interface IReportGenerator
	{
		IReadOnlyCollection<Report1970> GenerateAllYears();
		
		Report1970 Generate(int year);
	}
}