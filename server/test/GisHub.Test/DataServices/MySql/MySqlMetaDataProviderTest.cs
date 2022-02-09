using System;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DataServices.MySql;
using NUnit.Framework;
using static NUnit.Framework.Assert;
using static System.Console;

namespace Beginor.GisHub.Test.DataServices.MySql;

[TestFixture]
public class MySqlMetaDataProviderTest : BaseTest<MySqlMetaDataProvider> {

    [Test]
    public void _01_CanResolveTarget() {
        IsNotNull(Target);
    }

    [Test]
    public void _02_CanBuildConnStr() {
        var dataSource = CreateTestDataSource();
        var connStr = Target.BuildConnectionString(dataSource);
        IsNotNull(connStr);
        WriteLine(connStr);
    }

    [Test]
    public async Task _03_CanGetStatus() {
        var dataSource = CreateTestDataSource();
        await Target.GetStatus(dataSource);
    }

    [Test]
    public async Task _04_CanGetTables() {
        var dataSource = CreateTestDataSource();
        var tables = await Target.GetTablesAsync(dataSource, string.Empty);
        IsNotEmpty(tables);
        foreach (var table in tables) {
            WriteLine(table.ToJson());
        }
    }

    [Test]
    public async Task _05_CanGetColumns() {
        var dataSource = CreateTestDataSource();
        var tables = await Target.GetTablesAsync(dataSource, string.Empty);
        IsNotEmpty(tables);
        foreach (var table in tables) {
            var columns = await Target.GetColumnsAsync(
                dataSource,
                dataSource.DatabaseName,
                table.Name
            );
            IsNotEmpty(columns);
            WriteLine(columns.ToJson());
            WriteLine();
        }

    }

    private DataSourceModel CreateTestDataSource() {
        return new DataSourceModel {
            Id = string.Empty,
            Name = "test mysql datasource",
            DatabaseType = "mysql",
            ServerAddress = "192.168.1.43",
            ServerPort = 3306,
            DatabaseName = "sea_moni",
            Username = "root",
            Password = "1a2b3c4D",
            UseSsl = false,
            Timeout = 90
        };
    }

}
