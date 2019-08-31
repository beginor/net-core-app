using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models {

    /// <summary>登录信息</summary>
    public class AccountLoginModel {

        /// <summary>用户名</summary>
        [Required(ErrorMessage = "用户名必须填写！")]
        public string UserName { get; set; }

        /// <summary>密码</summary>
        [Required(ErrorMessage = "密码必须填写！")]
        public string Password { get; set; }

        /// <summary>保持登录</summary>
        public bool IsPersistent { get; set; }

    }

    /// <summary>已经登录的账户信息</summary>
    public class AccountInfoModel : StringEntity {

        /// <summary>用户名</summary>
        public string UserName { get; set; }

        /// <summary>名字</summary>
        public string GivenName { get; set; }

        /// <summary>姓氏</summary>
        public string Surname { get; set; }

        /// <summary>角色</summary>
        public IDictionary<string, bool> Roles { get; set; }

        /// <summary>权限</summary>
        public IDictionary<string, bool> Privileges { get; set; }

    }

}
