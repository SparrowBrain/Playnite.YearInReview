namespace YearInReview.Settings
{
	public class StartupSettingsValidator
	{
		private readonly IPluginSettingsPersistence _pluginSettingsPersistence;
		private readonly ISettingsMigrator _settingsMigrator;

		public StartupSettingsValidator(IPluginSettingsPersistence pluginSettingsPersistence, ISettingsMigrator settingsMigrator)
		{
			_pluginSettingsPersistence = pluginSettingsPersistence;
			_settingsMigrator = settingsMigrator;
		}

		public void EnsureCorrectVersionSettingsExist()
		{
			var versionedSettings = _pluginSettingsPersistence.LoadPluginSettings<VersionedSettings>();
			if (versionedSettings == null)
			{
				_pluginSettingsPersistence.SavePluginSettings(YearInReviewSettings.Default);
				return;
			}

			if (versionedSettings.Version < YearInReviewSettings.CurrentVersion)
			{
				var newSettings = _settingsMigrator.LoadAndMigrateToNewest(versionedSettings.Version);
				_pluginSettingsPersistence.SavePluginSettings(newSettings);
			}
		}
	}
}