namespace YearInReview.Settings
{
	internal class PluginSettingsPersistence : IPluginSettingsPersistence
	{
		private readonly YearInReview _plugin;

		public PluginSettingsPersistence(YearInReview plugin)
		{
			_plugin = plugin;
		}

		public T LoadPluginSettings<T>() where T : class
		{
			return _plugin.LoadPluginSettings<T>();
		}

		public void SavePluginSettings<T>(T settings) where T : class
		{
			_plugin.SavePluginSettings(settings);
		}
	}
}