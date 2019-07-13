using System;
using System.Linq;
using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Data {

    [TestFixture]
    public class AppNavItemRepositoryTest : BaseTest<IAppNavItemRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
