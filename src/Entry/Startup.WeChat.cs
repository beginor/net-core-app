using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beginor.NetCoreApp.WeChat.Common;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureWeChatServices(IServiceCollection services, IWebHostEnvironment env) {
        var section = config.GetSection("wechat");
        if (section.Exists()) {
            services.Configure<WeChatOption>(section);
            services.AddSingleton<ApiGateway>();
        }
    }

    private void ConfigureWeChat(WebApplication app, IWebHostEnvironment env) {
        if (config.GetSection("wechat").Exists()) {
            // do nothing now.
        }
    }

}
