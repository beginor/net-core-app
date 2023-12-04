using System.Text.Json.Serialization;

namespace Beginor.NetCoreApp.WeChat;

public class WeChatGetUserPhoneNumberRequest {
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}