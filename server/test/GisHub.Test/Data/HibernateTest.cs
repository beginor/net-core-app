using System;
using System.Linq;
using NHibernate;
using NUnit.Framework;
using Beginor.GisHub.Data.Entities;

namespace Beginor.GisHub.Test.Data;

[TestFixture]
public class HibernateTest : BaseTest<ISessionFactory> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

    [Test]
    public void _02_CanQueryApplicationUsers() {
        using (var session = Target.OpenSession()) {
            var users = session.Query<AppUser>().ToList();
            Console.WriteLine(users.Count);
            Assert.IsTrue(users.Count >= 0);
        }
    }
    
    [Test]
    public void _03_CanQueryBaseResource() {
        using var session = Target.OpenSession();
        var query = session.Query<BaseResource>().Where(r => r.Type == "data_api");
        var data = query.ToList();
        Console.WriteLine(data.ToJson());
    }

}
