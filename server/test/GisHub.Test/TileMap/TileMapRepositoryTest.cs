using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.TileMap.Models;
using Beginor.GisHub.TileMap.Data;

namespace Beginor.GisHub.Test.TileMap {

    /// <summary>切片地图仓储测试</summary>
    [TestFixture]
    public class TileMapRepositoryTest : BaseTest<ITileMapRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanDoSearchAsync() {
            var searchModel = new TileMapSearchModel {
                Skip = 0,
                Take = 10
            };
            var result = await Target.SearchAsync(searchModel);
            Assert.GreaterOrEqual(result.Total, 0);
            Assert.GreaterOrEqual(result.Take, result.Data.Count);
        }

    }

}
