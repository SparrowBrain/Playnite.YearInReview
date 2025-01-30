using Playnite.SDK.Controls;
using System.Collections.ObjectModel;
using System.Windows;

namespace YearInReview.Validation.MVVM
{
	/// <summary>
	/// Interaction logic for ValidationErrorsView.xaml
	/// </summary>
	public partial class ValidationErrorsView : PluginUserControl
	{
		public ValidationErrorsView()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty ValidationErrorsProperty =
			DependencyProperty.Register(
				nameof(ValidationErrors),
				typeof(ObservableCollection<ValidationErrorViewModel>),
				typeof(ValidationErrorsView),
				new PropertyMetadata(new ObservableCollection<ValidationErrorViewModel>()));

		public ObservableCollection<ValidationErrorViewModel> ValidationErrors
		{
			get => (ObservableCollection<ValidationErrorViewModel>)GetValue(ValidationErrorsProperty);
			set => SetValue(ValidationErrorsProperty, value);
		}
	}
}