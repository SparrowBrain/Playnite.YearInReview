using Playnite.SDK.Plugins;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
