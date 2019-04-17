using NHibernate;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Data {

    [TestFixture]
    public class HibernateTest : BaseTest<ISession> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.NotNull(Target);
        }

    }

}
