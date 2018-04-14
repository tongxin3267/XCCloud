using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCCloudService.Base
{
    public class ApiMethodAttribute:System.Attribute
    {
        public ApiMethodAttribute()
        {
            this.SignKeyEnum = SignKeyEnum.DogNoToken;
            this.RespDataTypeEnum = RespDataTypeEnum.Json;
            this.SysIdAndVersionNo = true;
            this.IconOutputLock = true;
        }
        /// <summary>
        /// 验证方式
        /// </summary>
        public SignKeyEnum SignKeyEnum { set; get; }
        /// <summary>
        /// 返回的数据类型
        /// </summary>
        public RespDataTypeEnum RespDataTypeEnum { set; get; }

        public bool SysIdAndVersionNo { set; get; }

        public bool IconOutputLock { set; get; }
    }
}