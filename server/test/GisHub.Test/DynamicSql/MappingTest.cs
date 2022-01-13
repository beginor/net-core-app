using System;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DynamicSql.Data;
using Beginor.GisHub.DynamicSql.Models;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace Beginor.GisHub.Test.DynamicSql;

[TestFixture]
public class MappingTest : BaseTest<AutoMapper.IMapper> {

    [Test]
    public void _00_CanResolveTarget() {
        IsNotNull(Target);
    }

    [Test]
    public void _01_CanMapModelToApi() {
        var model = new DataApiModel {
            Id = "",
            Name = "",
            Description = "",
            DataSource = new StringIdNameEntity(),
            Statement = "<Statement Id=\"\">\n  <!-- 动态 SQL 标签请参考 https://smartsql.net/config/sqlmap.html#statement-筛选子标签 -->\n</Statement>",
            Columns = Array.Empty<DataServiceFieldModel>(),
            Parameters = Array.Empty<DataApiParameterModel>(),
            Roles = Array.Empty<string>()
        };
        var api = Target.Map<DataApi>(model);
        IsNotNull(api);
        IsNotNull(api.Statement);
    }

}
