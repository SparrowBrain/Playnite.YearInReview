namespace YearInReview.Settings
{
	public interface ISettingsMigrator
	{
		YearInReviewSettings LoadAndMigrateToNewest(int version);
	}
}