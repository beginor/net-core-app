using System.IO;
using System.Threading.Tasks;

namespace Beginor.GisHub.Common;

public interface IFileCacheProvider {

    Task SetContentAsync(string path, byte[] content);

    Task<byte[]> GetContentAsync(string path, int duration);

    FileInfo GetFileInfo(string path);

    DirectoryInfo GetDirectoryInfo(string path);

    Task DeleteAsync(string path);

}
