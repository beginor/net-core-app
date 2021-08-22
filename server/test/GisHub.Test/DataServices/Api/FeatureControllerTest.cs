using System.Threading.Tasks;
using Beginor.GisHub.DataServices.Api;
using Beginor.GisHub.Geo.Esri;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Beginor.GisHub.Test.DataServices.Api {

    [TestFixture]
    public class FeatureControllerTest : BaseTest<FeatureController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanGetLayerDescription() {
            var result = await Target.GetLayerDescriptionAsync(
                0,
                new AgsCommonParams { Format = "json" }
            );
            Assert.IsNotNull(result);
        }

    }

}
