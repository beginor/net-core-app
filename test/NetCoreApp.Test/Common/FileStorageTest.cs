using System;
using Microsoft.Extensions.FileProviders;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Common;

[TestFixture]
public class FileStorageTest {

    [Test]
    public void _00_CanGetDirInfo() {
        var currDir = Environment.CurrentDirectory;
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        Console.WriteLine($"Current Directory is: {currDir}");
        Console.WriteLine($"App Data Directory is: {appData}");
        var provider = new CompositeFileProvider(
            new PhysicalFileProvider(currDir),
            new PhysicalFileProvider(appData)
        );
        var dir1 = provider.GetDirectoryContents("/");
        Assert.IsTrue(dir1.Exists, "dir1.Exists");
        foreach (var file in dir1) {
            Console.WriteLine(file.Name);
        }

        var testDir = provider.GetDirectoryContents("test");
        Assert.IsTrue(testDir.Exists);
    }

}
