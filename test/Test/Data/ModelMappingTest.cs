using AutoMapper;
using NUnit.Framework;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Beginor.NetCoreApp.Test.Data;

[TestFixture]
public class ModelMappingTest : BaseTest<IMapper> {

    [Test]
    public void _01_CanMapAppUser() {
        var user = new AppUser {
            Id = "000000001",
            UserName = "TestUser"
        };
        var entity = Target.Map<StringIdNameEntity>(user);
        Assert.That(user.Id, Is.EqualTo(entity.Id));
        Assert.That(user.UserName, Is.EqualTo(entity.Name));
        user = Target.Map<AppUser>(entity);
        Assert.That(user.Id, Is.EqualTo(entity.Id));
        Assert.That(user.UserName, Is.EqualTo(entity.Name));
    }

    [Test]
    public void _02_CanMapAppRole() {
        var role = new AppRole {
            Id = "000001",
            Name = "TestRole"
        };
        var entity = Target.Map<StringIdNameEntity>(role);
        Assert.That(role.Id, Is.EqualTo(entity.Id));
        Assert.That(role.Name, Is.EqualTo(entity.Name));
        role = Target.Map<AppRole>(entity);
        Assert.That(role.Id, Is.EqualTo(entity.Id));
        Assert.That(role.Name, Is.EqualTo(entity.Name));
    }

    [Test]
    public void _03_CanDoCustomMapping() {
        var config = new MapperConfiguration(
            configure => {
                configure.CreateMap<TestEntity, TestModel>()
                    .ForMember(t => t.UserId, m => m.MapFrom(s => s.User.Id))
                    .ForMember(t => t.UserName, m => m.MapFrom(s => s.User.Name))
                    .ReverseMap();
            },
            ServiceProvider.GetRequiredService<ILoggerFactory>()
        );
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
        Assert.That(entity.Id, Is.EqualTo(model.Id));
        Assert.That(entity.User.Id, Is.EqualTo(model.UserId));
        var entity2 = mapper.Map<TestEntity>(model);
        Assert.That(model.Id, Is.EqualTo(entity2.Id));
        Assert.That(model.UserId, Is.EqualTo(entity2.User.Id));
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
