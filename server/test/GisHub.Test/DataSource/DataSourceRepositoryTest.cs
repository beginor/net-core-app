using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.Test.Data {

    /// <summary>数据源（数据表或视图）仓储测试</summary>
    [TestFixture]
    public class DataSourceRepositoryTest : BaseTest<IDataSourceRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanDoSearchAsync() {
            var searchModel = new DataSourceSearchModel {
                Skip = 0,
                Take = 10
            };
            var result = await Target.SearchAsync(searchModel);
            Assert.GreaterOrEqual(result.Total, 0);
            Assert.GreaterOrEqual(result.Take, result.Data.Count);
        }

    }

}
