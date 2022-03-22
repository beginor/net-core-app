using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Beginor.GisHub.DynamicSql.Data;
using Beginor.GisHub.DynamicSql.Models;
using NUnit.Framework;
using static System.Console;
using static NUnit.Framework.Assert;

namespace Beginor.GisHub.Test.DynamicSql; 

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

    private IList<string> FindTableNames(string sql) {
        var methodInfo = Target.GetType().GetMethod(
            "FindTableNames",
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        return methodInfo.Invoke(Target, new []{sql}) as IList<string>;
    }

    [Test]
    public void _03_CanFindTableFromSql() {
        var sql = @"select * from public.regions r
                        left join geep.data d on d.region_id = r.id";
        var matches = FindTableNames(sql);
        IsNotEmpty(matches);
        WriteLine(matches.Count);
        foreach (var match in matches) {
            WriteLine(match);
        }
    }

    [Test]
    public void _04_CanFindTableNameFromSqlWithSubselect() {
        var sql = @"select t.region_code FROM (
                  select region_code, region_name, '工业源' as ent_type, ent_cnt_industry as ""count"" FROM abi.wp2017_cnt_region_all
                  union
                  select region_code, region_name, '农业源' as ent_type, ent_cnt_livestock as ""count"" FROM abi.wp2017_cnt_region_all
                ) t";
        var matches = FindTableNames(sql);
        IsNotEmpty(matches);
        WriteLine(matches.Count);
        foreach (var match in matches) {
            WriteLine(match);
        }
    }

}