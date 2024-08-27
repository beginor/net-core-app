using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using NUnit.Framework;

using Beginor.NetCoreApp.Common;

namespace Beginor.NetCoreApp.Test.Common;

[TestFixture]
public class CaptchaResultTest : BaseTest {

    [Test]
    public async Task _01_CanGenerateCaptcha() {
        var options = new CaptchaOptions {
            CodeLength = 6,
            FontFamily = "",
            ForegroundColor = "#FF0000",
            EnableDistortion = true,
            EnableNoisePoints = true,
            NoisePointColor = "#00FF00"
        };
        var cache = ServiceProvider.GetService<IDistributedCache>();
        ICaptchaGenerator target = new CaptchaGenerator(options, cache);
        var captcha = await target.GenerateAsync();
        Assert.That(captcha.Code, Is.Not.Empty);
        Console.WriteLine(captcha.Code);
        Assert.That(captcha.ContentType, Is.EqualTo("image/jpeg"));
        await File.WriteAllBytesAsync("captcha-01.jpeg", captcha.Image);

        captcha = await target.GenerateAsync();
        Assert.That(captcha.Code, Is.Not.Empty);
        Console.WriteLine(captcha.Code);
        Assert.That(captcha.ContentType, Is.EqualTo("image/jpeg"));
        await File.WriteAllBytesAsync("captcha-02.jpeg", captcha.Image);
    }

    [Test]
    public void _02_CanParseFormat() {
        var format = Enum.Parse<SKEncodedImageFormat>("jpeg", true);
        Assert.That(format, Is.EqualTo(SKEncodedImageFormat.Jpeg));
    }

}
