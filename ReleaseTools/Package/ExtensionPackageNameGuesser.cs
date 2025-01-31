namespace ReleaseTools.Package
{
    public class ExtensionPackageNameGuesser : IExtensionPackageNameGuesser
    {
        public string GetName(string version)
        {
            var versionNumbers = version.Split('.');
            return $"SparrowBrain_YearInReview_{versionNumbers[0]}_{versionNumbers[1]}_{versionNumbers[2]}.pext";
        }
    }
}