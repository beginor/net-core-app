namespace Beginor.NetCoreApp.WeChat.Models;

/// <summary>
/// 微信小程序登录请求参数
/// </summary>
public class WeChatLoginRequestModel {
    /// <summary>
    /// 小程序临时登录凭证
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// 微信小程序手机登录请求参数
/// </summary>
public class WeChatLoginByPhoneRequestModel {
    /// <summary>
    /// Session 缓存对应ID
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// 小程序获取手机号临时登录凭证
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 加密数据
    /// </summary>
    public string EncryptedData { get; set; } = string.Empty;

    /// <summary>
    /// 加密算法的初始向量
    /// </summary>
    public string IV { get; set; } = string.Empty;
}

/// <summary>
/// 微信小程序登录响应参数
/// </summary>
public class WeChatLoginResponseModel {
    /// <summary>
    /// 错误码
    /// </summary>
    public int ErrCode { get; set; } = 0;

    /// <summary>
    /// 错误描述
    /// </summary>
    public string ErrMsg { get; set; } = string.Empty;

    /// <summary>
    /// Session 缓存对应ID
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// 登录成功时响应
    /// </summary>
    public string TmpTokenKey { get; set; } = string.Empty;

    /// <summary>
    /// 登录成功时响应
    /// </summary>
    public string TmpTokenValue { get; set; } = string.Empty;
}
