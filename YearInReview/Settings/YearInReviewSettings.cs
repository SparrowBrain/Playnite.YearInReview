using System.Collections.Generic;

namespace YearInReview.Settings
{
	public class YearInReviewSettings : ObservableObject, IVersionedSettings
	{
		public const int CurrentVersion = 0;

		private string _username;
		private bool _showSidebarItem;
		private bool _showNewReportNotifications;

		public YearInReviewSettings()
		{
			Version = CurrentVersion;
		}

		public static YearInReviewSettings Default => new YearInReviewSettings
		{
			ShowSidebarItem = true,
			ShowNewReportNotifications = true,
		};

		public int Version { get; set; }

		public string Username
		{
			get => _username;
			set => SetValue(ref _username, value);
		}

		public bool ShowSidebarItem
		{
			get => _showSidebarItem;
			set => SetValue(ref _showSidebarItem, value);
		}

		public bool ShowNewReportNotifications
		{
			get => _showNewReportNotifications;
			set => SetValue(ref _showNewReportNotifications, value);
		}
	}
}