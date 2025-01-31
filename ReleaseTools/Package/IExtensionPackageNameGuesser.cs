namespace ReleaseTools.Package
{
    public interface IExtensionPackageNameGuesser
    {
        string GetName(string version);
    }
}