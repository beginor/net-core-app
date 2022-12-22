using NUnit.Framework;
using Beginor.GisHub.Common;

namespace Beginor.GisHub.Test.Common;

[TestFixture]
public class Base64UrlEncoderTest {

    [Test]
    public void _01_CanEncode() {
        var input = "Hello,world!";
        var result = Base64UrlEncoder.Encode(input);
        Assert.AreEqual("SGVsbG8sd29ybGQh", result);
        //
        input += "!";
        result = Base64UrlEncoder.Encode(input);
        Assert.AreEqual("SGVsbG8sd29ybGQhIQ", result);
    }

    [Test]
    public void _02_CanDecode() {
        var input = "SGVsbG8sd29ybGQh";
        var output = Base64UrlEncoder.Decode(input);
        Assert.AreEqual("Hello,world!", output);
        input = "SGVsbG8sd29ybGQhIQ";
        output = Base64UrlEncoder.Decode(input);
        Assert.AreEqual("Hello,world!!", output);
    }

}
