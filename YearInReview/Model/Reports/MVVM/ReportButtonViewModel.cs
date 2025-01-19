using System.Collections.Generic;
using System.Windows.Input;

namespace YearInReview.Model.Reports.MVVM
{
	public class ReportButtonViewModel : ObservableObject
	{
		public string Username { get; set; }

		public ICommand DisplayCommand { get; set; }
	}
}