using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Playnite.SDK;
using YearInReview.Extensions.GameActivity;
using YearInReview.Settings;

namespace YearInReview.Validation
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

		public async Task<IReadOnlyCollection<InitValidationError>> IsOkToRun()
		{
			var errors = new List<InitValidationError>();

			if (_playniteApi.Addons.Plugins.All(x => x.Id != GameActivityExtension.ExtensionId))
			{
				_logger.Warn("GameActivity extension not installed. Cannot run YearInReview.");

				var message = ResourceProvider.GetString("LOC_YearInReview_Notification_InstallGameActivity");
				Action callToAction = () => System.Diagnostics.Process.Start("https://playnite.link/addons.html#playnite-gameactivity-plugin");

				_playniteApi.Notifications.Add(new NotificationMessage(
				InitValidationError.GameActivityExtensionNotInstalled,
				message,
				NotificationType.Info,
				callToAction));

				errors.Add(new InitValidationError
				{
					Id = InitValidationError.GameActivityExtensionNotInstalled,
					Message = message,
					CallToAction = callToAction
				});
			}

			var settings = _plugin.LoadPluginSettings<YearInReviewSettings>();
			if (string.IsNullOrEmpty(settings.Username))
			{
				_logger.Warn("Username is not set, plugin will not work correctly.");

				var message = ResourceProvider.GetString("LOC_YearInReview_Notification_SetUsername");
				Action callToAction = () => _plugin.OpenSettingsView();

				_playniteApi.Notifications.Add(new NotificationMessage(
				InitValidationError.UsernameNotSetError,
				message,
				NotificationType.Info,
				callToAction));
				errors.Add(new InitValidationError()
				{
					Id = InitValidationError.UsernameNotSetError,
					Message = message,
					CallToAction = callToAction
				});
			}

			return errors;
		}
	}
}