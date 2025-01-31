namespace ReleaseTools.GitHubTools
{
    public class AuthStatusParser
    {
        public bool IsUserLoggedIn(string output)
        {
            return output.Contains("Logged in to github.com account");
        }
    }
}