using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using ReleaseTools.Changelog;
using Xunit;

namespace ReleaseTools.IntegrationTests.Changelog
{
    public class ChangelogReaderTests : IDisposable
    {
        private readonly string _file;

        public ChangelogReaderTests()
        {
            _file = Path.GetTempFileName();
            var contents = @"v1.2.3
- Change is here
- Fix!

v1.2.2
- Something old";

            File.WriteAllText(_file, contents);
        }

        [Theory, AutoData]
        public async Task Read_ReturnsChangelogContentsUntilFirstEmptyLine(
            ChangelogReader sut)
        {
            // Act
            var result = await sut.Read(_file);

            // Assert
            Assert.Equal("v1.2.3\r\n- Change is here\r\n- Fix!\r\n", result);
        }

        public void Dispose()
        {
            File.Delete(_file);
        }
    }
}