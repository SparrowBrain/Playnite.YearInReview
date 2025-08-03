using Playnite.SDK.Controls;

namespace YearInReview.Model.Reports.MVVM
{
	/// <summary>
	/// Interaction logic for ExportWithImagesView.xaml
	/// </summary>
	public partial class ExportWithImagesView : PluginUserControl
	{
		public ExportWithImagesView(ExportWithImagesViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}