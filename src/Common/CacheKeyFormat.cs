namespace Beginor.NetCoreApp.Common;

public static class CacheKeyFormat {

    public static readonly string TmpToken = "net-core-app:tmp_token:{0}";
    public static readonly string WeChatAuthSession = "net-core-app:wechat_auth_session:{0}";
    public static readonly string WeChatAuthAccessToken = "net-core-app:wechat_auth_access_token:{0}_{1}_{2}";
    public static readonly string UserClaims = "net-core-app:user_claims:{0}";
    public static readonly string Captcha = "net-core-app:captcha:{0}";
    public static readonly string Storage = "net-core-app:app_storage:{0}";
    public static readonly string UserToken = "net-core-app:user_token:{0}";

}
