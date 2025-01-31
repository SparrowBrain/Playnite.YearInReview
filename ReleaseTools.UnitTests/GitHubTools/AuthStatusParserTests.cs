using AutoFixture.Xunit2;
using ReleaseTools.GitHubTools;
using Xunit;

namespace ReleaseTools.UnitTests.GitHubTools
{
	public class AuthStatusParserTests
	{
		[Theory, AutoData]
		public void IsUserLoggedIn_ReturnsTrue_When_UserIsLoggedIn(AuthStatusParser sut)
		{
			// Arrange
			var output = "\u001b[0;1;39mgithub.com\u001b[0m\r\n  \u001b[0;32mŌ£ō\u001b[0m Logged in to github.com account \u001b[0;1;39mSparrowBrain\u001b[0m (keyring)\r\n  \u001b[0;32mŌ£ō\u001b[0m Git operations for github.com configured to use \u001b[0;1;39mhttps\u001b[0m protocol.\r\n  \u001b[0;32mŌ£ō\u001b[0m Token: gho_************************************\r\n  \u001b[0;32mŌ£ō\u001b[0m Token scopes: gist, read:org, repo\r\n\r\n";

			// Act
			var result = sut.IsUserLoggedIn(output);

			// Assert
			Assert.True(result);
		}

		[Theory, AutoData]
		public void IsUserLoggedIn_ReturnsFalse_When_UserIsLoggedOut(AuthStatusParser sut)
		{
			// Arrange
			var output = "You are not logged into any GitHub hosts. Run \u001b[0;1;39mgh auth login\u001b[0m to authenticate.\r\n\r\n";

			// Act
			var result = sut.IsUserLoggedIn(output);

			// Assert
			Assert.False(result);
		}
	}
}