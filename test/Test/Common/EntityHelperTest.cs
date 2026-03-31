using System;
using NUnit.Framework;
using Beginor.AppFx.Core;

using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Test.Common;

[TestFixture]
public class EntityHelperTest {

    [Test]
    public void _01_CanGetEntityMapping() {
        var mapping = EntityHelper.GetEntityMapping(typeof(AppAuditLogEntity));
        Assert.That(mapping.Schema, Is.Not.Empty);
        Console.WriteLine(mapping.ToJson());
    }

    [Test]
    public void _02_CanGenerateInsertSql() {
        var sql = EntityHelper.GenerateInsertSql(typeof(AppAuditLogEntity));
        Assert.That(sql, Is.Not.Empty);
        Console.WriteLine(sql);

        sql = EntityHelper.GenerateInsertSql(typeof(AppStorageEntity));
        Assert.That(sql, Is.Not.Empty);
        Console.WriteLine(sql);
    }

}
