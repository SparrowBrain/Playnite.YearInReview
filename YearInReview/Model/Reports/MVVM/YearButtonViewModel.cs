using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace YearInReview.Model.Reports.MVVM
{
	public class YearButtonViewModel : ObservableObject
	{
		public int Year { get; set; }
		public ICommand DisplayCommand { get; set; }
	}
}