using System.IO;
using System.Threading.Tasks;

namespace Beginor.GisHub.Common;

public interface IFileCacheProvider {

    Task SetContent(string path, byte[] content);

    Task<byte[]> GetContentAsync(string path, int duration);

    FileInfo GetFileInfo(string path);

}
