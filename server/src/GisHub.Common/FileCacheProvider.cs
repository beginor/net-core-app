using System;
using System.IO;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.Common;

public class FileCacheProvider : IFileCacheProvider {

    private CommonOption option;
    private IWebHostEnvironment host;
    private ILogger<FileCacheProvider> logger;

    public FileCacheProvider(CommonOption option, IWebHostEnvironment host, ILogger<FileCacheProvider> logger) {
        this.option = option ?? throw new ArgumentNullException(nameof(option));
        this.host = host ?? throw new ArgumentNullException(nameof(host));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<byte[]> GetContentAsync(string path, int duration) {
        if (path.IsNullOrEmpty()) {
            throw new ArgumentNullException(nameof(path));
        }
        if (Path.IsPathRooted(path)) {
            throw new InvalidOperationException("Rooted path is not supported!");
        }
        var fileInfo = GetFileInfo(path);
        if (!fileInfo.Exists) {
            return null;
        }
        if ((DateTime.Now - fileInfo.LastWriteTime).TotalSeconds > duration) {
            DeleteObsoletedFile(fileInfo);
            return null;
        }
        await using var stream = new MemoryStream();
        await using var fileStream = fileInfo.OpenRead();
        await fileStream.CopyToAsync(stream);
        var buffer = stream.GetBuffer();
        return buffer;
    }

    private void DeleteObsoletedFile(FileInfo fileInfo) {
        try {
            logger.LogInformation(
                $"Delete obsoleted cache file {fileInfo.FullName}"
            );
            fileInfo.Delete();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete file {fileInfo.FullName} !");
        }
    }

    public FileInfo GetFileInfo(string path) {
        if (path.IsNullOrEmpty()) {
            throw new ArgumentNullException(nameof(path));
        }
        if (Path.IsPathRooted(path)) {
            throw new InvalidOperationException("Rooted path is not supported!");
        }
        var fullPath = Path.Combine(host.ContentRootPath, option.Cache.Directory, path!);
        var fileInfo = new FileInfo(fullPath);
        return fileInfo;
    }

    public async Task SetContentAsync(string path, byte[] content) {
        if (path.IsNullOrEmpty()) {
            throw new ArgumentNullException(nameof(path));
        }
        if (Path.IsPathRooted(path)) {
            throw new InvalidOperationException("Rooted path is not supported!");
        }
        var fileInfo = GetFileInfo(path);
        if (fileInfo.Exists) {
            DeleteObsoletedFile(fileInfo);
        }
        fileInfo = GetFileInfo(path);
        if (fileInfo.Exists) {
            throw new InvalidOperationException($"Can not override old file {fileInfo.FullName}");
        }
        if (!Directory.Exists(fileInfo.DirectoryName)) {
            Directory.CreateDirectory(fileInfo.DirectoryName!);
        }
        await using var fs = fileInfo.OpenWrite();
        await fs.WriteAsync(content);
        await fs.FlushAsync();
        fs.Close();
    }

    public Task DeleteAsync(string path) {
        var fileInfo = GetFileInfo(path);
        if (fileInfo.Exists) {
            fileInfo.Delete();
        }
        else if (Directory.Exists(fileInfo.FullName)) {
            Directory.Delete(fileInfo.FullName, true);
        }
        return Task.CompletedTask;
    }

}
