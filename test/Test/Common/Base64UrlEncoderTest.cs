using NUnit.Framework;
using Beginor.NetCoreApp.Common;

namespace Beginor.NetCoreApp.Test.Common;

[TestFixture]
public class Base64UrlEncoderTest {

    [Test]
    public void _01_CanEncode() {
        var input = "Hello,world!";
        var result = Base64UrlEncoder.Encode(input);
        Assert.That(result, Is.EqualTo("SGVsbG8sd29ybGQh"));
        //
        input += "!";
        result = Base64UrlEncoder.Encode(input);
        Assert.That(result, Is.EqualTo("SGVsbG8sd29ybGQhIQ"));
    }

    [Test]
    public void _02_CanDecode() {
        var input = "SGVsbG8sd29ybGQh";
        var output = Base64UrlEncoder.Decode(input);
        Assert.That(output, Is.EqualTo("Hello,world!"));
        input = "SGVsbG8sd29ybGQhIQ";
        output = Base64UrlEncoder.Decode(input);
        Assert.That(output, Is.EqualTo("Hello,world!!"));
    }

}
