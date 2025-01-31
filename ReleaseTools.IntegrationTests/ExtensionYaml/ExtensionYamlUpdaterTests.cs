using System;
using System.IO;
using AutoFixture.Xunit2;
using ReleaseTools.ExtensionYaml;
using Xunit;

namespace ReleaseTools.IntegrationTests.ExtensionYaml
{
    public class ExtensionYamlUpdaterTests : IDisposable
    {
        private const string ExpectedExtensionYaml = "ExtensionYaml\\TestData\\extension_after.yaml";
        private const string ExtensionYamlBefore = "ExtensionYaml\\TestData\\extension_before.yaml";
        private readonly string _installerManifest;

        public ExtensionYamlUpdaterTests()
        {
            _installerManifest = Path.GetTempFileName();
            File.Delete(_installerManifest);
            File.Copy(ExtensionYamlBefore, _installerManifest);
        }

        [Theory, AutoData]
        public void Update_ReplacesTheVersionWithTheGivenOne(
            ExtensionYamlUpdater sut)
        {
            // Arrange
            var expectedYaml = File.ReadAllText(ExpectedExtensionYaml);

            // Act
            sut.Update(_installerManifest, "1.2.3");

            // Assert
            var actual = File.ReadAllText(_installerManifest);
            Assert.Equal(expectedYaml, actual);
        }

        public void Dispose()
        {
            File.Delete(_installerManifest);
        }
    }
}