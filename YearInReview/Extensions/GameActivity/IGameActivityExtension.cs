using System.Collections.Generic;
using System.Threading.Tasks;
using Playnite.SDK.Models;

namespace YearInReview.Extensions.GameActivity
{
	public interface IGameActivityExtension
	{
		Task<IReadOnlyCollection<Activity>> GetActivityForGames(IEnumerable<Game> games);
	}
}