using System.Collections.Generic;
using System.IO;
using System.Xml;
using Beginor.GisHub.DynamicSql;
using NUnit.Framework;
using static NUnit.Framework.Assert;
using static System.Console;

namespace Beginor.GisHub.Test.DynamicSql {

    [TestFixture]
    public class DynamicSqlTest : BaseTest<IDynamicSqlProvider> {

        private XmlElement rootElement;

        public DynamicSqlTest() {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Path.Combine("DynamicSql", "SqlMap.xml"));
            rootElement = xmlDoc.DocumentElement;
        }

        private string GetStatement(string id) {
            var statements = rootElement.GetElementsByTagName("Statement");
            foreach (XmlNode statement in statements) {
                if (statement.Attributes.TryGetValueAsString("Id", out var attrVal) && id == attrVal) {
                    return statement.OuterXml;
                }
            }
            return string.Empty;
        }

        [Test]
        public void _00_CanResolveTarget() {
            IsNotNull(Target);
        }

        [Test]
        public void _01_CanReadRootElement() {
            NotNull(rootElement);
            WriteLine(rootElement.Name);
        }

        [Test]
        public void _02_CanUseIsNotEmpty() {
            string command = GetStatement("is-not-empty");
            IsNotEmpty(command);
            var databaseType = "postgis";
            var parameters = new Dictionary<string, object>();
            var sql = Target.BuildDynamicSql(databaseType, command, parameters);
            IsNotEmpty(sql);
            WriteLine(sql);
            parameters["id"] = "440";
            sql = Target.BuildDynamicSql(databaseType, command, parameters);
            IsNotEmpty(sql);
            WriteLine(sql);
            parameters["area"] = 123.0;
            sql = Target.BuildDynamicSql(databaseType, command, parameters);
            IsNotEmpty(sql);
            WriteLine(sql);
        }

        [Test]
        public void _03_CanUseFor() {
            string command = GetStatement("for");
            IsNotEmpty(command);
            var databaseType = "postgis";
            var parameters = new Dictionary<string, object>();
            parameters["names"] = new[] { "4401", "4402", "4403" };
            var sql = Target.BuildDynamicSql(databaseType, command, parameters);
            IsNotEmpty(sql);
            WriteLine(sql);
            parameters["area"] = 123.0;
            sql = Target.BuildDynamicSql(databaseType, command, parameters);
            IsNotEmpty(sql);
            WriteLine(sql);
        }

    }

}
