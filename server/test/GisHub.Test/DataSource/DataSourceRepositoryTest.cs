using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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

        [Test]
        public void _03_CanMapDataSource() {
            var entity = new DataSource {
                Id = 1,
                ConnectionString = new ConnectionString {
                    Id = 2,
                    Name = "Test"
                }
            };
            var mapper = ServiceProvider.GetService<AutoMapper.IMapper>();
            var model = mapper.Map<DataSourceModel>(entity);
            Assert.AreEqual(entity.ConnectionString.Id.ToString(), model.ConnectionString.Id);
            var entity2 = mapper.Map<DataSource>(model);
            Assert.IsNotNull(entity2.ConnectionString);
            Assert.AreEqual(entity2.ConnectionString.Id.ToString(), model.ConnectionString.Id);
        }

    }

}
