namespace YearInReview.Model.Reports._1970.MVVM
{
	public class FriendPlaytimeLeaderboardViewModel
	{
		private const int MaxBarWidth = 500;

		public FriendPlaytimeLeaderboardViewModel(int position, string name, int playtime, int maxPlaytime)
		{
			Position = position;
			Name = name;
			Playtime = playtime;

			BarWidth = (float)playtime * MaxBarWidth / maxPlaytime;
		}

		public int Position { get; }

		public string Name { get; }

		public int Playtime { get; }

		public float BarWidth { get; set; }
	}
}