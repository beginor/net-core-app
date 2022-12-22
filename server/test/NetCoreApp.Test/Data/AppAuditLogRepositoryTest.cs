using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Repositories;

namespace Beginor.NetCoreApp.Test.Data;

[TestFixture]
public class AppAuditLogRepositoryTest : BaseTest<IAppAuditLogRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

    [Test]
    public async Task _02_CanStatTrafic() {
        var endDate = DateTime.Today;
        var startDate = endDate.AddDays(-29);
        var result = await Target.StatTrafficAsync(startDate, endDate);
        var trafics = result.Data;
        Assert.IsNotNull(trafics);
        Assert.IsNotEmpty(trafics);
        Console.WriteLine(trafics.Count);
        foreach (var t in trafics) {
            Console.WriteLine(t.ToJson());
        }
    }

    [Test]
    public async Task _03_CanStatStatus() {
        var endDate = DateTime.Now;
        var startDate = endDate.AddDays(-29);
        var result = await Target.StatStatusAsync(startDate, endDate);
        Assert.IsNotNull(result.Data);
        Console.WriteLine(result.Data.Count);
        foreach (var t in result.Data) {
            Console.WriteLine(t.ToJson());
        }
    }

    [Test]
    public async Task _04_CanStatDuration() {
        var endDate = DateTime.Now;
        var startDate = endDate.AddDays(-29);
        var result = await Target.StatDurationAsync(startDate, endDate);
        Assert.IsNotNull(result.Data);
        Console.WriteLine(result.Data.Count);
        foreach (var t in result.Data) {
            Console.WriteLine(t.ToJson());
        }
    }

    [Test]
    public async Task _05_CanStatUser() {
        var endDate = DateTime.Now;
        var startDate = endDate.AddDays(-29);
        var result = await Target.StatUserAsync(startDate, endDate);
        Assert.IsNotNull(result.Data);
        Console.WriteLine(result.Data.Count);
        foreach (var t in result.Data) {
            Console.WriteLine(t.ToJson());
        }
    }

    [Test]
    public async Task _06_CanStatIp() {
        var endDate = DateTime.Now;
        var startDate = endDate.AddDays(-29);
        var result = await Target.StatIpAsync(startDate, endDate);
        Assert.IsNotNull(result.Data);
        Console.WriteLine(result.Data.Count);
        foreach (var t in result.Data) {
            Console.WriteLine(t.ToJson());
        }
    }

}
