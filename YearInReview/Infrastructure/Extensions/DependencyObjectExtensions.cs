using System.Windows;
using System.Windows.Media;

namespace YearInReview.Infrastructure.Extensions
{
	public static class DependencyObjectExtensions
	{
		public static FrameworkElement FindChildByName(this DependencyObject parent, string childName)
		{
			if (parent == null || string.IsNullOrEmpty(childName))
			{
				return null;
			}

			var childCount = VisualTreeHelper.GetChildrenCount(parent);
			for (var i = 0; i < childCount; i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);

				if (child is FrameworkElement fe && fe.Name == childName)
				{
					return fe;
				}

				var result = FindChildByName(child, childName);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		public static T FindChildByType<T>(this DependencyObject parent) where T : DependencyObject
		{
			if (parent == null)
			{
				return null;
			}

			var childCount = VisualTreeHelper.GetChildrenCount(parent);
			for (var i = 0; i < childCount; i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);

				if (child is T typedChild)
				{
					return typedChild;
				}

				var result = FindChildByType<T>(child);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}
	}
}