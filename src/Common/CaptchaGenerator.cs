using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Beginor.NetCoreApp.Common;

public interface ICaptchaGenerator {
    Task<CaptchaResult> GenerateAsync();
    Task<string> GenerateCodeAsync();
    Task<byte[]> GenerateImageAsync(string code);
}

public class CaptchaGenerator(CaptchaOptions options) : ICaptchaGenerator {

    private static readonly string allowedChars = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ1234567890";

    public async Task<CaptchaResult> GenerateAsync() {
        var code = await GenerateCodeAsync();
        var data = await GenerateImageAsync(code);
        var captcha = new CaptchaResult {
            Code = code,
            Image = data,
            ContentType = "image/jpeg"
        };
        return captcha;
    }

    public Task<string> GenerateCodeAsync() {
        var random = new Random();
        var builder = new StringBuilder();
        for (var i = 0; i < options.CodeLength; i++) {
            var idx = random.Next(allowedChars.Length);
            builder.Append(allowedChars[idx]);
        }
        return Task.FromResult(builder.ToString());
    }

    public Task<byte[]> GenerateImageAsync(string code) {
        using var image = GenerateImage(code);
        var imageFormat = SKEncodedImageFormat.Jpeg;
        if (Enum.TryParse<SKEncodedImageFormat>(options.ImageFormat, true, out var format)) {
            imageFormat = format;
        }
        var data = image.Encode(imageFormat, options.ImageQuality).ToArray();
        return Task.FromResult(data);
    }

    private (int newX, int newY) Distortion(int oldX, int oldY, double distortionLevel) {
        var newX = (int)(oldX + distortionLevel * Math.Sin(Math.PI * oldY / 64.0));
        var newY = (int)(oldY + distortionLevel * Math.Cos(Math.PI * oldX / 64.0));
        if (newX < 0 || newX >= options.ImageWidth) {
            newX = 0;
        }
        if (newY < 0 || newY >= options.ImageHeight) {
            newY = 0;
        }

        return (newX, newY);
    }

    private IEnumerable<(int x, int y)> GenerateNoiseMap() {
        var random = new Random();
        var noisePointCount = (int)(options.ImageWidth * options.ImageHeight * options.NoisePointsPercent);
        return Enumerable.Range(0, noisePointCount)
            .Select(_ => ( random.Next(options.ImageWidth), random.Next(options.ImageHeight)));
    }

    private SKImage GenerateImage(string code) {
        var imageInfo = new SKImageInfo(
            options.ImageWidth,
            options.ImageHeight,
            SKImageInfo.PlatformColorType,
            SKAlphaType.Premul
        );
        using var plainSurface = SKSurface.Create(imageInfo);
        var plainCanvas = plainSurface.Canvas;
        plainCanvas.Clear(SKColor.Parse(options.BackgroundColor));

        using var paint = new SKPaint();
        paint.Typeface = SKTypeface.FromFamilyName(options.FontFamily);
        paint.TextSize = options.FontSize;
        paint.Color = SKColor.Parse(options.ForegroundColor);
        paint.IsAntialias = true;

        var xToDraw = (options.ImageWidth - paint.MeasureText(code)) / 2;
        var yToDraw = (options.ImageHeight - options.FontSize) / 2 + options.FontSize;
        plainCanvas.DrawText(code, xToDraw, yToDraw, paint);
        plainCanvas.Flush();

        if (options is { EnableDistortion: false, EnableNoisePoints: false }) {
            return plainSurface.Snapshot();
        }

        using var captchaSurface = SKSurface.Create(imageInfo);
        var captchaCanvas = captchaSurface.Canvas;

        var distortionLevel = 0.0;
        if (options.EnableDistortion) {
            var random = new Random();
            distortionLevel = options.MinDistortion + (options.MaxDistortion - options.MinDistortion) * random.NextDouble();
            if (random.NextDouble() > 0.5) {
                distortionLevel *= -1;
            }
        }
        var plainPixmap = plainSurface.PeekPixels();
        for (var x = 0; x < options.ImageWidth; x++) {
            for (var y = 0; y < options.ImageHeight; y++) {
                var (newX, newY) = Distortion(x, y, distortionLevel);
                var originalPixel = plainPixmap.GetPixelColor(newX, newY);
                captchaCanvas.DrawPoint(x, y, originalPixel);
            }
        }

        if (options.EnableNoisePoints) {
            var noisePointColor = SKColor.Parse(options.NoisePointColor);
            foreach (var noisePoint in GenerateNoiseMap()) {
                captchaCanvas.DrawPoint(noisePoint.x, noisePoint.y, noisePointColor);
            }
        }

        captchaCanvas.Flush();
        return captchaSurface.Snapshot();
    }

}

public class CaptchaResult {
    public string Code { get; init; } = string.Empty;
    public byte[] Image { get; init; } = [];
    public string ContentType { get; init; } = string.Empty;
}

public class CaptchaOptions {
    public int CodeLength { get; set; } = 6;
    public string ForegroundColor { get; set; } = "#808080"; //
    public string BackgroundColor { get; set; } = "#F5DEB3";
    public string NoisePointColor { get; set; } = "#D3D3D3";
    public int ImageWidth { get; set; } = 120;
    public int ImageHeight { get; set; } = 48;
    public string ImageFormat { get; set; } = "jpeg";
    public int ImageQuality { get; set; } = 90;
    public string? FontFamily { get; set; } = "";
    public int FontSize { get; set; } = 20;
    public bool EnableDistortion { get; set; } = true;
    public double MinDistortion { get; set; } = 5.0;
    public double MaxDistortion { get; set; } = 15.0;
    public bool EnableNoisePoints { get; set; } = true;
    public double NoisePointsPercent { get; set; }  = 0.05;
}
