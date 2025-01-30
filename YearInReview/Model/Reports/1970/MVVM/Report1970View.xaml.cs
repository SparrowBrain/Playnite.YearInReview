using Playnite.SDK.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace YearInReview.Model.Reports._1970.MVVM
{
	/// <summary>
	/// Interaction logic for Report1970UserControl.xaml
	/// </summary>
	public partial class Report1970View : PluginUserControl
	{
		private const double MainGridBreakpoint = 1000;

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
			parent?.RaiseEvent(eventArg);
		}

		private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!(sender is Grid grid))
			{
				return;
			}

			if (grid.ActualWidth < MainGridBreakpoint)
			{
				grid.ColumnDefinitions[0].Width = new GridLength(0);
				grid.ColumnDefinitions[2].Width = new GridLength(0);
			}
			else
			{
				grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
				grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
			}
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

		private void GameGrid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateBarMaxWidth(sender, "GameBarColumn", "GameBar");
		}

		private void SourceGrid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateBarMaxWidth(sender, "SourceBarColumn", "SourceBar");
		}

		private static void UpdateBarMaxWidth(object sender, string columnName, string barName)
		{
			if (!(sender is Grid grid))
			{
				return;
			}

			var gameBarColumn = grid.ColumnDefinitions.FirstOrDefault(col => col.Name == columnName);
			if (gameBarColumn == null)
			{
				return;
			}

			var columnWidth = gameBarColumn.ActualWidth;
			if (columnWidth <= 0)
			{
				return;
			}

			foreach (var element in FindVisualChildren<FrameworkElement>(grid))
			{
				if (element.Name == barName)
				{
					element.MaxWidth = columnWidth;
				}
			}
		}

		private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
		{
			if (depObj == null)
			{
				yield break;
			}

			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				var child = VisualTreeHelper.GetChild(depObj, i);
				if (child is T dependencyObject)
				{
					yield return dependencyObject;
				}

				foreach (var childOfChild in FindVisualChildren<T>(child))
				{
					yield return childOfChild;
				}
			}
		}
	}
}