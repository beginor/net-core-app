using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Beginor.GisHub.Common;
using NUnit.Framework;

namespace Beginor.GisHub.Test.DataServices;

[TestFixture]
public class FileCacheProviderTest : BaseTest<IFileCacheProvider> {

    [Test]
    public void _00_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

    [Test]
    public async Task _01_CanGetFileInfo() {
        var path = Path.Combine("1234567", "11", "2234", "4321.mvt");
        var info = Target.GetFileInfo(path);
        Assert.IsFalse(info.Exists);
        Console.WriteLine(info.FullName);
        var content = Encoding.UTF8.GetBytes("hello,world!");
        await Target.SetContentAsync(path, content);
        info = Target.GetFileInfo(path);
        Assert.IsTrue(info.Exists);
        content = Encoding.UTF8.GetBytes("HELLO,WORLD!");
        await Target.SetContentAsync(path, content);
        await Target.DeleteAsync("1234567");
        info = Target.GetFileInfo(path);
        Assert.IsFalse(info.Exists);
    }

}
