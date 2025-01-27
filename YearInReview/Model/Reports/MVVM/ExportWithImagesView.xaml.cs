using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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