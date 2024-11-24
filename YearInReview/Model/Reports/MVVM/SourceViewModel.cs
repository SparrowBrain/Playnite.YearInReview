using System;
using System.Collections.Generic;

namespace YearInReview.Model.Reports.MVVM
{
	public class SourceViewModel : ObservableObject
	{
		public SourceViewModel(ReportSourceWithTime source, int totalSourcePlaytime)
		{
			Id = source.Id;
			Name = source.Name;
			TimePlayed = source.TimePlayed;
			Percentage = (float)source.TimePlayed / totalSourcePlaytime;
		}

		public Guid Id { get; set; }

		public string Name { get; set; }

		public int TimePlayed { get; set; }

		public float Percentage { get; set; }
	}
}