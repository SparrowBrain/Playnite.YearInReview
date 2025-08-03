using System.Collections.Generic;
using YearInReview.Model;

namespace YearInReview.Settings
{
	public class YearInReviewSettings : ObservableObject, IVersionedSettings
	{
		public const int CurrentVersion = 1;

		private string _username;
		private bool _showSidebarItem;
		private bool _showNewReportNotifications;
		private bool _saveWithImages;

		public YearInReviewSettings()
		{
			Version = CurrentVersion;
		}

		public static YearInReviewSettings Default => new YearInReviewSettings
		{
			ShowSidebarItem = true,
			ShowNewReportNotifications = true,
			SaveWithImages = true,
		};

		public int Version { get; }

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

		public RememberedChoice ExportWithImages { get; set; }

		public ExportFormat ExportFormat { get; set; }

		public bool SaveWithImages
		{
			get => _saveWithImages;
			set => SetValue(ref _saveWithImages, value);
		}
	}
}