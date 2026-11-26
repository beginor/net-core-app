using System.IO;
using System.Xml;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Logging.Log4net;

namespace Beginor.NetCoreApp.Entry;

public static class LoggingExtensions {

    // 加载 log4net 配置文件，用环境变量中的数据库连接串覆盖 AdoNetAppender 后注册日志。
    public static ILoggingBuilder AddLog4netWithDatabaseConnection(
        this ILoggingBuilder builder,
        string configFile
    ) {
        if (!File.Exists(configFile)) {
            throw new FileNotFoundException($"Log config file {configFile} does not exists!", configFile);
        }
        var connectionString = Startup.GetDatabaseConnection();
        var doc = new XmlDocument();
        doc.Load(configFile);
        if (!string.IsNullOrWhiteSpace(connectionString)) {
            var node = doc.SelectSingleNode("//appender[@name='AdoNetAppender']/connectionString");
            if (node is XmlElement element) {
                element.SetAttribute("value", connectionString);
            }
        }
        return builder.AddLog4net(doc.DocumentElement!);
    }

}
