using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Beginor.AspNetCore.Middlewares.CustomHeader;
using Beginor.AspNetCore.Middlewares.SpaFailback;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.WeChat.Common;

namespace Beginor.NetCoreApp.Test;

[TestFixture]
public class ConfigTest : BaseTest<IConfiguration> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(Target, Is.Not.Null);
    }

    [Test]
    public void _02_CanGetJwtOptions() {
        var setting = Target.GetSection("jwt");
        Assert.That(setting, Is.Not.Null);
        var jwt = setting.Get<JwtOption>();
        Assert.That(jwt.Secret, Is.Not.Empty);
    }

    [Test]
    public void _03_CanResolveOptions() {
        var section = Target.GetSection("identity");
        var options = section.Get<IdentityOptions>();
        // Test Config Options
        Assert.That(options, Is.Not.Null);
        Assert.That(options.Password.RequiredLength, Is.EqualTo(8));
        Assert.That(
            options.Lockout.DefaultLockoutTimeSpan,
            Is.EqualTo(TimeSpan.FromHours(1))
        );
    }

    [Test]
    public void _04_CanResolveCorsPolicy() {
        var section = Target.GetSection("cors");
        var policy = section.Get<CorsPolicy>();
        Assert.That(policy.AllowAnyOrigin, Is.False);
        Assert.That(policy.AllowAnyHeader);
        Assert.That(policy.AllowAnyMethod);
        Assert.That(policy.SupportsCredentials);
    }

    [Test]
    public void _05_CanResolveSpaFailback() {
        var section = Target.GetSection("spaFailback");
        var spaFailback = section.Get<SpaFailbackOptions>();
        Assert.That(spaFailback, Is.Not.Null);
        Console.WriteLine(spaFailback.Rules.Count);
    }

    [Test]
    public void _06_CanResolveCustomHeader() {
        var section = Target.GetSection("customHeader");
        var options = section.Get<CustomHeaderOptions>();
        Assert.That(options.Headers, Is.Not.Null);
        Assert.That(options.Headers.Keys.Count, Is.GreaterThan(0));
    }

    [Test]
    public void _07_CanGetWeChatOptions() {
        var section = Target.GetSection("wechat");
        var options = section.Get<WeChatOption>();
        Assert.That(options.AppId, Is.Not.Null);
        Assert.That(options.Secret, Is.Not.Null);
    }

    [Test]
    [TestCase("/web/home")]
    [TestCase("/web/about")]
    [TestCase("/web/admin/users")]
    [TestCase("/web/assets/icon")]
    public void _07_CanMatchUrl(string url) {
        var shouldIgnore = url.ToLowerInvariant().StartsWith("/web/assets/");
        var regex = new Regex("/web/(?!assets/).*");
        var isMatch = regex.IsMatch(url);
        Assert.That(shouldIgnore, Is.EqualTo(!isMatch));
        var match = regex.Match(url);
        Console.WriteLine(match);
    }

    [Test]
    public void _08_CanResolveCommonOption() {
        var option = ServiceProvider.GetService<CommonOption>();
        Assert.That(option, Is.Not.Null);
        Console.WriteLine(option.Cache.Enabled);
        Console.WriteLine(option.Cache.MemoryExpiration);
        Console.WriteLine(option.Cache.FileExpiration);
        Console.WriteLine(option.Cache.Directory);
    }

}
