using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReleaseTools.Changelog
{
    public class ChangelogParser
    {
        public ChangelogEntry Parse(string changelogEntry)
        {
            if (string.IsNullOrEmpty(changelogEntry))
            {
                throw new ArgumentNullException(nameof(changelogEntry));
            }

            var regex = new Regex(@"^v(?<version>\d+\.\d+\.\d+)\r\n((?<change>- [^\r\n]+)\r\n)+$", RegexOptions.Multiline);
            var match = regex.Match(changelogEntry);
            if (!match.Success)
            {
                throw new ArgumentException($"Invalid changelog entry passed to parser: {changelogEntry}");
            }

            var version = match.Groups["version"].Value;
            var changes = match.Groups["change"].Captures.Cast<Capture>().Select(x => x.Value).ToArray();
            return new ChangelogEntry(version, changes);
        }
    }
}