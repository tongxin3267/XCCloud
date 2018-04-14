using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Common.Enum;

namespace XCCloudService.Base
{
    public class AuthorizeAttribute:System.Attribute
    {
        public AuthorizeAttribute()
        {
            this.Roles = string.Empty;
            this.Users = string.Empty;
            this.Merches = string.Empty;
        }

        /// <summary>
        /// 角色
        /// </summary>
        public string Roles { set; get; }

        /// <summary>
        /// 用户
        /// </summary>
        public string Users { get; set; }

        /// <summary>
        /// 商户
        /// </summary>
        public string Merches { set; get; }

    }

    public class AllowAnonymousAttribute : System.Attribute
    {
        public AllowAnonymousAttribute()
        {
            
        }        

    }
}