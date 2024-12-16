using Playnite.SDK;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using YearInReview.Infrastructure.Services;

namespace YearInReview.Infrastructure.UserControls
{
	/// <summary>
	/// Interaction logic for PlaytimeCalendar.xaml
	/// </summary>
	public partial class PlaytimeCalendar : UserControl
	{
		private const double GridCellSize = 50;
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly IDictionary<float, SolidColorBrush> _colorCache = new Dictionary<float, SolidColorBrush>();
		private IReadOnlyList<CalendarDayViewModel> _playtimeCalendarDays;

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register(nameof(ItemsSource), typeof(IReadOnlyList<CalendarDayViewModel>), typeof(PlaytimeCalendar), new PropertyMetadata(null, OnItemsSourceChanged));

		public PlaytimeCalendar()
		{
			InitializeComponent();
		}

		public IReadOnlyList<CalendarDayViewModel> ItemsSource
		{
			get => (IReadOnlyList<CalendarDayViewModel>)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as PlaytimeCalendar;
			control?.OnItemsSourceChanged(e);
		}

		private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			_playtimeCalendarDays = e.NewValue as IReadOnlyList<CalendarDayViewModel>;
			AddYear();
		}

		private void AddYear()
		{
			for (var i = 0; i < 12; i++)
			{
				var monthDays = _playtimeCalendarDays.Where(a => a.Date.Month == i + 1).ToList();
				AddMonth(monthDays);
			}

			_colorCache.Clear();
		}

		private void AddMonth(IReadOnlyCollection<CalendarDayViewModel> monthDays)
		{
			if (monthDays == null || !monthDays.Any())
			{
				_logger.Warn("No days to display in month.");
				return;
			}

			var grid = InitializeMonthGrid();
			AddMonthHeader(monthDays, grid);

			var dayOfWeekOffset = GetDayOfWeekOffset(monthDays);
			foreach (var day in monthDays)
			{
				SetupDayCell(day, dayOfWeekOffset, grid);
			}

			MainPanel.Children.Add(grid);
		}

