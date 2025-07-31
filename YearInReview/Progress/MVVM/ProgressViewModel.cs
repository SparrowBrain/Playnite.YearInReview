using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace YearInReview.Progress.MVVM
{
	public class ProgressViewModel : ObservableObject, IDisposable
	{
		private readonly IPlayniteAPI _api;
		private Window _window;

		public ProgressViewModel(IPlayniteAPI api)
		{
			_api = api;
		}

		public void SetWindow(Window window)
		{
			_window = window;
		}

		public ICommand Hide => new RelayCommand(CloseWindow);

		public void Dispose()
		{
			CloseWindow();
		}

		private void CloseWindow()
		{
			_api.MainView.UIDispatcher.Invoke(() => _window?.Close());
		}
	}
}