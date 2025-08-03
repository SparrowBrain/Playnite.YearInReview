using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using YearInReview.Infrastructure.Services;

namespace YearInReview.Infrastructure.UserControls
{
	/// <summary>
	/// Interaction logic for RadialClockGraph.xaml
	/// </summary>
	public partial class RadialClockGraph : UserControl
	{
		public static readonly DependencyProperty HourlyDataProperty =
			DependencyProperty.Register(
				nameof(HourlyData),
				typeof(IList<int>),
				typeof(RadialClockGraph),
				new PropertyMetadata(null, OnHourlyDataChanged));

		public IList<int> HourlyData
		{
			get => (IList<int>)GetValue(HourlyDataProperty);
			set => SetValue(HourlyDataProperty, value);
		}

		public RadialClockGraph()
		{
			InitializeComponent();
			Loaded += RadialClockGraph_Loaded;
			SizeChanged += RadialClockGraph_SizeChanged;
		}

		private void RadialClockGraph_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (RadialCanvas != null)
			{
				RadialCanvas.Height = RadialCanvas.ActualWidth;
				DrawRadialGraph();
			}
		}

		private void RadialClockGraph_Loaded(object sender, RoutedEventArgs e)
		{
			DrawRadialGraph();
		}

		private static void OnHourlyDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as RadialClockGraph;
			if (control?.IsLoaded ?? false)
			{
				control.DrawRadialGraph();
			}
		}

		private void DrawRadialGraph()
		{
			if (HourlyData == null || HourlyData.Count != 24 || RadialCanvas == null)
			{
				return;
			}

			RadialCanvas.Children.Clear();

			var width = RadialCanvas.ActualWidth;
			var height = RadialCanvas.ActualHeight;
			var centerX = width / 2;
			var centerY = height / 2;
			var spaceForLabels = 30;
			var maxRadius = Math.Min(width, height) / 2 - spaceForLabels;
			var maxValue = HourlyData.Max();

			var angleStep = 360.0 / 24;

			DrawRadialSlices(maxValue, maxRadius, angleStep, centerX, centerY);
			DrawOuterCircle(maxRadius, centerX, centerY);
			DrawHourLabels(angleStep, maxRadius, centerX, centerY);
		}

		private void DrawRadialSlices(int maxValue, double maxRadius, double angleStep, double centerX, double centerY)
		{
			for (var hour = 0; hour < 24; hour++)
			{
				var value = HourlyData[hour];
				var radius = (value / (double)maxValue) * maxRadius;

				var startAngle = hour * angleStep - 90;
				var endAngle = startAngle + angleStep;

				var path = CreateRadialSlice(centerX, centerY, radius, startAngle, endAngle, maxRadius);

				path.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)(50 + hour * 8), 120, 200));
				path.Stroke = TryFindResource("PanelSeparatorBrush") as Brush ?? Brushes.Black;
				path.StrokeThickness = 1;

				AddTooltip(hour, value, path);

				RadialCanvas.Children.Add(path);
			}
		}

		private void DrawOuterCircle(double maxRadius, double centerX, double centerY)
		{
			var circle = new Ellipse
			{
				Width = maxRadius * 2,
				Height = maxRadius * 2,
				Stroke = Brushes.Gray,
				StrokeThickness = 2,
			};
			Canvas.SetLeft(circle, centerX - maxRadius);
			Canvas.SetTop(circle, centerY - maxRadius);
			RadialCanvas.Children.Add(circle);
		}

		private void DrawHourLabels(double angleStep, double maxRadius, double centerX, double centerY)
		{
			for (var hour = 0; hour < 24; hour++)
			{
				var angle = hour * angleStep - 90;
				var rad = angle * Math.PI / 180;

				var dateTime = new DateTime(1970, 1, 1, hour, 0, 0);
				var label = new TextBlock
				{
					Text = dateTime.ToString("t"),
					FontWeight = FontWeights.Bold,
				};
				label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				var labelSize = label.DesiredSize;

				var labelRadius = Math.Max(maxRadius + labelSize.Width, maxRadius + labelSize.Height);
				var x = centerX + labelRadius * Math.Cos(rad);
				var y = centerY + labelRadius * Math.Sin(rad);

				Canvas.SetLeft(label, x - labelSize.Width);
				Canvas.SetTop(label, y - labelSize.Height);
				RadialCanvas.Children.Add(label);
			}
		}

		private Path CreateRadialSlice(double cx, double cy, double radius, double startAngle, double endAngle, double maxRadius)
		{
			var startRad = startAngle * Math.PI / 180;
			var endRad = endAngle * Math.PI / 180;

			var p1 = new Point(cx + radius * Math.Cos(startRad), cy + radius * Math.Sin(startRad));
			var p2 = new Point(cx + radius * Math.Cos(endRad), cy + radius * Math.Sin(endRad));
			var p0 = new Point(cx, cy);

			var isLargeArc = (endAngle - startAngle) > 180;

			var figure = new PathFigure { StartPoint = p0 };
			figure.Segments.Add(new LineSegment(p1, true));
			figure.Segments.Add(new ArcSegment(p2, new Size(radius, radius), 0, isLargeArc, SweepDirection.Clockwise, true));
			figure.Segments.Add(new LineSegment(p0, true));

			var geometry = new PathGeometry();
			geometry.Figures.Add(figure);

			return new Path { Data = geometry };
		}

		private static void AddTooltip(int hour, int value, Path path)
		{
			var dateTime = new DateTime(1970, 1, 1, hour, 0, 0);
			var tooltip = new ToolTip
			{
				Content = $"{dateTime:t} - {ReadableTimeFormatter.FormatTime(value, true)}"
			};
			ToolTipService.SetToolTip(path, tooltip);
		}
	}
}