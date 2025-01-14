using Playnite.SDK;
using System;
using System.Windows.Input;

namespace YearInReview.Validation.MVVM
{
	public class ValidationErrorViewModel
	{
		public ValidationErrorViewModel(string message, Action callToAction)
		{
			Message = message;
			CallToAction = callToAction;
		}

		public string Message { get; }

		public Action CallToAction { get; }

		public bool HasCallToAction => CallToAction != null;

		public ICommand InvokeCallToAction =>
			new RelayCommand(() =>
			{
				CallToAction.Invoke();
			});
	}
}