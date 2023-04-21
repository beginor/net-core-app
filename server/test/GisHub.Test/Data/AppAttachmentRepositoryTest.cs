using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Test.Data;

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
            Assert.IsNotNull(data);
        }
    }

    [Test]
    public void _03_CanProjectToModel() {
        var factory = ServiceProvider.GetSessionFactory();
        using (var session = factory.OpenSession()) {
            var query = session.Query<AppAttachment>()
                .Where(att => att.Creator.UserName == "TestUser")
                .Select(att => new AppAttachment {
                    Id = att.Id,
                    FileName = att.FileName,
                    CreatedAt = att.CreatedAt,
                    Creator = new AppUser {
                        Id = att.Creator.Id,
                        UserName = att.Creator.UserName,
                        LoginCount = att.Creator.LoginCount,
                        LastLogin = att.Creator.LastLogin
                    }
                });
            //.ProjectTo<AppAttachmentModel>();
            var data = query.ToList();
            Assert.IsNotNull(data);
        }
    }

    [Test]
    public async Task _04_CanSaveAttachment() {
        var userManager = ServiceProvider.GetService<UserManager<AppUser>>();
        var user = await userManager.FindByNameAsync("testuser");
        Assert.IsNotNull(user);
        var model = new AppAttachmentModel {
            BusinessId = "123456",
            FileName = "P30PRO.pdf",
            ContentType = "application/pdf",
            Length = 123456
        };
        // var content = await System.IO.File.ReadAllBytesAsync("");
        var content = System.Text.Encoding.UTF8.GetBytes("hello, world");
        await Target.SaveAsync(model, content, user);
    }

}
