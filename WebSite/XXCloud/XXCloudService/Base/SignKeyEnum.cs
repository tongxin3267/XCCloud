using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCCloudService.Base
{
    /// <summary>
    /// 验证签名模式
    /// </summary>
    public enum SignKeyEnum
    {
        XCGameMemberToken = 2,//使用会员token做验签
        MobileToken = 3,//使用手机token做验签
        XCGameMemberOrMobileToken = 4,//使用会员token做验签或手机token做验签
        MethodToken = 5,//方法token(方法体内验证token)
        DogNoToken = 6, //使用狗号做验签
        XCGameUserCacheToken=7,//用户token
        XCCloudUserCacheToken = 8,//云运营平台用户token
        XCGameManaUserToken = 9,//用户token
        XCGameAdminToken = 10, //XCGameManaDB表管理权限
        XCGameManamAdminUserToken = 11, //XCGameManaDB表系统管理权限（t_AdminUser用户权限）
    }

    public enum RespDataTypeEnum
    { 
        Json = 0,
        ImgStream = 1
    }
}