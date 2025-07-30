using AutoFixture.Xunit2;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using TestTools.Shared;
using Xunit;
using YearInReview.Settings;
using YearInReview.Settings.Old;

namespace YearInReview.UnitTests.Settings
{
	public class SettingsMigratorTests
	{
		[Theory, AutoFakeItEasyData]
		public void LoadAndMigrateToNewest_ThrowsException_WhenCalledWithNonConfiguredVersion(
			SettingsMigrator sut)
		{
			// Act
			var act = new Action(() => sut.LoadAndMigrateToNewest(int.MaxValue));

			// Assert
			Assert.ThrowsAny<ArgumentException>(act);
		}

		[Theory, MemberAutoFakeItEasyData(nameof(GetAllOldSettingsVersions))]
		public void LoadAndMigrateToNewest_MigratesAllExistingNonCurrentSettingsVersionsToNewest(
			int version,
			SettingsMigrator sut)
		{
			// Act
			var result = sut.LoadAndMigrateToNewest(version);

			// Assert
			Assert.Equal(YearInReviewSettings.CurrentVersion, result.Version);
		}

		[Theory]
		[InlineAutoFakeItEasyData(0)]
		[InlineAutoFakeItEasyData(+2)]
		[InlineAutoFakeItEasyData(-1)]
		public void LoadAndMigrateToNewest_ThrowsException_WhenSettingsMigratesToNonIncrementedVersion(
			int versionIncrement,
			[Frozen] IPluginSettingsPersistence pluginSettingsPersistence,
			SettingsV0Fake settingsV0Fake,
			SettingsMigrator sut)
		{
			// Arrange
			A.CallTo(() => pluginSettingsPersistence.LoadPluginSettings<SettingsV0>()).Returns(settingsV0Fake);
			settingsV0Fake.SetupVersionItMigratesTo(settingsV0Fake.Version + versionIncrement);

			// Act
			var act = new Action(() => sut.LoadAndMigrateToNewest(settingsV0Fake.Version));

			// Assert
			Assert.ThrowsAny<Exception>(act);
		}

		public static IEnumerable<object[]> GetAllOldSettingsVersions()
		{
			var type = typeof(IVersionedSettings);
			var types = AppDomain.CurrentDomain.GetAssemblies()
				.Where(x => x.FullName.StartsWith("YearInReview"))
				.SelectMany(s => s.GetTypes())
				.Where(x => x.IsClass)
				.Where(p => type.IsAssignableFrom(p))
				.Where(x => x != typeof(VersionedSettings))
				.Where(x => x != typeof(SettingsV0Fake));

			var allOldSettingsVersions = types.Select(x =>
			{
				var ctor = x.GetConstructor(new Type[] { });
				object instance = ctor.Invoke(new object[] { });
				return new object[] { (instance as IVersionedSettings).Version };
			}).Where(x => (int)x[0] != YearInReviewSettings.CurrentVersion);

			return allOldSettingsVersions;
		}

		public class SettingsV0Fake : SettingsV0
		{
			public SettingsV0Fake(int version)
			{
				Version = version;
			}

			private int _newVersion;

			public void SetupVersionItMigratesTo(int newVersion)
			{
				_newVersion = newVersion;
			}

			public override IVersionedSettings Migrate()
			{
				return new SettingsV0Fake(_newVersion);
			}
		}
	}
}