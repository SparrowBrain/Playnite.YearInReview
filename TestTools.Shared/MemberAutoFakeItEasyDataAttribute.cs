using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Xunit;
using Xunit.Sdk;

// Taken from https://github.com/AutoFixture/AutoFixture/issues/1142
namespace TestTools.Shared
{
	public class MemberAutoFakeItEasyDataAttribute : DataAttribute
	{
		private readonly Lazy<IFixture> _fixture;
		private readonly MemberDataAttribute _memberDataAttribute;

		public MemberAutoFakeItEasyDataAttribute(string memberName, params object[] parameters)
			: this(memberName, parameters, () =>
			{
				var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization() { ConfigureMembers = true });
				return fixture;
			})
		{
		}

		protected MemberAutoFakeItEasyDataAttribute(string memberName, object[] parameters, Func<IFixture> fixtureFactory)
		{
			if (fixtureFactory == null)
			{
				throw new ArgumentNullException(nameof(fixtureFactory));
			}

			_memberDataAttribute = new MemberDataAttribute(memberName, parameters);
			_fixture = new Lazy<IFixture>(fixtureFactory, LazyThreadSafetyMode.PublicationOnly);
		}

		public override IEnumerable<object[]> GetData(MethodInfo testMethod)
		{
			if (testMethod == null)
			{
				throw new ArgumentNullException(nameof(testMethod));
			}

			var memberData = _memberDataAttribute.GetData(testMethod);

			using (var enumerator = memberData.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					var specimens = GetSpecimens(testMethod.GetParameters(), enumerator.Current.Length).ToArray();

					do
					{
						yield return enumerator.Current.Concat(specimens).ToArray();
					} while (enumerator.MoveNext());
				}
			}
		}

		private IEnumerable<object> GetSpecimens(IEnumerable<ParameterInfo> parameters, int skip)
		{
			foreach (var parameter in parameters.Skip(skip))
			{
				CustomizeFixture(parameter);

				yield return Resolve(parameter);
			}
		}

		private void CustomizeFixture(ParameterInfo p)
		{
			var customizeAttributes = p.GetCustomAttributes()
				.OfType<IParameterCustomizationSource>()
				.OrderBy(x => x, new CustomizeAttributeComparer());

			foreach (var ca in customizeAttributes)
			{
				var c = ca.GetCustomization(p);
				_fixture.Value.Customize(c);
			}
		}

		private object Resolve(ParameterInfo p)
		{
			var context = new SpecimenContext(_fixture.Value);

			return context.Resolve(p);
		}

		private class CustomizeAttributeComparer : Comparer<IParameterCustomizationSource>
		{
			public override int Compare(IParameterCustomizationSource x, IParameterCustomizationSource y)
			{
				var xFrozen = x is FrozenAttribute;
				var yFrozen = y is FrozenAttribute;

				if (xFrozen && !yFrozen)
				{
					return 1;
				}

				if (yFrozen && !xFrozen)
				{
					return -1;
				}

				return 0;
			}
		}
	}
}