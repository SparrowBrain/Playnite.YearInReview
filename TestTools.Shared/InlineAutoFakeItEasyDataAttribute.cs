using AutoFixture.Xunit2;

namespace TestTools.Shared
{
	public class InlineAutoFakeItEasyDataAttribute : InlineAutoDataAttribute
	{
		public InlineAutoFakeItEasyDataAttribute(params object[] values) : base(new AutoFakeItEasyDataAttribute(), values)
		{
		}
	}
}