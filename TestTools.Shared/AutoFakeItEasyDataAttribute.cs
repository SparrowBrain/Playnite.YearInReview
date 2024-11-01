using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit2;

namespace TestTools.Shared
{
	public class AutoFakeItEasyDataAttribute : AutoDataAttribute
	{
		public AutoFakeItEasyDataAttribute()
			: base(() => new Fixture()
				.Customize(new AutoFakeItEasyCustomization()))
		{
		}
	}
}