using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Beginor.GisHub.DynamicSql.Data;
using Beginor.GisHub.DynamicSql.Models;
using NUnit.Framework;
using static System.Console;
using static NUnit.Framework.Assert;

namespace Beginor.GisHub.Test.DynamicSql {

    /// <summary>数据API仓储测试</summary>
    [TestFixture]
    public class DataApiRepositoryTest : BaseTest<IDataApiRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanDoSearchAsync() {
            var searchModel = new DataApiSearchModel {
                Skip = 0,
                Take = 10
            };
            var result = await Target.SearchAsync(searchModel);
            Assert.GreaterOrEqual(result.Total, 0);
            Assert.GreaterOrEqual(result.Take, result.Data.Count);
        }

        [Test]
        public void _03_CanFindTableFromSql() {
            var regex = new Regex(@"(?<=(from|join)\s+)(\w*\.\w*|\w*)");
            var sql = @"select * from public.regions r
                        left join geep.data d on d.region_id = r.id";
            var matches = regex.Matches(sql);
            IsNotEmpty(matches);
            foreach (Match match in matches) {
                WriteLine(match.Value);
            }
        }

    }

}
