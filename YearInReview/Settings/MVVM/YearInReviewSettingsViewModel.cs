using Playnite.SDK;
using Playnite.SDK.Data;
using System.Collections.Generic;

namespace YearInReview.Settings.MVVM
{
	public class YearInReviewSettingsViewModel : ObservableObject, ISettings
	{
		private readonly YearInReview _plugin;

		private YearInReviewSettings _editingClone;
		private YearInReviewSettings _settings;

		public YearInReviewSettings Settings
		{
			get => _settings;
			set
			{
				_settings = value;
				OnPropertyChanged();
			}
		}

		public YearInReviewSettingsViewModel(YearInReview plugin)
		{
			_plugin = plugin;

			var savedSettings = plugin.LoadPluginSettings<YearInReviewSettings>();
			Settings = savedSettings ?? new YearInReviewSettings();
		}

		public void BeginEdit()
		{
			_editingClone = Serialization.GetClone(Settings);
		}

		public void CancelEdit()
		{
			Settings = _editingClone;
		}

		public void EndEdit()
		{
			_plugin.SavePluginSettings(Settings);
		}

		public bool VerifySettings(out List<string> errors)
		{
			errors = new List<string>();
			if (string.IsNullOrEmpty(Settings.Username))
			{
				errors.Add(ResourceProvider.GetString("LOC_YearInReview_Settings_Error_UsernameEmpty"));
			}

			return errors.Count == 0;
		}
	}
}