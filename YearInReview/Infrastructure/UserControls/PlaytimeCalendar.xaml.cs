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
using YearInReview.Model.Reports._1970;

namespace YearInReview.Infrastructure.UserControls
{
	/// <summary>
	/// Interaction logic for PlaytimeCalendar.xaml
	/// </summary>
	public partial class PlaytimeCalendar : UserControl
	{
		private IReadOnlyList<ReportCalendarDay> _playtimeCalendarDays;

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IReadOnlyList<ReportCalendarDay>), typeof(PlaytimeCalendar), new PropertyMetadata(null, OnItemsSourceChanged));

		public PlaytimeCalendar()
		{
			InitializeComponent();
			//AddMonth();
		}

		public IReadOnlyList<ReportCalendarDay> ItemsSource
		{
			get => (IReadOnlyList<ReportCalendarDay>)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			for (int i = 0; i < 12; i++)
			{
				AddMonth();
			}
		}

		private void AddMonth()
		{
			Grid grid = new Grid();
			for (int i = 0; i < 6; i++)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
			}

			for (int i = 0; i < 8; i++)
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
			}

			// Add header row
			TextBlock header = new TextBlock
			{
				Text = "January",
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				FontWeight = FontWeights.Bold
			};
			Grid.SetRow(header, 0);
			Grid.SetColumnSpan(header, 6);
			grid.Children.Add(header);

			// Add squares into every cell of the grid except the first row
			for (int row = 1; row < 8; row++)
			{
				for (int col = 0; col < 6; col++)
				{
					Border border = new Border
					{
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(1),
						Background = Brushes.LightGray,
						Width = 50,
						Height = 50
					};
					Grid.SetRow(border, row);
					Grid.SetColumn(border, col);
					grid.Children.Add(border);
				}
			}

			// Add the grid to the MainPanel
			MainPanel.Children.Add(grid);
		}

		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as PlaytimeCalendar;
			control.OnItemsSourceChanged(e);
		}

		private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			_playtimeCalendarDays = e.NewValue as IReadOnlyList<ReportCalendarDay>;
			AddMonth();
		}
	}
}