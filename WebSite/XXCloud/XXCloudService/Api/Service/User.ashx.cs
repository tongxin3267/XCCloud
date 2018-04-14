using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Business.XCGameMana;
using XCCloudService.CacheService;
using XCCloudService.Model.XCGameManager;

namespace XXCloudService.Api.Service
{
    /// <summary>
    /// User 的摘要说明
    /// </summary>
    public class User : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken, SysIdAndVersionNo = false)]
        public object login(Dictionary<string, object> dicParas)
        {
            string userMobile = dicParas.ContainsKey("userMobile") ? dicParas["userMobile"].ToString() : string.Empty;
            string password = dicParas.ContainsKey("password") ? dicParas["password"].ToString() : string.Empty;
            string imgCode = dicParas.ContainsKey("imgCode") ? dicParas["imgCode"].ToString() : string.Empty;

            //验证码
            if (!ValidateImgCache.Exist(imgCode.ToUpper()))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "验证码无效");
            }
            ValidateImgCache.Remove(imgCode.ToUpper());

            IAdminUserService adminUserService = BLLContainer.Resolve<IAdminUserService>();
            var model = adminUserService.GetModels(p => p.Mobile.Equals(userMobile)).FirstOrDefault<t_AdminUser>();
            if (model == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户不存在");
            }
            else
            {
                if (model.Password.Equals(password))
                {
                    string token = XCGameManaAdminUserTokenBusiness.SetToken(model.Mobile, model.Id);
                    var obj = new { token = token };
                    return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "密码不正确");
                }
            }
        }
    }
}