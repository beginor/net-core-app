using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Data {

    [TestFixture]
    public class ModelMappingTest : BaseTest<IMapper> {

        [Test]
        public void _01_CanMapAppUser() {
            var user = new AppUser {
                Id = "000000001",
                UserName = "TestUser"
            };
            var entity = Target.Map<StringIdNameEntity>(user);
            Assert.AreEqual(user.Id, entity.Id);
            Assert.AreEqual(user.UserName, entity.Name);
            user = Target.Map<AppUser>(entity);
            Assert.AreEqual(user.Id, entity.Id);
            Assert.AreEqual(user.UserName, entity.Name);
        }

        [Test]
        public void _02_CanMapAppRole() {
            var role = new AppRole {
                Id = "000001",
                Name = "TestRole"
            };
            var entity = Target.Map<StringIdNameEntity>(role);
            Assert.AreEqual(role.Id, entity.Id);
            Assert.AreEqual(role.Name, entity.Name);
            role = Target.Map<AppRole>(entity);
            Assert.AreEqual(role.Id, entity.Id);
            Assert.AreEqual(role.Name, entity.Name);
        }

        [Test]
        public void _03_CanDoCustomMapping() {
            var config = new MapperConfiguration(configure => {
                configure.CreateMap<TestEntity, TestModel>()
                    .ForMember(t => t.UserId, m => m.MapFrom(s => s.User.Id))
                    .ForMember(t => t.UserName, m => m.MapFrom(s => s.User.Name))
                    .ReverseMap();
            });
            var mapper = config.CreateMapper();
            var entity = new TestEntity {
                Id = 123,
                Name = "entity",
                User = new TestUser {
                    Id = 456,
                    Name = "beginor"
                }
            };
            var model = mapper.Map<TestModel>(entity);
            Assert.AreEqual(entity.Id, model.Id);
            Assert.AreEqual(entity.User.Id, model.UserId);
            var entity2 = mapper.Map<TestEntity>(model);
            Assert.AreEqual(model.Id, entity2.Id);
            Assert.AreEqual(model.UserId, entity2.User.Id);
        }

    }

    public class TestEntity {
        public long Id { get; set; }
        public string Name { get; set; }

        public TestUser User { get; set; }
    }

    public class TestUser {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class TestModel {
        public long Id { get; set; }
        public string Name { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
    }

}
