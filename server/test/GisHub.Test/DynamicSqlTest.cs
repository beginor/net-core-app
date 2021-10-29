using System;
using System.Collections.Generic;
using NUnit.Framework;
using static NUnit.Framework.Assert;
using static System.Console;
using Beginor.GisHub.DynamicSql;

namespace Beginor.GisHub.Test {

    [TestFixture]
    public class DynamicSqlTest : BaseTest<IDynamicSqlProvider> {

        [Test]
        public void _01_CanResolveTarget() {
            IsNotNull(Target);
        }

        [Test]
        public void _02_CanBuildDynamicSql() {
            string command = @"<Statement Id=""123456"">
                  select fid, id, name, area
                  from public.sr4326_cn_sar_dist
                  <Dynamic Prepend=""where"">
                    <IsNotEmpty Prepend=""and"" Property=""id"">
                      id like @id || '%'
                    </IsNotEmpty>
                    <IsNotEmpty Prepend=""or"" Property=""area"">
                      area >= @area
                    </IsNotEmpty>
                    <For Prepend=""And"" Key=""LikeKey"" Open=""("" Close="")"" Property=""names"" Separator=""Or"">
                      name like Concat(@LikeKey,'%')
                    </For>
                  </Dynamic>
                </Statement>";
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
            parameters["names"] = new[] { "4401", "4402", "4403" };
            sql = Target.BuildDynamicSql(databaseType, command, parameters);
            IsNotEmpty(sql);
            WriteLine(sql);
        }

    }

}