		private static Grid InitializeMonthGrid()
		{
			var grid = new Grid();
			for (var i = 0; i < 6; i++)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(GridCellSize) });
			}

			for (var i = 0; i < 8; i++)
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(GridCellSize) });
			}

			return grid;
		}

		private static void AddMonthHeader(IReadOnlyCollection<CalendarDayViewModel> monthDays, Grid grid)
		{
			var header = new TextBlock
			{
				Text = monthDays.First().Date.ToString("MMMM"),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				FontWeight = FontWeights.Bold
			};
			Grid.SetRow(header, 0);
			Grid.SetColumnSpan(header, 6);
			grid.Children.Add(header);
		}

		private static int GetDayOfWeekOffset(IReadOnlyCollection<CalendarDayViewModel> monthDays)
		{
			var regionalFirstDayOfWeek = (int)System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

			var firstOfMonthDayOfWeek = (int)monthDays.First().Date.DayOfWeek;
			var dayOfWeekOffset = firstOfMonthDayOfWeek - regionalFirstDayOfWeek >= 0
				? firstOfMonthDayOfWeek - regionalFirstDayOfWeek
				: firstOfMonthDayOfWeek - regionalFirstDayOfWeek + 7;
			return dayOfWeekOffset;
		}

		private void SetupDayCell(CalendarDayViewModel day, int dayOfWeekOffset, Grid grid)
		{
			var row = (day.Date.Day - 1 + dayOfWeekOffset) % 7 + 1;
			var col = (day.Date.Day - 1 + dayOfWeekOffset) / 7;

			var dayColorBrush = GetDayColorBrush(day);
			var tooltip = CreateDayTooltip(day);

			var border = new Border
			{
				BorderBrush = Brushes.Black,
				BorderThickness = new Thickness(1),
				Background = dayColorBrush,
				Width = GridCellSize,
				Height = GridCellSize,
				ToolTip = tooltip
			};
			Grid.SetRow(border, row);
			Grid.SetColumn(border, col);
			grid.Children.Add(border);
		}

		private SolidColorBrush GetDayColorBrush(CalendarDayViewModel day)
		{
			if (_colorCache.TryGetValue(day.Opacity, out var color))
			{
				return color;
			}

			var blendedColor = Color.FromArgb(
				(byte)(Colors.DimGray.A * (1 - day.Opacity) + Colors.LimeGreen.A * day.Opacity),
				(byte)(Colors.DimGray.R * (1 - day.Opacity) + Colors.LimeGreen.R * day.Opacity),
				(byte)(Colors.DimGray.G * (1 - day.Opacity) + Colors.LimeGreen.G * day.Opacity),
				(byte)(Colors.DimGray.B * (1 - day.Opacity) + Colors.LimeGreen.B * day.Opacity)
			);

			var dayColorBrush = new SolidColorBrush(blendedColor);
			_colorCache[day.Opacity] = dayColorBrush;

			return dayColorBrush;
		}

		private static Grid CreateDayTooltip(CalendarDayViewModel day)
		{
			var tooltipGrid = new Grid();
			tooltipGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			tooltipGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

			AddTooltipDate(day, tooltipGrid);

			if (day.TotalPlaytime > 0)
			{
				AddTooltipTotalPlaytime(day, tooltipGrid);
			}

			AddTooltipGames(day, tooltipGrid);
			return tooltipGrid;
		}

		private static void AddTooltipDate(CalendarDayViewModel day, Grid tooltipGrid)
		{
			var dateText = new TextBlock
			{
				Text = day.Date.ToString("d"),
				HorizontalAlignment = HorizontalAlignment.Left,
				FontWeight = FontWeights.Bold
			};
			Grid.SetRow(dateText, 0);
			Grid.SetColumnSpan(dateText, 2);
			tooltipGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			tooltipGrid.Children.Add(dateText);
		}

		private static void AddTooltipTotalPlaytime(CalendarDayViewModel day, Grid tooltipGrid)
		{
			var totalPlaytimeLabel = new TextBlock
			{
				Text = ResourceProvider.GetString("LOC_YearInReview_PlaytimeCalendar"),
				HorizontalAlignment = HorizontalAlignment.Left,
				FontWeight = FontWeights.Bold
			};
			Grid.SetRow(totalPlaytimeLabel, 1);
			Grid.SetColumn(totalPlaytimeLabel, 0);
			tooltipGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			tooltipGrid.Children.Add(totalPlaytimeLabel);

			var totalPlaytimeText = new TextBlock
			{
				Text = ReadableTimeFormatter.FormatTime(day.TotalPlaytime),
				HorizontalAlignment = HorizontalAlignment.Left,
				FontWeight = FontWeights.Bold
			};
			Grid.SetRow(totalPlaytimeText, 1);
			Grid.SetColumn(totalPlaytimeText, 1);
			tooltipGrid.Children.Add(totalPlaytimeText);
		}

		private static void AddTooltipGames(CalendarDayViewModel day, Grid tooltipGrid)
		{
			var gameRow = 2;
			foreach (var game in day.Games)
			{
				var gameNameText = new TextBlock
				{
					Text = game.Name,
					HorizontalAlignment = HorizontalAlignment.Left
				};
				Grid.SetRow(gameNameText, gameRow);
				Grid.SetColumn(gameNameText, 0);
				tooltipGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
				tooltipGrid.Children.Add(gameNameText);

				var gameTimeText = new TextBlock
				{
					Text = ReadableTimeFormatter.FormatTime(game.TimePlayed),
					HorizontalAlignment = HorizontalAlignment.Left
				};
				Grid.SetRow(gameTimeText, gameRow);
				Grid.SetColumn(gameTimeText, 1);
				tooltipGrid.Children.Add(gameTimeText);

				gameRow++;
			}
		}
	}
}