using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Playnite.SDK.Controls;

namespace YearInReview.Model.Reports.MVVM
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : PluginUserControl
	{
		public MainView(MainViewModel viewModel)
		{
			DataContext = viewModel;
			InitializeComponent();
		}
	}
}