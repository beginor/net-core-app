using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.GisHub.VectorTile.Data;
using Beginor.GisHub.VectorTile.Models;

namespace Beginor.GisHub.Test.VectorTile {

    /// <summary>矢量切片包仓储测试</summary>
    [TestFixture]
    public class VectortileRepositoryTest : BaseTest<IVectortileRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanDoSearchAsync() {
            var searchModel = new VectortileSearchModel {
                Skip = 0,
                Take = 10
            };
            var result = await Target.SearchAsync(searchModel);
            Assert.GreaterOrEqual(result.Total, 0);
            Assert.GreaterOrEqual(result.Take, result.Data.Count);
        }

    }

}
