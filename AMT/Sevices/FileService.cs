using System.IO;
using System.Threading.Tasks;

namespace AMTools.Services
{
    public class FileService
    {
        public bool DirectoryExists(string path) => Directory.Exists(path);

        public bool FileExists(string path) => File.Exists(path);

        public async Task<string> ReadFileAsync(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        public async Task CopyFileAsync(string source, string destination)
        {
            using var sourceStream = File.OpenRead(source);
            using var destStream = File.Create(destination);
            await sourceStream.CopyToAsync(destStream);
        }
    }
}
