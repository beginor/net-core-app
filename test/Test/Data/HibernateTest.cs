using System;
using System.Linq;
using NHibernate;
using NUnit.Framework;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Test.Data;

[TestFixture]
public class HibernateTest : BaseTest<ISessionFactory> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(Target, Is.Not.Null);
    }

    [Test]
    public void _02_CanQueryApplicationUsers() {
        using (var session = Target.OpenSession()) {
            var users = session.Query<AppUser>().ToList();
            Console.WriteLine(users.Count);
            Assert.That(users.Count >= 0);
        }
    }

}
