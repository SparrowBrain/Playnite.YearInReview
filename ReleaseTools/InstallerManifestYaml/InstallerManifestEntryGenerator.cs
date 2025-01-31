using System.Text;
using ReleaseTools.Changelog;
using ReleaseTools.Package;

namespace ReleaseTools.InstallerManifestYaml
{
    public class InstallerManifestEntryGenerator
    {
        private readonly IPlayniteSdkVersionParser _playniteSdkVersionParser;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IExtensionPackageNameGuesser _extensionPackageNameGuesser;

        public InstallerManifestEntryGenerator(IPlayniteSdkVersionParser playniteSdkVersionParser, IDateTimeProvider dateTimeProvider, IExtensionPackageNameGuesser extensionPackageNameGuesser)
        {
            _playniteSdkVersionParser = playniteSdkVersionParser;
            _dateTimeProvider = dateTimeProvider;
            _extensionPackageNameGuesser = extensionPackageNameGuesser;
        }

        public string Generate(ChangelogEntry changelogEntry)
        {
            var apiVersion = _playniteSdkVersionParser.GetVersion();
            var packageName = _extensionPackageNameGuesser.GetName(changelogEntry.Version);

            var builder = new StringBuilder();
            builder.AppendLine($"  - Version: {changelogEntry.Version}");
            builder.AppendLine($"    RequiredApiVersion: {apiVersion}");
            builder.AppendLine($"    ReleaseDate: {_dateTimeProvider.Now:yyyy-MM-dd}");
            builder.AppendLine($"    PackageUrl: https://github.com/SparrowBrain/Playnite.YearInReview/releases/download/v{changelogEntry.Version}/{packageName}");
            builder.AppendLine($"    Changelog:");
            foreach (var change in changelogEntry.Changes)
            {
                builder.AppendLine($"      {change}");
            }

            return builder.ToString();
        }
    }
}