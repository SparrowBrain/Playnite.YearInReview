using Playnite.SDK.Models;
using System;
using System.Collections.Generic;

namespace YearInReview.Model.Reports.MVVM
{
	public class SourceViewModel : ObservableObject
	{
		public SourceViewModel(ReportSourceWithTime source, int position, int maxWidth, int totalSourcePlaytime)
		{
			Position = position;
			Id = source.Id;
			Name = source.Name;
			TimePlayed = source.TimePlayed;
			Percentage = (float)source.TimePlayed / totalSourcePlaytime;
			BarWidth = Percentage * maxWidth;
		}

		public Guid Id { get; set; }
		
		public int Position { get; }

		public string Name { get; set; }

		public int TimePlayed { get; set; }

		public float Percentage { get; set; }

		public float BarWidth { get; set; }
	}
}