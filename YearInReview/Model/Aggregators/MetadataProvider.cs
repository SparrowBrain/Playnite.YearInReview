using YearInReview.Infrastructure.Services;
using YearInReview.Model.Aggregators.Data;
using YearInReview.Settings;

namespace YearInReview.Model.Aggregators
{
	public class MetadataProvider : IMetadataProvider
	{
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IPluginSettingsPersistence _pluginSettingsPersistence;

		public MetadataProvider(IDateTimeProvider dateTimeProvider, IPluginSettingsPersistence pluginSettingsPersistence)
		{
			_dateTimeProvider = dateTimeProvider;
			_pluginSettingsPersistence = pluginSettingsPersistence;
		}

		public Metadata Get(int year)
		{
			return new Metadata
			{
				Year = year,
				GeneratedTimestamp = _dateTimeProvider.GetNow(),
				Username = _pluginSettingsPersistence.LoadPluginSettings<YearInReviewSettings>().Username
			};
		}
	}
}