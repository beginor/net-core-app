using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.AppFx.Core;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.Test.DataServices {

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

        [Test]
        public void _03_CanMapDataSource() {
            var entity = new DataSource {
                Id = 1,
                Connection = new Connection {
                    Id = 2,
                    Name = "Test"
                }
            };
            var mapper = ServiceProvider.GetService<AutoMapper.IMapper>();
            var model = mapper.Map<DataSourceModel>(entity);
            Assert.AreEqual(entity.Connection.Id.ToString(), model.Connection.Id);
            var entity2 = mapper.Map<Beginor.GisHub.DataServices.Data.DataSource>(model);
            Assert.IsNotNull(entity2.Connection);
            Assert.AreEqual(entity2.Connection.Id.ToString(), model.Connection.Id);
        }

        [Test]
        public async Task _03_CanGetCacheItem() {
            var id = 1607411721075030142;
            var cacheItem = await Target.GetCacheItemByIdAsync(id);
            Assert.IsNotNull(cacheItem);
            Console.WriteLine(cacheItem.ToJson());
        }

    }

}
