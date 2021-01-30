using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DataServices.PostGIS;

namespace Beginor.GisHub.Test.DataServices.PostGIS {

    [TestFixture]
    public class PostGISDataSourceReaderTest : BaseTest<PostGISDataSourceReader> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanGetColumns() {
            var dataSourceId = 1607411884905030151;
            var columns = await Target.GetColumnsAsync(dataSourceId);
            Assert.IsNotEmpty(columns);
            foreach (var col in columns) {
                Assert.AreEqual(col.Table, "books");
            }
        }

        [Test]
        public async Task _03_CanCountData() {
            var dataSourceId = 1607411884905030151;
            var count = await Target.CountAsync(
                dataSourceId,
                new CountParam { Where = "published_in > 1950" }
            );
            Assert.Greater(count, 0);
        }

        [Test]
        public async Task _04_CanReadDistinctData() {
            var dataSourceId = 1607411884905030151;
            var data = await Target.ReadDistinctDataAsync(
                dataSourceId,
                new DistinctParam {
                    Select = "author_id, language_id",
                    Where = "author_id > 0",
                    OrderBy = "author_id"
                }
            );
            Assert.IsNotEmpty(data);
        }

        [Test]
        public async Task _05_CanReadData() {
            var id = 1609887224871030614;
            var data = await Target.ReadDataAsync(
                id,
                new ReadDataParam {
                    Select = "objectid, id, name, cnty_id, cnty_name, shape",
                    Where = "cnty_id = '440105'",
                    GroupBy = string.Empty,
                    OrderBy = "id",
                    Skip = 0,
                    Take = 20
                }
            );
            Assert.IsNotEmpty(data);
        }

    }

}
