using System;
using System.Text;

namespace Beginor.NetCoreApp.Common;

public static class Base64UrlEncoder {

    public static string Encode(string input) {
        var bytes = Encoding.UTF8.GetBytes(input);
        var output = Convert.ToBase64String(bytes);
        output = output.Split('=')[0];
        output = output.Replace('+', '-');
        output = output.Replace('/', '_');
        return output;
    }

    public static string Decode(string input) {
        var output = input;
        output = output.Replace('-', '+');
        output = output.Replace('_', '/');
        switch (output.Length % 4) { // Pad with trailing '='s
            case 0:
                break; // No pad chars in this case
            case 2:
                output += "==";
                break; // Two pad chars
            case 3:
                output += "=";
                break; // One pad char
            default:
                throw new ArgumentOutOfRangeException(nameof(input), "Illegal base64url string!");
        }
        var bytes = Convert.FromBase64String(output);
        return Encoding.UTF8.GetString(bytes);
    }

}
