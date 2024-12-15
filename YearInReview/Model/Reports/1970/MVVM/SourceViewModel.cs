using System;
using System.Collections.Generic;

namespace YearInReview.Model.Reports._1970.MVVM
{
	public class SourceViewModel : ObservableObject
	{
		public SourceViewModel(ReportSourceWithTime source, int position, int maxWidth, int maxSourcePlaytime)
		{
			Position = position;
			Id = source.Id;
			Name = source.Name;
			TimePlayed = source.TimePlayed;
			Percentage = (float)source.TimePlayed / maxSourcePlaytime;
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