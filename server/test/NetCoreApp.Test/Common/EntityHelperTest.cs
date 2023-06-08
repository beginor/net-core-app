using System;
using NUnit.Framework;
using Beginor.AppFx.Core;

using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Test;

[TestFixture]
public class EntityHelperTest {

    [Test]
    public void _01_CanGetEntityMapping() {
        var mapping = EntityHelper.GetEntityMapping(typeof(AppAuditLog));
        Assert.IsNotEmpty(mapping.Schema);
        Console.WriteLine(mapping.ToJson());
    }

    [Test]
    public void _02_CanGenerateInsertSql() {
        var sql = EntityHelper.GenerateInsertSql(typeof(AppAuditLog));
        Assert.IsNotEmpty(sql);
        Console.WriteLine(sql);

        sql = EntityHelper.GenerateInsertSql(typeof(AppStorage));
        Assert.IsNotEmpty(sql);
        Console.WriteLine(sql);
    }

}
