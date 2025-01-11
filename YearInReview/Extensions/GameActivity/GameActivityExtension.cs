using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace YearInReview.Extensions.GameActivity
{
	public class GameActivityExtension : IGameActivityExtension
	{
		public static Guid ExtensionId = Guid.Parse("afbb1a0d-04a1-4d0c-9afa-c6e42ca855b4");
		private readonly ILogger _logger = LogManager.GetLogger(nameof(GameActivityExtension));
		private readonly string _activityPath;

		public GameActivityExtension(string extensionsDataPath)
		{
			var dataPath = Path.Combine(extensionsDataPath, ExtensionId.ToString(), "GameActivity");
			_activityPath = dataPath;
		}

		public async Task<IReadOnlyCollection<Activity>> GetActivityForGames(IEnumerable<Game> games)
		{
			try
			{
				if (!GameActivityPathExists())
				{
					_logger.Warn("GameActivity path does not exist!");
					return new List<Activity>();
				}

				var files = Directory.GetFiles(_activityPath);
				var validFiles = files
					.Where(path =>
						Guid.TryParse(Path.GetFileNameWithoutExtension(path), out var id) &&
						games.Any(x => x.Id == id));
				var deserializedFiles = validFiles
					.Select(DeserializeActivityFile);

				var activities = await Task.WhenAll(deserializedFiles);

				var withSessions = activities
					.Where(activity => (activity?.Items?.Count() ?? 0) > 0);

				var activitiesOfInterest = new List<Activity>(withSessions);
				_logger.Info($"{activitiesOfInterest.Count} games with activity found");
				return activitiesOfInterest;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Failure reading game activity files");
				return new List<Activity>();
			}
		}

		private bool GameActivityPathExists()
		{
			return !string.IsNullOrEmpty(_activityPath) && Directory.Exists(_activityPath);
		}

		private async Task<Activity> DeserializeActivityFile(string file)
		{
			try
			{
				using (var reader = new StreamReader(file))
				{
					var json = await reader.ReadToEndAsync();
					return JsonConvert.DeserializeObject<Activity>(json);
				}
			}
			catch { return null; }
		}
	}
}