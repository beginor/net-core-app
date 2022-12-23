using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Beginor.AppFx.Core;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DynamicSql.Data;
using Beginor.GisHub.DynamicSql.Models;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
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

    [Test]
    public void _05_CanGetById() {
        var id = 1646193841190030916L;
        var session = ServiceProvider.GetService<ISession>();
        var entity = session.Query<DataApi>().FirstOrDefault(x => x.Id == id);
        IsNotNull(entity.Statement);
        // var model = Target.GetById(id);
        // Assert.IsNotNull(model.Statement);
    }

    [Test]
    public void _06_CanQueryBaseResource() {
        var session = ServiceProvider.GetService<ISession>();
        var query = session.Query<BaseResource>()
            .Where(x => x.Type == "data_api")
            .Select(x => new BaseResource {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Category = new Category {
                    Id = x.Category.Id,
                    Name = x.Category.Name
                },
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            });
        var data = query.ToList();
        IsNotEmpty(data);
        WriteLine(data.Count);
    }

    [Test]
    public void _07_CanSaveDataApi() {
        var session = ServiceProvider.GetService<ISession>();
        IsNotNull(session);
        var user = session.Query<AppUser>().First(u => u.UserName == "admin");
        IsNotNull(user);
        var category = session.Query<Category>().First();
        IsNotNull(category);
        var datasource = session.Query<DataSource>().First();
        var statement = new XmlDocument();
        statement.LoadXml("<Statement></Statement>");
        var api = new DataApi {
            Name = "test-api",
            Description = "test create api",
            Category = category,
            Creator = user,
            CreatedAt = DateTime.Now,
            Updater = user,
            UpdatedAt = DateTime.Now,
            Tags = new []{"test"},
            Roles = new []{"roles"},
            DataSource = datasource,
            WriteData = false,
            Statement = statement,
            Parameters = new DataApiParameter[0],
            Columns = new DataServiceField[0],
            GeometryColumn = "geom",
            IdColumn = "fid"
        };
        AreEqual("data_api", api.Type);
        session.Save(api);
        session.Flush();
        session.Clear();
        Greater(api.Id, 0);
        var entity = session.Query<DataApi>().FirstOrDefault(x => x.Id == api.Id);
        IsNotNull(entity);
        session.Delete(entity);
        session.Flush();
        session.Clear();
    }

}
