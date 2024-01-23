using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Beginor.NetCoreApp.Common;

public static class FileHelper {

    private static readonly int thumbSize = 480;

    public static Thumbnail GetThumbnail(string imageFilePath) {
        if (!File.Exists(imageFilePath)) {
            throw new FileNotFoundException("File does not exists!", imageFilePath);
        }
        using var memStream = new MemoryStream();
        using var image = Image.Load(imageFilePath);
        var imageWidth = image.Width;
        var imageHeight = image.Height;
        var thumbWidth = image.Width;
        var thumbHeight = image.Height;
        if (imageWidth <= thumbSize && imageHeight <= thumbSize) {
            image.Save(memStream, new JpegEncoder());
        }
        else {
            if (imageWidth >= imageHeight) {
                thumbWidth = thumbSize;
                thumbHeight = imageHeight * (thumbWidth / imageWidth);
            }
            else {
                thumbHeight = thumbSize;
                thumbWidth = imageWidth * (thumbHeight / imageHeight);
            }
            image.Mutate(x => x.Resize(thumbWidth, thumbHeight));
            image.Save(memStream, new JpegEncoder());
        }
        return new Thumbnail(imageWidth, imageHeight, thumbWidth, thumbHeight, memStream.GetBuffer());
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
}
