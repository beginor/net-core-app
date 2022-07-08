using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.Gmap.Cache; 

public class CacheProvider : ICacheProvider {
    
    private CacheOptions options;
    private ILogger<CacheProvider> logger;
    private const string FileExt = ".png";
    
    public CacheProvider(CacheOptions options, ILogger<CacheProvider> logger) {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SaveTileAsync(string serviceId, int level, int row, int col, byte[] content) {
        var tilePath = GetTilePath(serviceId, level, row, col);
        try {
            var fileInfo = new FileInfo(tilePath);
            if (!Directory.Exists(fileInfo.DirectoryName)) {
                Directory.CreateDirectory(fileInfo.DirectoryName!);
            }
            if (fileInfo.Exists) {
                fileInfo.Delete();
            }
            await using var fs = fileInfo.OpenWrite();
            await fs.WriteAsync(content);
            await fs.FlushAsync();
            fs.Close();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save tile cache to file {tilePath} !");
        }
    }

    public Task<byte[]> GetTileAsync(string serviceId, int level, int row, int col) {
        var tilePath = GetTilePath(serviceId, level, row, col);
        try {
            if (!File.Exists(tilePath)) {
                return Task.FromResult(Array.Empty<byte>());
            }
            return File.ReadAllBytesAsync(tilePath);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not read tile cache from file {tilePath}");
            return Task.FromResult(Array.Empty<byte>());
        }
    }
    
    private string GetTilePath(string serviceId, int level, int row, int col) {
        return Path.Combine(options.Directory, serviceId, level.ToString(), row.ToString(), col.ToString()) + FileExt;
    }

}
