using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DataServices;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.Test.DataServices {

    [TestFixture]
    public class DataServiceFactoryTest : BaseTest<IDataServiceFactory> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        [TestCase("postgis")]
        public void _01_CanCreateMetadataProvider(string databaseType) {
            var provider = Target.CreateMetadataProvider(databaseType);
            Assert.IsNotNull(provider);
        }

        [Test]
        [TestCase("postgis")]
        public void _02_CanCreateDataSourceReader(string databaseType) {
            var reader = Target.CreateDataSourceReader(databaseType);
            Assert.IsNotNull(reader);
        }

    }

}
