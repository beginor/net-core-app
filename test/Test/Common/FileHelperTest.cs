using System.IO;
using NUnit.Framework;

using Beginor.NetCoreApp.Common;

namespace Beginor.NetCoreApp.Test.Common;

[TestFixture]
public class FileHelperTest {

    [Test]
    public void _01_CanGetImageThumbnail() {
        var images = "Images";
        var inputFile = $"{images}/input.jpg";
        var outputFile = $"{images}/input-thumbnail.jpg";
        var thumbnail = FileHelper.GetThumbnail(inputFile, null);
        Assert.That(thumbnail.Content, Is.Not.Empty);
        if (File.Exists(outputFile)) {
            File.Delete(outputFile);
        }
        File.WriteAllBytes(outputFile, thumbnail.Content);
    }

}
