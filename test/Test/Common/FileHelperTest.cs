using System.IO;
using NUnit.Framework;

using Beginor.NetCoreApp.Common;

namespace Beginor.NetCoreApp.Test.Common;

[TestFixture]
public class FileHelperTest {

    [Test, Ignore("already passed!")]
    public void _01_CanGetImageThumbnail() {
        var dir = "Images/";
        var inputFile = dir + "input.gif";
        var outputFile = dir + "output.jpg";
        var thumbnail = FileHelper.GetThumbnail(inputFile, null);
        Assert.That(thumbnail.Content, Is.Not.Empty);
        File.WriteAllBytes(outputFile, thumbnail.Content);
    }

}
