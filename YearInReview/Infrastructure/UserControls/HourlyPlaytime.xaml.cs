using System.Collections.Generic;
using System.Windows;
using Playnite.SDK.Controls;

namespace YearInReview.Infrastructure.UserControls
{
	/// <summary>
	/// Interaction logic for HourlyPlaytime.xaml
	/// </summary>
	public partial class HourlyPlaytime : PluginUserControl
	{
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register(nameof(ItemsSource), typeof(IReadOnlyList<HourlyPlaytimeViewModel>), typeof(HourlyPlaytime), new PropertyMetadata(null, OnItemsSourceChanged));

		private IReadOnlyList<HourlyPlaytimeViewModel> _hourlyPlaytime;

		public HourlyPlaytime()
		{
			InitializeComponent();
		}

		public IReadOnlyList<HourlyPlaytimeViewModel> ItemsSource
		{
			get => (IReadOnlyList<HourlyPlaytimeViewModel>)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as HourlyPlaytime;
			control?.OnItemsSourceChanged(e);
		}

		private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			_hourlyPlaytime = e.NewValue as IReadOnlyList<HourlyPlaytimeViewModel>;
		}
	}
}