using System.Threading.Tasks;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.WeChat;

public static class ApiGatewayExtensions {

    public static async Task<WeChatJsCode2SessionResponse> GetJsCode2SessionAsync(this ApiGateway api, string jsCode) {
        var url = $"/sns/jscode2session?appid={api.option.AppId}&secret={api.option.Secret}&js_code={jsCode}&grant_type=authorization_code";
        var res = await api.RequestAsync<WeChatJsCode2SessionResponse>(url, string.Empty);
        return res;
    }
}
