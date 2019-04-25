using System;
using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Repositories;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Data {

    [TestFixture]
    public class AppAttachmentRepositoryTest : BaseTest<IAppAttachmentRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        [TestCase("0000", "application/xml")]
        public async Task _02_CanQueryCountAsync(
            string userId,
            string contentType
        ) {
            var count = await Target.CountAsync(userId, contentType);
            Assert.GreaterOrEqual(count, 0);
            Console.WriteLine(count);
        }

        [Test]
        [TestCase("0000", "application/xml", 0, 1)]
        public async Task _03_CanQueryAsync(
            string userId,
            string contentType,
            int skip,
            int take
        ) {
            var data = await Target.QueryAsync(userId, contentType, skip, take);
            Assert.NotNull(data);
            Assert.GreaterOrEqual(data.Count, 0);
        }

    }
}
