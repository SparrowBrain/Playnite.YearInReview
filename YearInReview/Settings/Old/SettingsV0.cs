using YearInReview.Model;

namespace YearInReview.Settings.Old
{
	public class SettingsV0 : IMigratableSettings
	{
		public int Version { get; protected set; } = 0;

		public virtual IVersionedSettings Migrate()
		{
			var settings = YearInReviewSettings.Default;
			settings.Username = Username;
			settings.ShowSidebarItem = ShowSidebarItem;
			settings.ShowNewReportNotifications = ShowNewReportNotifications;
			settings.ExportWithImages = ExportWithImages;
			return settings;
		}

		public string Username { get; set; }

		public bool ShowSidebarItem { get; set; }

		public bool ShowNewReportNotifications { get; set; }

		public RememberedChoice ExportWithImages { get; set; }
	}
}