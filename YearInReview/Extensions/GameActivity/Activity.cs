using System;
using System.Collections.Generic;

namespace YearInReview.Extensions.GameActivity
{
	public class Activity : ObservableObject
	{
		private List<Session> _items = new List<Session>();
		private Guid _id = default;
		private string _name = string.Empty;

		public List<Session> Items { get => _items; set => SetValue(ref _items, value); }
		public Guid Id { get => _id; set => SetValue(ref _id, value); }
		public string Name { get => _name; set => SetValue(ref _name, value); }
	}
}