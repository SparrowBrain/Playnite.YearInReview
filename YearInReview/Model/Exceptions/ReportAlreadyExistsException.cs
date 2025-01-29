using System;

namespace YearInReview.Model.Exceptions
{
	public class ReportAlreadyExistsException : Exception
	{
		public ReportAlreadyExistsException(string message) : base(message)
		{
		}
	}
}