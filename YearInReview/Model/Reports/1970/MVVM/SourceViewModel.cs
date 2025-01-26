using System;
using System.Collections.Generic;

namespace YearInReview.Model.Reports._1970.MVVM
{
	public class SourceViewModel : ObservableObject
	{
		public SourceViewModel(ReportSourceWithTime source, int position, int maxSourcePlaytime)
		{
			Position = position;
			Id = source.Id;
			Name = source.Name;
			TimePlayed = source.TimePlayed;
			Percentage = (double)source.TimePlayed / maxSourcePlaytime;
		}

		public Guid Id { get; set; }
		
		public int Position { get; }

		public string Name { get; set; }

		public int TimePlayed { get; set; }

		public double Percentage { get; set; }

	}
}