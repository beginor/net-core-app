using System;
using System.IO;
using SkiaSharp;
using NUnit.Framework;

using Beginor.NetCoreApp.Common;

namespace Beginor.NetCoreApp.Test.Common;

[TestFixture]
public class CaptchaTest {

    [Test]
    public void _01_CanGenerateCode() {
        var options = new CaptchaOptions {
            CodeLength = 6,
            FontFamily = "",
            ForegroundColor = "#FF0000",
            EnableDistortion = true,
            EnableNoisePoints = true,
            NoisePointColor = "#00FF00"
        };
        var target = new CaptchaGenerator(options);
        var captcha = target.Generate();
        Assert.That(captcha.Code, Is.Not.Empty);
        Console.WriteLine(captcha.Code);
        File.WriteAllBytes("captcha-01.jpeg", captcha.Data);

        captcha = target.Generate();
        Assert.That(captcha.Code, Is.Not.Empty);
        Console.WriteLine(captcha.Code);
        File.WriteAllBytes("captcha-02.jpeg", captcha.Data);
    }

    [Test]
    public void _02_CanParseFormat() {
        var format = Enum.Parse<SKEncodedImageFormat>("jpeg", true);
        Assert.That(format, Is.EqualTo(SKEncodedImageFormat.Jpeg));
    }

}
