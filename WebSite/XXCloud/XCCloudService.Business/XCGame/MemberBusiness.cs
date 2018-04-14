using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;

namespace XCCloudService.Business.XCGame
{
    public class MemberBusiness
    {
        public static bool IsEffectiveStore(string mobile, string xcGameDBName, ref XCCloudService.Model.XCGame.t_member memberModel, out string errMsg)
        {
            errMsg = string.Empty;
            XCCloudService.BLL.IBLL.XCGame.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IMemberService>(xcGameDBName);
            var model = memberService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGame.t_member>();
            if (model == null)
            {
                errMsg = "会员信息不存在";
                return false;
            }
            else if (model.MemberState != "1")
            {
                errMsg = "会员卡已" + GetMemberStateName(model.MemberState);
                return false;
            }
            else if (model.Lock == 1)
            {
                errMsg = "会员已被锁定";
                return false;
            }
            else
            {
                memberModel = model;
                return true;
            }
        }


        public static string GetMemberStateName(string state)
        {
            //0：注销，1：使用中，2：挂失
            switch (state)
            {
                case "0": return "注销";
                case "1": return "使用中";
                case "2": return "挂失";
                default: return "无效";
            }
        }
    }
}
