using System;
using System.Linq;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices {

    public static class SqlValidator {
        private static readonly string[] Disallowed = {
            "alert", "update", "exec", "delete", "master", "truncate",
            "declare", "create", "drop", "select", "join", "%%", "--",
            "*", "1=1", "0=0"
        };

        public static bool IsValid(string sql) {
            if (sql.IsNullOrEmpty()) {
                return true;
            }
            if (sql.Trim() == "*") {
                return false;
            }
            var sqlArr = sql.Split(' ', ',');
            for (var i = 0; i < sqlArr.Length; i++) {
                if (sqlArr[i] == "=") {
                    if (i - 1 < 0) {
                        return false;
                    }
                    if (i + 1 > sqlArr.Length) {
                        return false;
                    }
                    if (sqlArr[i - 1].EqualsOrdinalIgnoreCase(sqlArr[i + 1])) {
                        return false;
                    }
                }
            }
            return sqlArr.All(
                s => !Disallowed.Contains(s, StringComparer.OrdinalIgnoreCase)
            );
        }

    }

}
