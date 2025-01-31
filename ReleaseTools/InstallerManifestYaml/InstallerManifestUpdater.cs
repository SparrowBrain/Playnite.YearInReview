using System;
using System.IO;

namespace ReleaseTools.InstallerManifestYaml
{
    public class InstallerManifestUpdater
    {
        private const string PackagesString = "Packages:\r\n";

        public void Update(string manifestFile, string manifestEntry)
        {
            var manifest = File.ReadAllText(manifestFile);
            var packagesStart = manifest.IndexOf(PackagesString, StringComparison.InvariantCulture);
            var newManifest = manifest.Insert(packagesStart + PackagesString.Length, manifestEntry);
            File.WriteAllText(manifestFile, newManifest);
        }
    }
}