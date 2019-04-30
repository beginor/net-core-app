using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Config {

    [TestFixture]
    public class ConfigTest : BaseTest<IConfiguration> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public void _02_CanGetValue() {
            var allowedHosts = Target.GetValue<string>("AllowedHosts");
            var allowedHosts2 = Target.GetValue<string>("allowedHosts");
            Assert.AreEqual(allowedHosts, allowedHosts2);
            Assert.IsNotEmpty(allowedHosts);
            Console.WriteLine($"allowedHosts: {allowedHosts}");
            Console.WriteLine($"allowedHosts2: {allowedHosts2}");
        }

        [Test]
        public void _023CanResolveOptions() {
            var section = Target.GetSection("IdentityOptions");
            var options = section.Get<IdentityOptions>();
            // Test Config Options
            Assert.IsNotNull(options);
            Assert.AreEqual(8, options.Password.RequiredLength);
            Assert.AreEqual(
                TimeSpan.FromHours(1),
                options.Lockout.DefaultLockoutTimeSpan
            );
        }

    }

}
