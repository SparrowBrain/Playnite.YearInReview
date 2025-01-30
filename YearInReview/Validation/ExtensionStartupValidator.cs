using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YearInReview.Extensions.GameActivity;
using YearInReview.Infrastructure.Services;
using YearInReview.Model.Reports.Persistence;
using YearInReview.Settings;

namespace YearInReview.Validation
{
	public class ExtensionStartupValidator
	{
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly IYearInReview _plugin;
		private readonly IPlayniteAPI _playniteApi;
		private readonly IReportPersistence _reportPersistence;
		private readonly IGameActivityExtension _gameActivityExtension;
		private readonly IDateTimeProvider _dateTimeProvider;

		public ExtensionStartupValidator(
			IYearInReview plugin,
			IPlayniteAPI playniteApi,
			IReportPersistence reportPersistence,
			IGameActivityExtension gameActivityExtension,
			IDateTimeProvider dateTimeProvider)
		{
			_plugin = plugin;
			_playniteApi = playniteApi;
			_reportPersistence = reportPersistence;
			_gameActivityExtension = gameActivityExtension;
			_dateTimeProvider = dateTimeProvider;
		}

		public async Task<IReadOnlyCollection<InitValidationError>> IsOkToRun()
		{
			var errors = new List<InitValidationError>();

			ValidateGameActivityExtensionInstalled(errors);
			ValidateUsernameSet(errors);
			await ValidateGameActivitySessionsExistForPreviousYears(errors);

			return errors;
		}

		private void ValidateGameActivityExtensionInstalled(List<InitValidationError> errors)
		{
			if (_playniteApi.Addons.Plugins.Any(x => x.Id == GameActivityExtension.ExtensionId))
			{
				return;
			}

			_logger.Warn("GameActivity extension not installed. Cannot run YearInReview.");

			CreateValidationError(
				errors,
				InitValidationError.GameActivityExtensionNotInstalled,
				"LOC_YearInReview_Notification_InstallGameActivity",
				() => System.Diagnostics.Process.Start("https://playnite.link/addons.html#playnite-gameactivity-plugin"));
		}

		private void ValidateUsernameSet(List<InitValidationError> errors)
		{
			var settings = _plugin.LoadPluginSettings<YearInReviewSettings>();
			if (!string.IsNullOrEmpty(settings.Username))
			{
				return;
			}

			_logger.Warn("Username is not set, plugin will not work correctly.");

			CreateValidationError(
				errors,
				InitValidationError.UsernameNotSetError,
				"LOC_YearInReview_Notification_SetUsername",
				() => _plugin.OpenSettingsView());
		}

		private async Task ValidateGameActivitySessionsExistForPreviousYears(List<InitValidationError> errors)
		{
			var persistedReports = _reportPersistence.PreLoadAllReports();
			if (persistedReports.Count == 0)
			{
				var currentYear = _dateTimeProvider.GetNow().Year;
				var games = _playniteApi.Database.Games;
				var activities = await _gameActivityExtension.GetActivityForGames(games);
				if (activities.All(x => x.Items.All(session => session.DateSession.Year >= currentYear)))
				{
					_logger.Warn("No GameActivity sessions found for previous years. Cannot run YearInReview.");

					CreateValidationError(
						errors,
						InitValidationError.NoActivityInPreviousYears,
						"LOC_YearInReview_Notification_NoActivityInPreviousYears",
						null);
				}
			}
		}

		private void CreateValidationError(
			List<InitValidationError> errors,
			string errorId,
			string messageKey,
			Action callToAction)
		{
			var message = ResourceProvider.GetString(messageKey);

			_playniteApi.Notifications.Add(new NotificationMessage(
				errorId,
				message,
				NotificationType.Info,
				callToAction));

			errors.Add(new InitValidationError()
			{
				Id = errorId,
				Message = message,
				CallToAction = callToAction
			});
		}
	}
}