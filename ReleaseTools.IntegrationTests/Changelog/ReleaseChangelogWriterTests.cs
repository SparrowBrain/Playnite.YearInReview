using System;
using System.IO;
using AutoFixture.Xunit2;
using ReleaseTools.Changelog;
using Xunit;

namespace ReleaseTools.IntegrationTests.Changelog
{
    public class ReleaseChangelogWriterTests : IDisposable
    {
        private readonly string _file;

        public ReleaseChangelogWriterTests()
        {
            _file = Path.GetTempFileName();
        }

        [Theory, AutoData]
        public void Write_CreatesChangelogFile(
            string version,
            ReleaseChangelogWriter sut)
        {
            // Arrange
            var changes = new[]
            {
                "- Change 1",
                "- Change 2",
                "- Fix 1",
            };
            var changelogEntry = new ChangelogEntry(version, changes);
            var expectedText = File.ReadAllText(@"Changelog\TestData\expected_release_changelog.md");

            // Act
            sut.Write(_file, changelogEntry);

            // Assert
            var actualText = File.ReadAllText(_file);
            Assert.Equal(expectedText, actualText);
        }

        public void Dispose()
        {
            if (File.Exists(_file))
            {
                File.Delete(_file);
            }
        }
    }
}