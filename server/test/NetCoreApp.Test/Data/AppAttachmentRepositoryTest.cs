using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;
using NHibernate.NetCore;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Data {

    [TestFixture]
    public class AppAttachmentRepositoryTest : BaseTest<IAppAttachmentRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public void _02_CanQueryWithUser() {
            var factory = ServiceProvider.GetSessionFactory();
            using (var session = factory.OpenSession()) {
                var query = from att in session.Query<AppAttachment>()
                    select new AppAttachment {
                        Id = att.Id,
                        BusinessId = att.BusinessId,
                        Creator = new AppUser {
                            Id = att.Creator.Id,
                            UserName = att.Creator.UserName
                        }
                    };
                var data = query.ToList();
                var models = Mapper.Map<List<AppAttachmentModel>>(data);
            }
        }

    }

}
