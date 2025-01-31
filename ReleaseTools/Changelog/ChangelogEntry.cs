namespace ReleaseTools.Changelog
{
    public class ChangelogEntry
    {
        public ChangelogEntry(string version, string[] changes)
        {
            Version = version;
            Changes = changes;
        }

        public string Version { get; }
        public string[] Changes { get; }
    }
}