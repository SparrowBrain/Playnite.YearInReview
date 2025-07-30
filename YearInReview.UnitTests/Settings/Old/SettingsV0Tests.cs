using AutoFixture.Xunit2;
using Xunit;
using YearInReview.Settings;
using YearInReview.Settings.Old;

namespace YearInReview.UnitTests.Settings.Old
{
	public class SettingsV0Tests
	{
		[Theory, AutoData]
		public void Migrate_MigratesToV1(
			SettingsV0 old)
		{
			// Act
			var result = old.Migrate() as YearInReviewSettings;

			// Assert
			Assert.NotNull(result);
			Assert.Equal(old.ExportWithImages, result.ExportWithImages);
			Assert.Equal(old.Username, result.Username);
			Assert.Equal(old.ShowSidebarItem, result.ShowSidebarItem);
			Assert.Equal(old.ShowNewReportNotifications, result.ShowNewReportNotifications);

			Assert.Equal(1, result.Version);
		}
	}
}