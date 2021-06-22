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
            var result = await Target.GetFolderContentAsync("icons", "fa", "*.svg");
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Folders);
            Assert.IsEmpty(result.Files);
            result = await Target.GetFolderContentAsync("icons", "bi/", "*.svg");
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Files);
            Assert.IsEmpty(result.Folders);
        }

    }

}
