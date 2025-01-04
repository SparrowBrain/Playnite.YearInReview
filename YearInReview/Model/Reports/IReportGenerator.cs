using System.Collections.Generic;
using System.Threading.Tasks;
using YearInReview.Model.Reports._1970;

namespace YearInReview.Model.Reports
{
	public interface IReportGenerator
	{
		Task<IReadOnlyCollection<Report1970>> GenerateAllYears();

		Task<Report1970> Generate(int year);
	}
}