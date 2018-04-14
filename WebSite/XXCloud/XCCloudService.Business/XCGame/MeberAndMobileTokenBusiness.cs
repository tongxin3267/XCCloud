using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Business.Common;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCGame;

namespace XCCloudService.Business.XCGame
{
    public class MeberAndMobileTokenBusiness
    {
        public static bool GetTokenData(Dictionary<string, object> dicParas, out TokenType tokenType, ref MobileTokenModel mobileTokenModel, ref XCGameMemberTokenModel memberTokenModel, out string errMsg)
        {
            //获取token模式
            errMsg = string.Empty;
            memberTokenModel = Utils.GetDictionaryValue<XCGameMemberTokenModel>(dicParas, Constant.XCGameMemberTokenModel) as XCGameMemberTokenModel;
            mobileTokenModel = Utils.GetDictionaryValue<MobileTokenModel>(dicParas, Constant.MobileTokenModel) as MobileTokenModel;
            tokenType = TokenType.Member;
            if (memberTokenModel != null && mobileTokenModel != null)
            {
                tokenType = TokenType.MemberAndMobile;
                return true;
            }
            else if (memberTokenModel == null && mobileTokenModel != null)
            {
                tokenType = TokenType.Mobile;
                return true;
            }
            else if (memberTokenModel != null && mobileTokenModel == null)
            {
                tokenType = TokenType.Member;
                return true;
            }
            else
            {
                errMsg = "手机令牌或会员令牌无效";
                return false;
            }
        }

        public static bool GetTokenData(Dictionary<string, object> dicParas, out TokenType tokenType, out string mobile, out string errMsg)
        {
            //获取token模式
            errMsg = string.Empty;
            mobile = string.Empty;
            XCGameMemberTokenModel memberTokenModel = Utils.GetDictionaryValue<XCGameMemberTokenModel>(dicParas, Constant.XCGameMemberTokenModel) as XCGameMemberTokenModel;
            MobileTokenModel mobileTokenModel = Utils.GetDictionaryValue<MobileTokenModel>(dicParas, Constant.MobileTokenModel) as MobileTokenModel;
            tokenType = TokenType.Member;
            if (memberTokenModel != null && mobileTokenModel != null)
            {
                tokenType = TokenType.MemberAndMobile;
                if (!memberTokenModel.Mobile.Equals(mobileTokenModel.Mobile))
                {
                    errMsg = "手机令牌或会员令牌无效";
                    return false;
                }
                return true;
            }
            else if (memberTokenModel == null && mobileTokenModel != null)
            {
                tokenType = TokenType.Mobile;
                mobile = mobileTokenModel.Mobile;
                return true;
            }
            else if (memberTokenModel != null && mobileTokenModel == null)
            {
                tokenType = TokenType.Member;
                mobile = memberTokenModel.Mobile;
                return true;
            }
            else
            {
                errMsg = "手机令牌或会员令牌无效";
                return false;
            }
        }

    }
}
