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

    }

}
