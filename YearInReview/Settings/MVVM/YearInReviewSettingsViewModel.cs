using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using YearInReview.Model;

namespace YearInReview.Settings.MVVM
{
	public class YearInReviewSettingsViewModel : ObservableObject, ISettings, ISettingsViewModel
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

		public event Action CurrentYearReportGenerationChanged;

		public YearInReviewSettings Settings
		{
			get => _settings;
			set
			{
				_settings = value;
				OnPropertyChanged();

				InitRememberedChoiceSetting(
					value.ExportWithImages,
					ask => ExportWithImagesAsk = ask,
					never => ExportWithImagesNever = never,
					always => ExportWithImagesAlways = always);

				InitExportFormatSetting(
					value.ExportFormat,
					ask => ExportFormatAsk = ask,
					png => ExportAsPng = png,
					json => ExportAsJson = json);
			}
		}

		public bool ExportFormatAsk
		{
			get => Settings.ExportFormat == ExportFormat.Ask;
			set => SetExportFormatRadioButtonValue(nameof(ExportFormatAsk), ExportFormat.Ask, value);
		}

		public bool ExportAsPng
		{
			get => Settings.ExportFormat == ExportFormat.Png;
			set => SetExportFormatRadioButtonValue(nameof(ExportAsPng), ExportFormat.Png, value);
		}

		public bool ExportAsJson
		{
			get => Settings.ExportFormat == ExportFormat.Json;
			set => SetExportFormatRadioButtonValue(nameof(ExportAsJson), ExportFormat.Json, value);
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

			if (_editingClone.ShowCurrentYearReport != Settings.ShowCurrentYearReport)
			{
				OnCurrentYearReportGenerationChanged();
			}
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

		protected virtual void OnCurrentYearReportGenerationChanged()
		{
			CurrentYearReportGenerationChanged?.Invoke();
		}

		private void InitRememberedChoiceSetting(RememberedChoice choice, Action<bool> assignAsk, Action<bool> assignNever, Action<bool> assignAlways)
		{
			switch (choice)
			{
				case RememberedChoice.Ask:
					assignAsk.Invoke(true);
					assignNever.Invoke(false);
					assignAlways.Invoke(false);
					break;

				case RememberedChoice.Never:
					assignAsk.Invoke(false);
					assignNever.Invoke(true);
					assignAlways.Invoke(false);

					break;

				case RememberedChoice.Always:
					assignAsk.Invoke(false);
					assignNever.Invoke(false);
					assignAlways.Invoke(true);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(choice));
			}
		}

		private void InitExportFormatSetting(ExportFormat choice, Action<bool> assignAsk, Action<bool> assignPng, Action<bool> assignJson)
		{
			switch (choice)
			{
				case ExportFormat.Ask:
					assignAsk.Invoke(true);
					assignPng.Invoke(false);
					assignJson.Invoke(false);
					break;

				case ExportFormat.Png:
					assignAsk.Invoke(false);
					assignPng.Invoke(true);
					assignJson.Invoke(false);

					break;

				case ExportFormat.Json:
					assignAsk.Invoke(false);
					assignPng.Invoke(false);
					assignJson.Invoke(true);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(choice));
			}
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

		private void SetExportFormatRadioButtonValue(
			string propertyName,
			ExportFormat radioButtonOption,
			bool value)
		{
			if (Settings.ExportFormat == radioButtonOption == value)
			{
				return;
			}

			if (value)
			{
				Settings.ExportFormat = radioButtonOption;
			}

			OnPropertyChanged(propertyName);
		}
	}
}