using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Test.Data;

[TestFixture]
public class AppAttachmentRepositoryTest : BaseTest<IAppAttachmentRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(Target, Is.Not.Null);
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
            Assert.That(data, Is.Not.Null);
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
            Assert.That(data, Is.Not.Null);
        }
    }

    [Test]
    public async Task _04_CanSaveAttachment() {
        var userManager = ServiceProvider.GetService<UserManager<AppUser>>();
        var user = await userManager.FindByNameAsync("testuser");
        Assert.That(user, Is.Not.Null);
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

    [Test]
    public async Task _05_CanCreateFolderForAttachment() {
        var commonOption = ServiceProvider.GetService<CommonOption>();
        Assert.That(commonOption, Is.Not.Null);
        var today = DateTime.Today;
        var folderPath = Path.Combine(
            commonOption.Storage.Directory,
            "app_attachments",
            today.Year.ToString("D4"),
            today.Month.ToString("D2"),
            today.Day.ToString("D2")
        );
        var dirInfo = new DirectoryInfo(folderPath);
        if (!dirInfo.Exists) {
            dirInfo.Create();
        }
        Assert.That(Directory.Exists(folderPath));
        Console.WriteLine(dirInfo.FullName);
    }

    [Test]
    public async Task _06_SaveAttachmentsToStorage() {
        var session = ServiceProvider.GetService<ISession>();
        var sql = @"select id, file_name, content from public.app_attachments
            where content is not null
        ";
        var conn = session.Connection;
        var reader = await conn.ExecuteReaderAsync(sql);
        var files = new Dictionary<long, string>();
        while (await reader.ReadAsync()) {
            var id = (long)reader["id"];
            var fileName = (string)reader["file_name"];
            var content = (byte[])reader["content"];
            var filePath = await Target.SaveContentAsync(id, content, Path.GetExtension(fileName));
            Console.WriteLine(filePath);
            files.Add(id, filePath);
        }
        await reader.CloseAsync();
        foreach (var (id, filePath) in files) {
            sql = @"update public.app_attachments
                set file_path = @filePath
                where id = @id
            ";
            await conn.ExecuteAsync(sql, new { id, filePath });
        }
    }

}
