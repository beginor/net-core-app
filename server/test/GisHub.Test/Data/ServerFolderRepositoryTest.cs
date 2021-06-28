using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Test.Data {

    /// <summary>服务器目录仓储测试</summary>
    [TestFixture]
    public class ServerFolderRepositoryTest : BaseTest<IServerFolderRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanDoSearchAsync() {
            var searchModel = new ServerFolderSearchModel {
                Skip = 0,
                Take = 10
            };
            var result = await Target.SearchAsync(searchModel);
            Assert.GreaterOrEqual(result.Total, 0);
            Assert.GreaterOrEqual(result.Take, result.Data.Count);
        }

        [Test]
        public async Task _03_GetFolderContentAsync() {
            var faModel = new ServerFolderBrowseModel {
                Alias = "icons",
                Path = "fa",
                Filter = "*.*"
            };
            var result = await Target.GetFolderContentAsync(faModel);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Folders);
            Assert.IsEmpty(result.Files);
            var biModel = new ServerFolderBrowseModel {
                Alias = "icons",
                Path = "bi",
                Filter = "*.svg"
            };
            result = await Target.GetFolderContentAsync(biModel);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Files);
            Assert.IsEmpty(result.Folders);
        }

    }

}
