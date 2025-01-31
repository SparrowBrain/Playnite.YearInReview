using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ReleaseTools.InstallerManifestYaml
{
    public class PlayniteSdkVersionParser : IPlayniteSdkVersionParser
    {
        private readonly string _projectFile;

        public PlayniteSdkVersionParser(string projectFile)
        {
            _projectFile = projectFile;
        }

        public string GetVersion()
        {
            var regex = new Regex(@"<Reference Include=""Playnite\.SDK, Version=(?<version>\d+\.\d+\.\d+)\.0");

            var contents = File.ReadAllText(_projectFile);
            var match = regex.Match(contents);
            if (match.Success)
            {
                return match.Groups["version"].Value;
            }

            throw new Exception($"Failed to parse Playnite SDK version from file {_projectFile}");
        }
    }
}