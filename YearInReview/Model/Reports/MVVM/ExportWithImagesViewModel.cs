using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using YearInReview.Settings.MVVM;

namespace YearInReview.Model.Reports.MVVM
{
	public class ExportWithImagesViewModel : ObservableObject
	{
		private readonly YearInReviewSettingsViewModel _settingsViewModel;
		private readonly Action<bool> _exportAsJsonAction;
		private readonly Action _exportAsPngAction;

		private Window _window;
		private bool _isExportWithImages;
		private bool _rememberChoice;
		private bool _exportAsPng;
		private bool _exportAsJson;

		public ExportWithImagesViewModel(YearInReviewSettingsViewModel settingsViewModel, Action<bool> exportAsJsonAction, Action exportAsPngAction)
		{
			_settingsViewModel = settingsViewModel;
			_exportAsJsonAction = exportAsJsonAction;
			_exportAsPngAction = exportAsPngAction;
			if (settingsViewModel.Settings.ExportFormat == ExportFormat.Json)
			{
				ExportAsJson = true;
			}
			else
			{
				ExportAsPng = true;
			}
		}

		public void AssociateWindow(Window window)
		{
			_window = window;
		}

		public bool ExportAsPng
		{
			get => _exportAsPng;
			set => SetValue(ref _exportAsPng, value);
		}

		public bool ExportAsJson
		{
			get => _exportAsJson;
			set => SetValue(ref _exportAsJson, value);
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
				_settingsViewModel.Settings.ExportWithImages = IsExportWithImages ? RememberedChoice.Always : RememberedChoice.Never;
				_settingsViewModel.Settings.ExportFormat = ExportAsPng
					? ExportFormat.Png
					: ExportAsJson
						? ExportFormat.Json
						: ExportFormat.Ask;
				_settingsViewModel.EndEdit();
			}

			if (ExportAsJson)
			{
				_exportAsJsonAction(IsExportWithImages);
			}
			else if (ExportAsPng)
			{
				_exportAsPngAction();
			}
			else
			{
				throw new InvalidOperationException("No export action defined.");
			}

			_window?.Close();
		});

		public ICommand CancelCommand => new RelayCommand(() =>
		{
			_window?.Close();
		});
	}
}