using Playnite.SDK.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YearInReview.Model.Reports._1970.MVVM
{
	/// <summary>
	/// Interaction logic for Report1970UserControl.xaml
	/// </summary>
	public partial class Report1970View : PluginUserControl
	{
		public Report1970View(Report1970ViewModel viewModel)
		{
			DataContext = viewModel;
			InitializeComponent();
		}

		private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Handled)
			{
				return;
			}

			e.Handled = true;
			var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
			{
				RoutedEvent = UIElement.MouseWheelEvent,
				Source = sender
			};
			var parent = ((Control)sender).Parent as UIElement;
			parent.RaiseEvent(eventArg);
		}

		private void HourlyPlaytimeScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Handled)
			{
				return;
			}

			if (!(sender is ScrollViewer scrollViewer))
			{
				return;
			}

			if ((scrollViewer.HorizontalOffset == 0 && e.Delta > 0)
				|| (Math.Abs(scrollViewer.HorizontalOffset - scrollViewer.ScrollableWidth) < 0.1 && e.Delta < 0))
			{
				HandlePreviewMouseWheel(sender, e);
				return;
			}

			e.Handled = true;
			scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
		}
	}
}