using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DataServices.PostGIS;

namespace Beginor.GisHub.Test.DataSources.PostGIS {

    public class PostGISConnectionProviderTest : BaseTest {

        private static PostGISConnectionProvider target;
        private static IConnectionRepository repository;

        [OneTimeSetUp]
        public static void Initialize() {
            var mock = new Mock<IConnectionRepository>();
            mock.Setup(repo => repo.GetByIdAsync(1L, default))
                .Returns(Task.FromResult(new ConnectionModel {
                    Id = "1",
                    DatabaseType = "postgis",
                    ServerAddress = "127.0.0.1",
                    ServerPort = 5432,
                    DatabaseName = "gishub",
                    Username = "postgres",
                    Password = "postgis_11",
                    Timeout = 30
                }));
            repository = mock.Object;
            target = new PostGISConnectionProvider();
        }

        [Test]
        public void _00_CanMockObjects() {
            Assert.IsNotNull(target);
        }

        [Test]
        public async Task _01_CanBuildConnectionString() {
            var model = await repository.GetByIdAsync(1L);
            var connStr = target.BuildConnectionString(model);
            Console.WriteLine(connStr);
            Assert.IsNotEmpty(connStr);
        }

        [Test]
        public async Task _02_CanGetSchemes() {
            var model = await repository.GetByIdAsync(1L);
            var schemes = await target.GetSchemasAsync(model);
            Assert.IsNotEmpty(schemes);
            foreach (var scheme in schemes) {
                Console.WriteLine(scheme);
            }
        }

        [Test]
        public async Task _03_CanGetTables() {
            var model = await repository.GetByIdAsync(1L);
            var tables = await target.GetTablesAsync(model, "public");
            Assert.IsNotEmpty(tables);
            foreach (var table in tables) {
                Console.WriteLine(table.ToJson());
                Assert.IsNotNull(table.TableName);
            }
        }

        [Test]
        public async Task _04_CanGetColumns() {
            var model = await repository.GetByIdAsync(1L);
            var columns = await target.GetColumnsAsync(model, "public", "connections");
            Assert.IsNotEmpty(columns);
            foreach (var col in columns) {
                Console.WriteLine(col.ToJson());
                Assert.IsNotNull(col.ColumnName);
            }
        }
    }

}
