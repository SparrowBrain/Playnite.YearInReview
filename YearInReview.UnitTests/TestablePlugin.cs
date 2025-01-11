using Playnite.SDK;
using Playnite.SDK.Plugins;
using System;

namespace YearInReview.UnitTests
{
	public class TestablePlugin : Plugin
	{
		private Guid _id;

		public TestablePlugin(IPlayniteAPI playniteApi) : base(playniteApi)
		{
		}

		public override Guid Id => _id;

		public void SetId(Guid id)
		{
			_id = id;
		}
	}
}