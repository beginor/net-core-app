using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Services {

    [TestFixture]
    public class ModelMappingTest : BaseTest {

        [Test]
        public void _01_CanMapAppUser() {
            var user = new AppUser {
                Id = "000000001",
                UserName = "TestUser"
            };
            var entity = Mapper.Map<StringIdNameEntity>(user);
            Assert.AreEqual(user.Id, entity.Id);
            Assert.AreEqual(user.UserName, entity.Name);
            user = Mapper.Map<AppUser>(entity);
            Assert.AreEqual(user.Id, entity.Id);
            Assert.AreEqual(user.UserName, entity.Name);
        }

        [Test]
        public void _02_CanMapAppRole() {
            var role = new AppRole {
                Id = "000001",
                Name = "TestRole"
            };
            var entity = Mapper.Map<StringIdNameEntity>(role);
            Assert.AreEqual(role.Id, entity.Id);
            Assert.AreEqual(role.Name, entity.Name);
            role = Mapper.Map<AppRole>(entity);
            Assert.AreEqual(role.Id, entity.Id);
            Assert.AreEqual(role.Name, entity.Name);
        }

    }

}
