using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseTools.Changelog
{
    public class ChangelogReader
    {
        public async Task<string> Read(string file)
        {
            using (var fileStream = new FileStream(file, FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            {
                var builder = new StringBuilder();
                while (true)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }

                    builder.AppendLine(line);
                }

                return builder.ToString();
            }
        }
    }
}