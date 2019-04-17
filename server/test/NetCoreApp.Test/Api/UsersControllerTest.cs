using Beginor.NetCoreApp.Api.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Beginor.NetCoreApp.Test.Api {

    [TestClass]
    public class UsersControllerTest : BaseTest<UsersController> {

        [TestMethod]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
