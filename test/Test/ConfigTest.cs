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
        Assert.IsNotNull(Target);
    }

    [Test]
    public void _02_CanGetJwtOptions() {
        var setting = Target.GetSection("jwt");
        Assert.IsNotNull(setting);
        var jwt = setting.Get<JwtOption>();
        Assert.IsNotEmpty(jwt.Secret);
    }

    [Test]
    public void _03_CanResolveOptions() {
        var section = Target.GetSection("identity");
        var options = section.Get<IdentityOptions>();
        // Test Config Options
        Assert.IsNotNull(options);
        Assert.AreEqual(8, options.Password.RequiredLength);
        Assert.AreEqual(
            TimeSpan.FromHours(1),
            options.Lockout.DefaultLockoutTimeSpan
        );
    }

    [Test]
    public void _04_CanResolveCorsPolicy() {
        var section = Target.GetSection("cors");
        var policy = section.Get<CorsPolicy>();
        Assert.IsFalse(policy.AllowAnyOrigin);
        Assert.IsTrue(policy.AllowAnyHeader);
        Assert.IsTrue(policy.AllowAnyMethod);
        Assert.IsTrue(policy.SupportsCredentials);
    }

    [Test]
    public void _05_CanResolveSpaFailback() {
        var section = Target.GetSection("spaFailback");
        var spaFailback = section.Get<SpaFailbackOptions>();
        Assert.IsNotNull(spaFailback);
        Console.WriteLine(spaFailback.Rules.Count);
    }

    [Test]
    public void _06_CanResolveCustomHeader() {
        var section = Target.GetSection("customHeader");
        var options = section.Get<CustomHeaderOptions>();
        Assert.IsNotNull(options.Headers);
        Assert.Greater(options.Headers.Keys.Count, 0);
    }

    [Test]
    public void _07_CanGetWeChatOptions() {
        var section = Target.GetSection("wechat");
        var options = section.Get<WeChatOption>();
        Assert.IsNotNull(options.AppId);
        Assert.IsNotNull(options.Secret);
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
        Assert.AreEqual(shouldIgnore, !isMatch);
        var match = regex.Match(url);
        Console.WriteLine(match);
    }

    [Test]
    public void _08_CanResolveCommonOption() {
        var option = ServiceProvider.GetService<CommonOption>();
        Assert.IsNotNull(option);
        Console.WriteLine(option.Cache.Enabled);
        Console.WriteLine(option.Cache.MemoryExpiration);
        Console.WriteLine(option.Cache.FileExpiration);
        Console.WriteLine(option.Cache.Directory);
    }

}
