using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml;
using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.DataSource;
using SmartSql.Utils;

namespace Beginor.GisHub.DynamicSql {

    public class SmartSqlProvider : IDynamicSqlProvider {

        private TagBuilderFactory tagBuilderFactory = new();
        private Dictionary<string, SqlMap> sqlmaps = new(StringComparer.OrdinalIgnoreCase) {
            ["postgis"] = new SqlMap {
                SmartSqlConfig = new SmartSqlConfig {
                    Settings = new Settings { IgnoreParameterCase = true, ParameterPrefix = "$" },
                    Database = new Database { DbProvider = DbProviderManager.POSTGRESQL_DBPROVIDER },
                    SqlParamAnalyzer = new SqlParamAnalyzer(true, "@")
                }
            },
            ["postsde"] = new SqlMap {
                SmartSqlConfig = new SmartSqlConfig {
                    Settings = new Settings { IgnoreParameterCase = true, ParameterPrefix = "$" },
                    Database = new Database { DbProvider = DbProviderManager.POSTGRESQL_DBPROVIDER },
                    SqlParamAnalyzer = new SqlParamAnalyzer(true, "@")
                }
            },
            ["mssql"] = new SqlMap {
                SmartSqlConfig = new SmartSqlConfig {
                    Settings = new Settings { IgnoreParameterCase = true, ParameterPrefix = "$" },
                    Database = new Database { DbProvider = DbProviderManager.SQLSERVER_DBPROVIDER },
                    SqlParamAnalyzer = new SqlParamAnalyzer(true, "@")
                }
            },
            ["mysql"] = new SqlMap {
                SmartSqlConfig = new SmartSqlConfig {
                    Settings = new Settings { IgnoreParameterCase = true, ParameterPrefix = "$" },
                    Database = new Database { DbProvider = DbProviderManager.MYSQL_DBPROVIDER },
                    SqlParamAnalyzer = new SqlParamAnalyzer(true, "@")
                }
            }
        };

        private ILogger<SmartSqlProvider> logger;

        public SmartSqlProvider(
            ILogger<SmartSqlProvider> logger
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string BuildDynamicSql(string databaseType, string command, IDictionary<string, object> parameters) {
            if (string.IsNullOrEmpty(databaseType)) {
                throw new ArgumentNullException(nameof(databaseType));
            }
            if (string.IsNullOrEmpty(command)) {
                throw new ArgumentNullException(nameof(command));
            }
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (!sqlmaps.TryGetValue(databaseType, out var sqlmap)) {
                logger.LogError($"Unknown database type {databaseType} !");
                throw new ArgumentOutOfRangeException(nameof(databaseType));
            }
            var statement = CreateStatement(command, sqlmap);
            if (statement == null) {
                throw new ArgumentException($"Can not create statement from {command} ");
            }
            var context = BuildSqlRequestContext(statement, parameters);
            foreach (var (key, value) in context.Parameters) {
                if (!parameters.ContainsKey(key)) {
                    parameters.Add(key, value.Value);
                }
            }
            return context.SqlBuilder.ToString();
        }

        public DbProviderFactory GetDbProviderFactory(string databaseType) {
            if (!sqlmaps.TryGetValue(databaseType, out var sqlmap)) {
                logger.LogError($"Unknown database type {databaseType} !");
                throw new ArgumentOutOfRangeException(nameof(databaseType));
            }
            var dbProvider = sqlmap.SmartSqlConfig.Database.DbProvider;
            return dbProvider.Factory ?? (dbProvider.Factory = DbProviderManager.Instance.GetDbProviderFactory(dbProvider.Type));
        }

        private Statement CreateStatement(string xml, SqlMap sqlmap) {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var root = xmlDoc.DocumentElement;
            if (root == null) {
                return null;
            }
            root.Attributes.TryGetValueAsString("Id", out var id);
            var statement = new Statement {
                Id = id,
                SqlMap = sqlmap,
                SqlTags = new List<ITag>(),
            };
            // add tag
            foreach (XmlNode node in root.ChildNodes) {
                var tag = LoadTag(node, statement);
                if (tag != null) {
                    statement.SqlTags.Add(tag);
                }
            }
            return statement;
        }

        private ITag LoadTag(XmlNode xmlNode, Statement stmt) {
            if (xmlNode.Name == "#comment") {
                return null;
            }
            var tag = tagBuilderFactory.Get(xmlNode.Name).Build(xmlNode, stmt);
            foreach (XmlNode childNode in xmlNode) {
                var childTag = LoadTag(childNode, stmt);
                if (childTag == null) {
                    continue;
                }
                childTag.Parent = tag;
                ((Tag)tag).ChildTags.Add(childTag);
            }
            return tag;
        }

        private static RequestContext<T> BuildSqlRequestContext<T>(Statement statement, T parameters) where T : class {
            var requestContext = new RequestContext<T>();
            SetExecutionContext(requestContext, statement.SqlMap.SmartSqlConfig);
            requestContext.SetRequest(parameters);
            requestContext.SetupParameters();
            statement.BuildSql(requestContext);
            return requestContext;
        }

        private static void SetExecutionContext(
            AbstractRequestContext requestContext,
            SmartSqlConfig config
        ) {
            // requestContext.ExecutionContext
            var propInfo = requestContext.GetType().GetProperty("ExecutionContext");
            var setter = propInfo.GetSetMethod(true);
            var executionContext = new ExecutionContext {
                Request = requestContext,
                SmartSqlConfig = config
            };
            setter.Invoke(requestContext, new [] { executionContext });
        }
    }

}
