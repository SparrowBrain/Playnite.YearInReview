using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using ReleaseTools.Changelog;
using ReleaseTools.ExtensionYaml;
using ReleaseTools.GitHubTools;
using ReleaseTools.InstallerManifestYaml;
using ReleaseTools.Package;

namespace ReleaseTools
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Assuming we're calling from /ci path
            var pathToSolution = "..";

            var msBuild = @"""C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe""";
            var testRunner = $@"""{pathToSolution}\packages\xunit.runner.console.2.4.2\tools\net462\xunit.console.exe""";
            var toolbox = @"""C:\Users\Qwx\AppData\Local\Playnite\Toolbox.exe""";

            await EnsureGitHubAuthentication();

            var extensionPackageNameGuesser = new ExtensionPackageNameGuesser();

            var releaseArtifactsDir = CleanUpReleaseArtifacts(pathToSolution);

            var changeEntry = await ParseChangelog(pathToSolution);

            var releaseChangelog = CreateReleaseChangelog(releaseArtifactsDir, changeEntry);
            var releasePackage = CreateReleasePackagePath(extensionPackageNameGuesser, changeEntry, releaseArtifactsDir);

            UpdateExtensionManifest(pathToSolution, changeEntry);

            var addonBuildDir = Build(msBuild, pathToSolution);
            RunTests(testRunner, pathToSolution);
            PackageExtension(toolbox, addonBuildDir, releaseArtifactsDir);

            CommitAndPush($@"v{changeEntry.Version} extension.yaml update");

            CreateRelease(changeEntry, releaseChangelog, releasePackage);

            UpdateInstallerManifest(pathToSolution, extensionPackageNameGuesser, changeEntry);
            //CommitAndPush($@"v{changeEntry.Version} installer-manifest.yaml update");
        }

        private static async Task EnsureGitHubAuthentication()
        {
            var authStatusParser = new AuthStatusParser();
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = "gh";
            p.StartInfo.Arguments = "auth status";
            p.Start();
			var output = await p.StandardOutput.ReadToEndAsync();
			p.WaitForExit();

            if (!authStatusParser.IsUserLoggedIn(output))
            {
                throw new AuthenticationException(
                    "User not logged in to GitHub via CLI. Either run `gh auth login` or setup an environment variable `GITHUB_TOKEN`. More info: https://cli.github.com/manual/.");
            }
        }

        private static string CleanUpReleaseArtifacts(string pathToSolution)
        {
            var releaseArtifactsDir = Path.Combine(pathToSolution, @"ci\release_artifacts");
            if (Directory.Exists(releaseArtifactsDir))
            {
                Directory.Delete(releaseArtifactsDir, true);
            }

            if (!Directory.Exists(releaseArtifactsDir))
            {
                Directory.CreateDirectory(releaseArtifactsDir);
            }

            return releaseArtifactsDir;
        }

        private static async Task<ChangelogEntry> ParseChangelog(string pathToSolution)
        {
            var changelogReader = new ChangelogReader();
            var changelogParser = new ChangelogParser();

            var changes = await changelogReader.Read(Path.Combine(pathToSolution, @"ci\Changelog.txt"));
            var changeEntry = changelogParser.Parse(changes);
            return changeEntry;
        }

        private static string CreateReleaseChangelog(string releaseArtifactsDir, ChangelogEntry changeEntry)
        {
            var releaseChangelogWriter = new ReleaseChangelogWriter();
            var releaseChangelog = Path.Combine(releaseArtifactsDir, "changelog.md");
            releaseChangelogWriter.Write(releaseChangelog, changeEntry);
            return releaseChangelog;
        }

        private static string CreateReleasePackagePath(ExtensionPackageNameGuesser extensionPackageNameGuesser, ChangelogEntry changeEntry, string releaseArtifactsDir)
        {
            var packageName = extensionPackageNameGuesser.GetName(changeEntry.Version);
            var releasePackage = Path.Combine(releaseArtifactsDir, packageName);
            return releasePackage;
        }

        private static void UpdateExtensionManifest(string pathToSolution, ChangelogEntry changeEntry)
        {
            var extensionYamlUpdater = new ExtensionYamlUpdater();
            extensionYamlUpdater.Update(Path.Combine(pathToSolution, @"YearInReview\extension.yaml"), changeEntry.Version);
        }

        private static string Build(string msBuild, string pathToSolution)
        {
            RunCommand(msBuild, $"{Path.Combine(pathToSolution, "YearInReview.sln")} -property:Configuration=Release");
            return Path.Combine(pathToSolution, "YearInReview", "bin", "Release");
        }

        private static void RunTests(string testRunner, string pathToSolution)
        {
            var testTasks = new List<Tuple<string, string>>();
            testTasks.AddRange(from projectDirectory in Directory.GetDirectories(pathToSolution)
                               select Path.Combine(projectDirectory, "bin", "Release")
                into buildDir
                               where Directory.Exists(buildDir)
                               from testAssembly in Directory.EnumerateFiles(buildDir, "*Tests.dll")
                               select CreateCommand(testRunner, testAssembly));
            foreach (var task in testTasks)
            {
                RunCommand(task.Item1, task.Item2);
            }
        }

        private static void PackageExtension(string toolbox, string addonBuildDir, string releaseArtifactsDir)
        {
            RunCommand(toolbox, $@"pack {addonBuildDir} {releaseArtifactsDir}");
        }

        private static void CommitAndPush(string message)
        {
            RunCommand("git", $@"commit -am ""{message}""");
            RunCommand("git", $@"push origin main");
        }

        private static void CreateRelease(ChangelogEntry changeEntry, string releaseChangelog, string releasePackage)
        {
            RunCommand("gh", $@"release create v{changeEntry.Version} -t ""Release v{changeEntry.Version}"" -F {releaseChangelog} {releasePackage}");
        }

        private static void UpdateInstallerManifest(string pathToSolution, ExtensionPackageNameGuesser extensionPackageNameGuesser, ChangelogEntry changeEntry)
        {
            var installerManifestUpdater = new InstallerManifestUpdater();
            var playniteSdkVersionParser = new PlayniteSdkVersionParser(Path.Combine(pathToSolution, @"YearInReview\YearInReview.csproj"));
            var dateTimeProvider = new DateTimeProvider();
            var installerManifestEntryGenerator = new InstallerManifestEntryGenerator(playniteSdkVersionParser, dateTimeProvider, extensionPackageNameGuesser);

            var manifestEntry = installerManifestEntryGenerator.Generate(changeEntry);
            installerManifestUpdater.Update(Path.Combine(pathToSolution, @"ci\installer_manifest.yaml"), manifestEntry);
        }

        private static void RunCommand(string command, string arguments)
        {
            Console.WriteLine($"{command} {arguments}");
            var info = Process.Start(command, arguments);
            info.WaitForExit();
            if (info.ExitCode != 0)
            {
                throw new Exception($"Failed with exit code: {info.ExitCode}");
            }
        }

        private static Tuple<string, string> CreateCommand(string command, string arguments)
        {
            return new Tuple<string, string>(command, arguments);
        }
    }
}