using System.Collections.Generic;
using System.Windows.Input;

namespace YearInReview.Model.Reports.MVVM
{
	public class YearButtonViewModel : ObservableObject
	{
		public int Year { get; set; }


		public ICommand SwitchYearCommand { get; set; }
	}
}