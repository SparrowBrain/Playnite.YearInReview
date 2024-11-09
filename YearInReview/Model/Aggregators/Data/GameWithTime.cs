using Playnite.SDK.Models;

namespace YearInReview.Model.Aggregators.Data
{
	public class GameWithTime
	{
		public Game Game { get; set; }
		public ulong TimePlayed { get; set; }
	}
}