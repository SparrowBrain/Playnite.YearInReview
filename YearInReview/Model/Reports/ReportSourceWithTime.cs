using System;

namespace YearInReview.Model.Reports
{
	public class ReportSourceWithTime
	{
		public Guid Id { get; set; }
		
		public string Name { get; set; }
		
		public int TimePlayed { get; set; }
	}
}