namespace Beginor.NetCoreApp.WeChat.Common;

/// <summary>
/// 微信相关配置
/// </summary>
public class WeChatOption {
    /// <summary>
    /// baseurl
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
    /// <summary>
    /// 小程序AppId
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// 小程序AppSecret
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// client_credential
    /// </summary>
    public string GrantType { get; set; } = string.Empty;
}
