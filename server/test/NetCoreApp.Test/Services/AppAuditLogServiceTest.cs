using System;
using System.Threading.Tasks;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Services;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Services {

    [TestFixture]
    public class AppAuditLogServiceTest : BaseTest<IAppAuditLogService> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanSearch() {
            var model = new AppAuditLogSearchModel {
                Skip = 0,
                Take = 10,
                RequestDate = DateTime.Today,
                UserName = "admin"
            };
            var result = await Target.SearchAsync(model);
            Assert.IsNotNull(result);
            Assert.AreEqual(model.Skip, result.Skip);
            Assert.AreEqual(model.Take, result.Take);
            Assert.IsNotNull(result.Data);
            Assert.GreaterOrEqual(model.Take, result.Data.Count);
        }

    }

}
