using System.Text.Json.Serialization;

namespace Beginor.NetCoreApp.WeChat.Models;

public class WeChatGetUserPhoneNumberRequest {
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}
