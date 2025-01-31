using System;
using System.IO;
using AutoFixture.Xunit2;
using ReleaseTools.InstallerManifestYaml;
using Xunit;

namespace ReleaseTools.IntegrationTests.InstallerManifestYaml
{
    public class InstallerManifestUpdaterTests : IDisposable
    {
        private const string BeforeManifest = @"InstallerManifestYaml\TestData\before_installer_manifest.yaml";
        private const string AfterManifest = @"InstallerManifestYaml\TestData\after_installer_manifest.yaml";
        private readonly string _installerManifest;

        public InstallerManifestUpdaterTests()
        {
            _installerManifest = Path.GetTempFileName();
            File.Delete(_installerManifest);
            File.Copy(BeforeManifest, _installerManifest);
        }

        [Theory, AutoData]
        public void Update_AddsExpectedEntry(
            InstallerManifestUpdater sut)
        {
            // Arrange
            var expected = File.ReadAllText(AfterManifest);
            var manifestEntry = @"  - Version: 2.3.4
    RequiredApiVersion: 9.8.7
    ReleaseDate: 2020-03-24
    PackageUrl: https://github.com/SparrowBrain/Playnite.YearInReview/releases/download/v2.3.4/SparrowBrain_YearInReview_2_3_4.pext
    Changelog:
      - Change 1
      - Change 2
      - Fix
";

            // Act
            sut.Update(_installerManifest, manifestEntry);

            // Assert
            var actual = File.ReadAllText(_installerManifest);
            Assert.Equal(expected, actual);
        }

        public void Dispose()
        {
            if (File.Exists(_installerManifest))
            {
                File.Delete(_installerManifest);
            }
        }
    }
}