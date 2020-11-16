using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.Slpk.Models {

    /// <summary>slpk 航拍模型模型</summary>
    public partial class SlpkModel : StringEntity {

        /// <summary>航拍模型目录</summary>
        [Required(ErrorMessage = "航拍模型目录 必须填写！")]
        public string Directory { get; set; }
        /// <summary>标签/别名</summary>
        public string[] Tags { get; set; }
        /// <summary>模型经度</summary>
        public virtual double Longitude { get; set; }
        /// <summary>模型纬度</summary>
        public virtual double Latitude { get; set; }
        /// <summary>模型海拔高度</summary>
        public virtual double Elevation { get; set; }

    }

    /// <summary>slpk 航拍模型搜索参数</summary>
    public partial class SlpkSearchModel : PaginatedRequestModel {
        public string Keywords { get; set; }
    }

}
