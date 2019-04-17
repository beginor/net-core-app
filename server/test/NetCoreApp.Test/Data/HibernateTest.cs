using System;
using System.Linq;
using Beginor.NetCoreApp.Data.Entities;
using NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Beginor.NetCoreApp.Test.Data {

    [TestClass]
    public class HibernateTest : BaseTest<ISessionFactory> {

        [TestMethod]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [TestMethod]
        public void _02_CanQueryApplicationUsers() {
            using (var session = Target.OpenSession()) {
                var users = session.Query<ApplicationUser>().ToList();
                Console.WriteLine(users.Count);
                Assert.IsTrue(users.Count >= 0);
            }
        }

    }

}
