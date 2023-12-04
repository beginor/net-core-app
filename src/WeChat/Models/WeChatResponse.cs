using System.Text.Json.Serialization;

namespace Beginor.NetCoreApp.WeChat;

public class WeChatBaseResponse {
    /// <summary>
    /// 错误码
    /// </summary>
    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; } = 0;

    /// <summary>
    /// 错误信息
    /// </summary>

    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; } = string.Empty;
}


public class WeChatAccessTokenResponse : WeChatBaseResponse {
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; } = 0;
}

public class WeChatJsCode2SessionResponse : WeChatBaseResponse {
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;
    [JsonPropertyName("session_key")]
    public string SessionKey { get; set; } = string.Empty;
    [JsonPropertyName("unionid")]
    public string UnionId { get; set; } = string.Empty;
}

public class WeChatGetUserPhoneNumberResponse : WeChatBaseResponse {
    [JsonPropertyName("phone_info")]
    public WeChatPhoneInfo PhoneInfo { get; set; } = new();
}
public class WeChatPhoneInfo {
    /// <summary>
    /// 用户绑定的手机号（国外手机号会有区号）
    /// </summary>
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// 没有区号的手机号
    /// </summary>
    [JsonPropertyName("purePhoneNumber")]
    public string PurePhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// 区号
    /// </summary>

    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// 数据水印
    /// </summary>

    [JsonPropertyName("watermark")]
    public WeChatWatermark Watermark { get; set; } = new();
}

public class WeChatWatermark {

    /// <summary>
    /// 用户获取手机号操作的时间戳
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; } = 0;

    /// <summary>
    /// 小程序appid
    /// </summary>

    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;
}