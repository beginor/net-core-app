using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Beginor.NetCoreApp.Common;

public static class ServiceCollectionExtensions {

    private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ServiceCollectionExtensions));

    extension(IServiceCollection services) {

        public IServiceCollection AddCommon(IConfiguration config, IHostEnvironment env) {
            // common options;
            var commonOption = new CommonOption();
            var commonSection = config.GetSection("common");
            commonSection.Bind(commonOption);
            services.AddSingleton(commonOption);
            var cacheFolder = Path.Combine(env.ContentRootPath, commonOption.Cache.Directory);
            if (!Directory.Exists(cacheFolder)) {
                logger.Error($"Cache directory {cacheFolder} does not exists, make sure your config is correct!");
                Directory.CreateDirectory(cacheFolder);
            }
            var storageFolder = Path.Combine(env.ContentRootPath, commonOption.Storage.Directory);
            if (!Directory.Exists(storageFolder)) {
                logger.Error($"Storage directory {storageFolder} does not exists, make sure your config is correct!");
                Directory.CreateDirectory(storageFolder);
            }
            // attachment
            var attachmentOptions = new AppAttachmentOptions();
            var attachmentSection = config.GetSection("attachment");
            attachmentSection.Bind(attachmentOptions);
            services.AddSingleton(attachmentOptions);
            // captcha
            var captchaOptions = new CaptchaOptions();
            var captchaSection = config.GetSection("captcha");
            captchaSection.Bind(captchaOptions);
            services.AddSingleton(captchaOptions);
            services.AddSingleton<ICaptchaGenerator, CaptchaGenerator>();
            return services;
        }

    }

}
