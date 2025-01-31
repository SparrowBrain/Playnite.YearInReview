using System.IO;
using System.Text;

namespace ReleaseTools.ExtensionYaml
{
    public class ExtensionYamlUpdater
    {
        public void Update(string extensionYaml, string version)
        {
            var yaml = File.ReadAllLines(extensionYaml);
            var newYaml = new StringBuilder();
            foreach (var line in yaml)
            {
                if (line.StartsWith("Version: "))
                {
                    newYaml.AppendLine($"Version: {version}");
                    continue;
                }

                newYaml.AppendLine(line);
            }

            if (newYaml.ToString().EndsWith("\r\n"))
            {
                newYaml.Remove(newYaml.Length - 2, 2);
            }

            File.WriteAllText(extensionYaml, newYaml.ToString(), Encoding.UTF8);
        }
    }
}