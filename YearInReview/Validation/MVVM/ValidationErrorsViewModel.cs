using System.Collections.Generic;
using System.Linq;

namespace YearInReview.Validation.MVVM
{
	public class ValidationErrorsViewModel : ObservableObject
	{
		public ValidationErrorsViewModel(IReadOnlyCollection<InitValidationError> validationErrors)
		{
			ValidationErrors = validationErrors
				.Select(x => new ValidationErrorViewModel(x.Message, x.CallToAction))
				.ToList();
		}

		public IReadOnlyCollection<ValidationErrorViewModel> ValidationErrors { get; }
	}
}