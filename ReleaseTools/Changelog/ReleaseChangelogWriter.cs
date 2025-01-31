using System.IO;
using System.Text;

namespace ReleaseTools.Changelog
{
    public class ReleaseChangelogWriter
    {
        public void Write(string file, ChangelogEntry changelogEntry)
        {
            var stringBuilder = new StringBuilder();
            foreach (var line in changelogEntry.Changes)
            {
                stringBuilder.AppendLine(line);
            }

            if (stringBuilder.ToString().EndsWith("\r\n"))
            {
                stringBuilder.Remove(stringBuilder.Length - 2, 2);
            }

            File.WriteAllText(file, stringBuilder.ToString());
        }
    }
}