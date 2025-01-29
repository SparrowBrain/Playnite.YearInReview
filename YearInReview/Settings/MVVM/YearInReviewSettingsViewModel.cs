using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using YearInReview.Model;

namespace YearInReview.Settings.MVVM
{
	public class YearInReviewSettingsViewModel : ObservableObject, ISettings
	{
		private readonly YearInReview _plugin;

		private YearInReviewSettings _editingClone;
		private YearInReviewSettings _settings;

		public YearInReviewSettingsViewModel(YearInReview plugin)
		{
			_plugin = plugin;

			var savedSettings = plugin.LoadPluginSettings<YearInReviewSettings>();
			Settings = savedSettings ?? new YearInReviewSettings();
		}

		public event Action SettingsSaved;

		public YearInReviewSettings Settings
		{
			get => _settings;
			set
			{
				_settings = value;
				OnPropertyChanged();
				switch (value.ExportWithImages)
				{
					case RememberedChoice.Ask:
						ExportWithImagesAsk = true;
						ExportWithImagesNever = false;
						ExportWithImagesAlways = false;
						break;

					case RememberedChoice.Never:
						ExportWithImagesAsk = false;
						ExportWithImagesNever = true;
						ExportWithImagesAlways = false;

						break;

					case RememberedChoice.Always:
						ExportWithImagesAsk = false;
						ExportWithImagesNever = false;
						ExportWithImagesAlways = true;
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(value.ExportWithImages));
				}
			}
		}

		public bool ExportWithImagesAsk
		{
			get => Settings.ExportWithImages == RememberedChoice.Ask;
			set => SetExportWithImagesRadioButtonValue(nameof(ExportWithImagesAsk), RememberedChoice.Ask, value);
		}

		public bool ExportWithImagesNever
		{
			get => Settings.ExportWithImages == RememberedChoice.Never;
			set => SetExportWithImagesRadioButtonValue(nameof(ExportWithImagesNever), RememberedChoice.Never, value);
		}

		public bool ExportWithImagesAlways
		{
			get => Settings.ExportWithImages == RememberedChoice.Always;
			set => SetExportWithImagesRadioButtonValue(nameof(ExportWithImagesAlways), RememberedChoice.Always, value);
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
			OnSettingsSaved();
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

		protected virtual void OnSettingsSaved()
		{
			SettingsSaved?.Invoke();
		}

		private void SetExportWithImagesRadioButtonValue(
			string propertyName,
			RememberedChoice radioButtonOption,
			bool value)
		{
			if (Settings.ExportWithImages == radioButtonOption == value)
			{
				return;
			}

			if (value)
			{
				Settings.ExportWithImages = radioButtonOption;
			}

			OnPropertyChanged(propertyName);
		}
	}
}