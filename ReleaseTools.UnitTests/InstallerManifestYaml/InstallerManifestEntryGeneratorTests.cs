using System;
using AutoFixture.Xunit2;
using FakeItEasy;
using ReleaseTools.Changelog;
using ReleaseTools.InstallerManifestYaml;
using ReleaseTools.Package;
using TestTools.Shared;
using Xunit;

namespace ReleaseTools.UnitTests.InstallerManifestYaml
{
    public class InstallerManifestEntryGeneratorTests
    {
        [Theory, AutoFakeItEasyData]
        public void Generate_CreatesEntry(
            [Frozen] IDateTimeProvider dateTimeProvider,
            [Frozen] IPlayniteSdkVersionParser playniteSdkVersionParser,
            [Frozen] IExtensionPackageNameGuesser extensionPackageNameGuesser,
            InstallerManifestEntryGenerator sut)
        {
            // Arrange
            A.CallTo(()=> dateTimeProvider.Now).Returns(DateTime.Parse("2020-03-24"));
            A.CallTo(()=> playniteSdkVersionParser.GetVersion()).Returns("9.8.7");
            A.CallTo(()=> extensionPackageNameGuesser.GetName( "2.3.4"))
                .Returns("SparrowBrain_YearInReview_2_3_4.pext");
            var changeEntry = new ChangelogEntry("2.3.4", new[] { "- Change 1", "- Change 22", "- Fix important!" });
            var expected = @"  - Version: 2.3.4
    RequiredApiVersion: 9.8.7
    ReleaseDate: 2020-03-24
    PackageUrl: https://github.com/SparrowBrain/Playnite.YearInReview/releases/download/v2.3.4/SparrowBrain_YearInReview_2_3_4.pext
    Changelog:
      - Change 1
      - Change 22
      - Fix important!
";

            // Act
            var result = sut.Generate(changeEntry);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}