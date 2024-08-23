using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SkiaSharp;

namespace Beginor.NetCoreApp.Common;

public static class FileHelper {

    private static readonly int thumbSize = 480;
    private static readonly int thumbQuality = 90;

    private static readonly string[] videoExts = { ".mp4", ".webm", ".mkv", ".flv" };
    private static readonly string[] imageExts = { ".png", ".jpg", ".webp", ".gif" };

    public static Thumbnail GetThumbnail(string filePath, IHostEnvironment hostEnv) {
        if (!File.Exists(filePath)) {
            throw new FileNotFoundException("File does not exists!", filePath);
        }
        var fileType = GetFileType(filePath);
        if (fileType == FileType.Image) {
            return GetImageThumbnail(filePath);
        }
        if (fileType == FileType.Video) {
            return GetVideoThumbnail(filePath, hostEnv);
        }
        return Thumbnail.Empty;
    }

    private static Thumbnail GetImageThumbnail(string imagePath) {
        using var inputCodec = SKCodec.Create(imagePath);
        if (inputCodec == null) {
            throw new FileLoadException($"Unable to load image: {imagePath}.");
        }

        var imageWidth = inputCodec.Info.Width;
        var imageHeight = inputCodec.Info.Height;
        var thumbWidth = imageWidth;
        var thumbHeight = imageHeight;

        SKBitmap thumbBitmap;

        if (imageWidth <= thumbSize && imageHeight <= thumbSize) {
            thumbBitmap = SKBitmap.Decode(inputCodec);
        }
        else {
            if (imageWidth >= imageHeight) {
                thumbWidth = thumbSize;
                thumbHeight = (int)(imageHeight * ((float)thumbWidth / imageWidth));
            }
            else {
                thumbHeight = thumbSize;
                thumbWidth = (int)(imageWidth * ((float)thumbHeight / imageHeight));
            }
            var supportedScale = inputCodec.GetScaledDimensions((float)thumbWidth / imageWidth);
            var nearestSize = new SKImageInfo(supportedScale.Width, supportedScale.Height);
            var destBitmap = SKBitmap.Decode(inputCodec, nearestSize);
            thumbBitmap = destBitmap.Resize(
                new SKImageInfo(thumbWidth, thumbHeight),
                SKFilterQuality.High
            );
        }

        using var thumbData = thumbBitmap.Encode(SKEncodedImageFormat.Jpeg, thumbQuality);
        using var memStream = new MemoryStream();
        thumbData.SaveTo(memStream);
        return new Thumbnail(imageWidth, imageHeight, thumbWidth, thumbHeight, memStream.GetBuffer());
    }

    public static FileType GetFileType(string fileName) {
        if (videoExts.Any(ext => fileName.EndsWith(ext))) {
            return FileType.Video;
        }
        if (imageExts.Any(ext => fileName.EndsWith(ext))) {
            return FileType.Image;
        }
        return FileType.Other;
    }

    private static Thumbnail GetVideoThumbnail(string videoPath, IHostEnvironment env) {
        var ffmpegExe = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffmpeg.exe" : "ffmpeg";
        string platform;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            platform = "win";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            platform = "linux";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
            platform = "osx";
        }
        else {
            // throw new PlatformNotSupportedException("ffmpeg only support win, osx and linux");
            return Thumbnail.Empty;
        }

        var ffmpeg = Path.Combine(env.ContentRootPath, "ffmpeg", platform, ffmpegExe);
        if (!File.Exists(ffmpeg)) {
            return Thumbnail.Empty;
        }

        var input = Path.Combine(env.ContentRootPath, videoPath);
        if (!File.Exists(input)) {
            throw new FileNotFoundException("Input video does not exists!", input);
        }
        var idx = videoPath.LastIndexOf('.');
        var thumbPath = videoPath[..idx] + ".thumb.jpg";
        if (File.Exists(thumbPath)) {
            File.Delete(thumbPath);
        }

        var args = new [] {
            "-i", input,
            "-ss", "00:00:01",
            "-t", "00:00:01",
            "-vf", "fps=1/1",
            "-f", "image2",
            thumbPath
        };
        var process = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = ffmpeg,
                Arguments = string.Join(' ', args),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = env.ContentRootPath,
            }
        };
        process.Start();
        process.WaitForExit();
        if (!File.Exists(thumbPath)) {
            return Thumbnail.Empty;
        }
        var thumbnail = GetImageThumbnail(thumbPath);
        File.Delete(thumbPath);
        return thumbnail;
    }

    public static async Task PartialSaveFile(long fileSize, FileInfo fileInfo, long offset, byte[] content) {
        FileStream stream;
        if (offset == 0) {
            if (fileInfo.Exists) {
                fileInfo.Delete();
            }
            stream = fileInfo.Create();
        }
        else {
            stream = fileInfo.OpenWrite();
        }
        stream.Seek(offset, SeekOrigin.Begin);
        await stream.WriteAsync(content);
        await stream.FlushAsync();
        stream.Close();
    }

}

public class Thumbnail(
    int width,
    int height,
    int thumbnailWidth,
    int thumbnailHeight,
    byte[] content
) {
    public int Width { get; init; } = width;
    public int Height { get; init; } = height;
    public int ThumbnailWidth { get; init; } = thumbnailWidth;
    public int ThumbnailImage { get; init; } = thumbnailHeight;
    public byte[] Content { get; init; } = content;

    public static Thumbnail Empty => new Thumbnail(0, 0, 0, 0, Array.Empty<byte>());
}

public enum FileType {
    Other = 0,
    Image,
    Video
}
