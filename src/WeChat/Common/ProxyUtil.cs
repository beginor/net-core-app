using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Beginor.NetCoreApp.WeChat.Common;

public static class ProxyUtil {

    public static HttpClient CreateHttpClient(string baseUrl) {
        var handler = new HttpClientHandler() {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
            AutomaticDecompression = DecompressionMethods.All,
            AllowAutoRedirect = false,
        };
        var http = new HttpClient(handler);
        http.BaseAddress = new Uri(baseUrl);
        return http;
    }

    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="encryptStr">密文字符串</param>
    /// <param name="key">密钥</param>
    /// <param name="iv">向量</param>
    /// <returns>明文</returns>
    public static string AESDecrypt(string encryptStr, string key, string iv) {
        var bKey = Convert.FromBase64String(key);
        var bIV = Convert.FromBase64String(iv);
        var byteArray = Convert.FromBase64String(encryptStr);
        using var aes = Aes.Create();
        aes.Key = bKey;
        aes.IV = bIV;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using var decryptor = aes.CreateDecryptor();
        var result = decryptor.TransformFinalBlock(byteArray, 0, byteArray.Length);
        return Encoding.UTF8.GetString(result);
    }

    public static T? AESDecrypt<T>(string encryptStr, string key, string iv) {
        var json = AESDecrypt(encryptStr, key, iv);
        return JsonSerializer.Deserialize<T>(json);
    }
}
