using System;

namespace YearInReview.Model.Exceptions
{
	public class InvalidReportFileException : Exception
	{
		public InvalidReportFileException(string message) : base(message)
		{
		}
	}
}