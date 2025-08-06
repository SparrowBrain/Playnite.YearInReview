using System;

namespace YearInReview.Validation
{
	public class InitValidationError
	{
		public const string UsernameNotSetError = "year_in_review_missing_username";
		public const string GameActivityExtensionNotInstalled = "year_in_review_game_activity_not_installed";
		public const string NoActivityInPreviousYears = "no_activity_in_previous_years";
		public const string NoActivityAtAll = "no_activity_at_all";

		public string Id { get; set; }

		public string Message { get; set; }

		public Action CallToAction { get; set; }
	}
}