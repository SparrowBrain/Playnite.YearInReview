﻿using System;
using System.Windows.Markup;

namespace YearInReview.Infrastructure.Converters
{
	public abstract class BaseConverter : MarkupExtension
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}