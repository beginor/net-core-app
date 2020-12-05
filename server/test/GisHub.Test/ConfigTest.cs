using System;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Beginor.GisHub.Api.Middlewares;
using Beginor.GisHub.Common;

namespace Beginor.GisHub.Test {

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
            Assert.IsFalse(policy.SupportsCredentials);
        }

        [Test]
        public void _05_CanResolveSpaFailback() {
            var section = Target.GetSection("spaFailback");
            var spaFailback = section.Get<SpaFailbackOptions>();
            Assert.IsNotNull(spaFailback);
            Console.WriteLine(spaFailback.Failbacks.Count);
        }

        [Test]
        public void _06_CanResolveCustomHeader() {
            var section = Target.GetSection("customHeader");
            var options = section.Get<CustomHeaderOptions>();
            Assert.IsNotNull(options.Headers);
            Assert.Greater(options.Headers.Keys.Count, 0);
        }

    }

}
