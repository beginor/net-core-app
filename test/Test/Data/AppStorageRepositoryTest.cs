using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Test.Data;

/// <summary>应用存储仓储测试</summary>
[TestFixture]
public class AppStorageRepositoryTest : BaseTest<IAppStorageRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(Target, Is.Not.Null);
    }

    [Test]
    public async Task _02_CanDoSearchAsync() {
        var searchModel = new AppStorageSearchModel {
            Skip = 0,
            Take = 10
        };
        var result = await Target.SearchAsync(searchModel);
        Assert.That(result.Total, Is.GreaterThanOrEqualTo(0));
        Assert.That(result.Take, Is.GreaterThanOrEqualTo(result.Data.Count));
    }

    [Test]
    public async Task _03_GetFolderContentAsync() {
        var faModel = new AppStorageBrowseModel {
            Alias = "icons",
            Path = "fa",
            Filter = "*.*"
        };
        var result = await Target.GetFolderContentAsync(faModel);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Folders, Is.Not.Empty);
        Assert.That(result.Files, Is.Empty);
        var biModel = new AppStorageBrowseModel {
            Alias = "icons",
            Path = "bi",
            Filter = "*.svg"
        };
        result = await Target.GetFolderContentAsync(biModel);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Files, Is.Not.Empty);
        Assert.That(result.Folders, Is.Empty);
    }

}
