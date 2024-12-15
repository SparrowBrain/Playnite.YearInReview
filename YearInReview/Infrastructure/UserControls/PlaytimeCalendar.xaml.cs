using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Reports._1970.MVVM;

namespace YearInReview.Infrastructure.UserControls
{
	/// <summary>
	/// Interaction logic for PlaytimeCalendar.xaml
	/// </summary>
	public partial class PlaytimeCalendar : UserControl
	{
		private IReadOnlyList<CalendarDayViewModel> _playtimeCalendarDays;

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IReadOnlyList<CalendarDayViewModel>), typeof(PlaytimeCalendar), new PropertyMetadata(null, OnItemsSourceChanged));

		public PlaytimeCalendar()
		{
			InitializeComponent();
		}

		public IReadOnlyList<CalendarDayViewModel> ItemsSource
		{
			get => (IReadOnlyList<CalendarDayViewModel>)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
		}

		private void AddMonth(IReadOnlyCollection<CalendarDayViewModel> monthDays)
		{
			if (monthDays == null || !monthDays.Any())
				return;

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
				Text = monthDays.First().Date.ToString("MMMM"),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				FontWeight = FontWeights.Bold
			};
			Grid.SetRow(header, 0);
			Grid.SetColumnSpan(header, 6);
			grid.Children.Add(header);

			// Determine the first day of the week based on regional settings
			DayOfWeek firstDayOfWeek = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

			// Add squares into every cell of the grid except the first row
			int dayOfWeek = (int)monthDays.First().Date.DayOfWeek;
			int firstDayOfWeekInt = (int)firstDayOfWeek;
			var dayOfWeekDelta = dayOfWeek - firstDayOfWeekInt >= 0
				? dayOfWeek - firstDayOfWeekInt
				: dayOfWeek - firstDayOfWeekInt + 7;

			foreach (var day in monthDays)
			{
				int row = (day.Date.Day - 1 + dayOfWeekDelta) % 7 + 1;
				int col = (day.Date.Day - 1 + dayOfWeekDelta) / 7;

				Grid tooltipGrid = new Grid();
				tooltipGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
				tooltipGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

				// Add date row
				TextBlock dateText = new TextBlock
				{
					Text = day.Date.ToString("d"),
					HorizontalAlignment = HorizontalAlignment.Left,
					FontWeight = FontWeights.Bold
				};
				Grid.SetRow(dateText, 0);
				Grid.SetColumnSpan(dateText, 2);
				tooltipGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
				tooltipGrid.Children.Add(dateText);

				// Add total playtime row
				if (day.TotalPlaytime > 0)
				{
					TextBlock totalPlaytimeLabel = new TextBlock
					{
						Text = "Total Playtime",
						HorizontalAlignment = HorizontalAlignment.Left,
						FontWeight = FontWeights.Bold
					};
					Grid.SetRow(totalPlaytimeLabel, 1);
					Grid.SetColumn(totalPlaytimeLabel, 0);
					tooltipGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
					tooltipGrid.Children.Add(totalPlaytimeLabel);

					TextBlock totalPlaytimeText = new TextBlock
					{
						Text = ReadableTimeFormatter.FormatTime(day.TotalPlaytime),
						HorizontalAlignment = HorizontalAlignment.Left,
						FontWeight = FontWeights.Bold
					};
					Grid.SetRow(totalPlaytimeText, 1);
					Grid.SetColumn(totalPlaytimeText, 1);
					tooltipGrid.Children.Add(totalPlaytimeText);
				}

				// Add game rows
				int gameRow = 2;
				foreach (var game in day.Games)
				{
					TextBlock gameNameText = new TextBlock
					{
						Text = game.Name,
						HorizontalAlignment = HorizontalAlignment.Left
					};
					Grid.SetRow(gameNameText, gameRow);
					Grid.SetColumn(gameNameText, 0);
					tooltipGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
					tooltipGrid.Children.Add(gameNameText);

					TextBlock gameTimeText = new TextBlock
					{
						Text = ReadableTimeFormatter.FormatTime(game.TimePlayed),
						HorizontalAlignment = HorizontalAlignment.Left
					};
					Grid.SetRow(gameTimeText, gameRow);
					Grid.SetColumn(gameTimeText, 1);
					tooltipGrid.Children.Add(gameTimeText);

					gameRow++;
				}

				// Blend LightGray and LightGreen based on opacity
				Color blendedColor = Color.FromArgb(
					(byte)(Colors.DimGray.A * (1 - day.Opacity) + Colors.LimeGreen.A * day.Opacity),
					(byte)(Colors.DimGray.R * (1 - day.Opacity) + Colors.LimeGreen.R * day.Opacity),
					(byte)(Colors.DimGray.G * (1 - day.Opacity) + Colors.LimeGreen.G * day.Opacity),
					(byte)(Colors.DimGray.B * (1 - day.Opacity) + Colors.LimeGreen.B * day.Opacity)
				);

				Border border = new Border
				{
					BorderBrush = Brushes.Black,
					BorderThickness = new Thickness(1),
					Background = new SolidColorBrush(blendedColor),
					Width = 50,
					Height = 50,
					ToolTip = tooltipGrid
				};
				Grid.SetRow(border, row);
				Grid.SetColumn(border, col);
				grid.Children.Add(border);
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
			_playtimeCalendarDays = e.NewValue as IReadOnlyList<CalendarDayViewModel>;

			for (int i = 0; i < 12; i++)
			{
				var monthDays = _playtimeCalendarDays.Where(a => a.Date.Month == i + 1).ToList();
				AddMonth(monthDays);
			}
		}
	}
}