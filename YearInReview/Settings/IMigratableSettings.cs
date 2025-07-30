namespace YearInReview.Settings
{
	public interface IMigratableSettings : IVersionedSettings
	{
		IVersionedSettings Migrate();
	}
}