using System.Windows.Controls;

namespace YearInReview.Progress.MVVM
{
	/// <summary>
	/// Interaction logic for ProgressView.xaml
	/// </summary>
	public partial class ProgressView : UserControl
	{
		public ProgressView(ProgressViewModel progressViewModel)
		{
			InitializeComponent();
			DataContext = progressViewModel;
		}
	}
}
