using System;
using System.Linq;
using AutoFixture.Xunit2;
using ReleaseTools.Changelog;
using Xunit;

namespace ReleaseTools.UnitTests.Changelog
{
    public class ChangelogParserTests
    {
        [Theory]
        [InlineAutoData("")]
        [InlineAutoData((string)null)]
        public void Parse_ThrowsException_When_EmptyInput(
            string input,
            ChangelogParser sut)
        {
            // Act
            var act = new Action(() => sut.Parse(input));

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Theory]
        [InlineAutoData("Good Day!")]
        [InlineAutoData("- Changed things for better")]
        [InlineAutoData("vA.B.C")]
        [InlineAutoData("v1.2.3.4")]
        [InlineAutoData("v1.2.3 and something more!")]
        [InlineAutoData("This is v1.2.3")]
        public void Parse_ThrowsException_When_FirstLineIsNotAValidVersion(
            string input,
            ChangelogParser sut)
        {
            // Act
            var act = new Action(() => sut.Parse(input));

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Theory, AutoData]
        public void Parse_ReturnsVersion_When_InputHasAVersion(
            ChangelogParser sut)
        {
            // Arrange
            var input = @"v1.2.3
- Change 1
";

            // Act
            var result = sut.Parse(input);

            // Assert
            Assert.Equal("1.2.3", result.Version);
        }

        [Theory, AutoData]
        public void Parse_ReturnsChanges_When_InputHasChangeItems(
            ChangelogParser sut)
        {
            // Arrange
            var input = @"v1.2.3
- Change 1
- Change 2
";

            // Act
            var result = sut.Parse(input);

            // Assert
            Assert.Equal(2, result.Changes.Length);
            Assert.Equal("- Change 1", result.Changes.First());
            Assert.Equal("- Change 2", result.Changes.Last());
        }
    }
}