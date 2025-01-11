using Playnite.SDK;

namespace YearInReview
{
	public interface IYearInReview
	{

		TSettings LoadPluginSettings<TSettings>() where TSettings : class;

		bool OpenSettingsView();
	}
}