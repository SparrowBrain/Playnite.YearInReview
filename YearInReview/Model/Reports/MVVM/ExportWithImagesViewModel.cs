using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using YearInReview.Settings;

namespace YearInReview.Model.Reports.MVVM
{
	public class ExportWithImagesViewModel : ObservableObject
	{
		private readonly IPluginSettingsPersistence _pluginSettingsPersistence;
		private readonly YearInReviewSettings _settings;
		private readonly Action<bool> _exportAction;

		private Window _window;
		private bool _isExportWithImages;
		private bool _rememberChoice;

		public ExportWithImagesViewModel(IPluginSettingsPersistence pluginSettingsPersistence, YearInReviewSettings settings, Action<bool> exportAction)
		{
			_pluginSettingsPersistence = pluginSettingsPersistence;
			_settings = settings;
			_exportAction = exportAction;
		}

		public void AssociateWindow(Window window)
		{
			_window = window;
		}

		public bool IsExportWithImages
		{
			get => _isExportWithImages;
			set
			{
				SetValue(ref _isExportWithImages, value);
				OnPropertyChanged(nameof(EstimatedSize));
			}
		}

		public bool RememberChoice
		{
			get => _rememberChoice;
			set => SetValue(ref _rememberChoice, value);
		}

		public string EstimatedSize => string.Format(
			ResourceProvider.GetString("LOC_YearInReview_ExportWithImages_EstimatedSize"),
			IsExportWithImages
				? ResourceProvider.GetString("LOC_YearInReview_ExportWithImages_EstimatedSize_3MB")
				: ResourceProvider.GetString("LOC_YearInReview_ExportWithImages_EstimatedSize_50kB"));

		public ICommand ExportCommand => new RelayCommand(() =>
		{
			if (RememberChoice)
			{
				_settings.ExportWithImages = IsExportWithImages ? RememberedChoice.Always : RememberedChoice.Never;
				_pluginSettingsPersistence.SavePluginSettings(_settings);
			}

			_exportAction(IsExportWithImages);
			_window?.Close();
		});

		public ICommand CancelCommand => new RelayCommand(() =>
		{
			_window?.Close();
		});
	}
}