using System;
using System.Linq;
using System.Threading.Tasks;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Data.Repositories;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;

namespace Beginor.GisHub.Test.Data {

    [TestFixture]
    public class AppAuditLogRepositoryTest : BaseTest<IAppAuditLogRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
