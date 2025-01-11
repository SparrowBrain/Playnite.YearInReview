using Playnite.SDK;
using System.Linq;
using System.Threading.Tasks;
using YearInReview.Extensions.GameActivity;
using YearInReview.Settings;

namespace YearInReview
{
	public class ExtensionStartupValidator
	{
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly IYearInReview _plugin;
		private readonly IPlayniteAPI _playniteApi;

		public ExtensionStartupValidator(
			IYearInReview plugin,
			IPlayniteAPI playniteApi)
		{
			_plugin = plugin;
			_playniteApi = playniteApi;
		}

		public async Task<bool> IsOkToRun()
		{
			foreach (var loadedPlugins in _playniteApi.Addons.Plugins)
			{
				_logger.Debug("Installed extension: " + loadedPlugins);
			}

			if (_playniteApi.Addons.Plugins.All(x => x.Id != GameActivityExtension.ExtensionId))
			{
				_logger.Warn("GameActivity extension not installed. Cannot run YearInReview.");

				_playniteApi.Notifications.Add(new NotificationMessage(
				"year_in_review_game_activity_not_installed",
				ResourceProvider.GetString("LOC_YearInReview_Notification_InstallGameActivity"),
				NotificationType.Info,
				() => System.Diagnostics.Process.Start("https://playnite.link/addons.html#playnite-gameactivity-plugin")));
				return false;
			}

			var settings = _plugin.LoadPluginSettings<YearInReviewSettings>();
			if (string.IsNullOrEmpty(settings.Username))
			{
				_logger.Warn("Username is not set, plugin will not work correctly.");

				_playniteApi.Notifications.Add(new NotificationMessage(
				"year_in_review_missing_username",
				ResourceProvider.GetString("LOC_YearInReview_Notification_SetUsername"),
				NotificationType.Info,
				() => _plugin.OpenSettingsView()));
				return false;
			}

			return true;
		}
	}
}