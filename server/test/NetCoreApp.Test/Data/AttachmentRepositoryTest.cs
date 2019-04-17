using Beginor.NetCoreApp.Data.Repositories;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Data {

    [TestFixture]
    public class AttachmentRepositoryTest : BaseTest<IAttachmentRepository> {

        [SetUp]
        public void SetUp() {}

        [TearDown]
        public void TearDown() {}

        [Test]
        public void _01_CanResolveTarget() {
            Assert.NotNull(Target);
        }

    }
}
