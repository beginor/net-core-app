using System;
using System.Linq;
using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.AppFx.Core;

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
        var result = await Target.StatsDurationAsync(startDate, endDate);
        Assert.IsNotNull(result.Data);
        Console.WriteLine(result.Data.Count);
        foreach (var t in result.Data) {
            Console.WriteLine(t.ToJson());
        }
    }

}
