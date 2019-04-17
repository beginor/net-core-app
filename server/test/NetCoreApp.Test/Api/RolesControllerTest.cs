using Beginor.NetCoreApp.Api.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Beginor.NetCoreApp.Test.Api {

    [TestClass]
    public class RolesControllerTest : BaseTest<RolesController> {

        [TestMethod]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
