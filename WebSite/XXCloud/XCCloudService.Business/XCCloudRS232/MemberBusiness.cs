using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;
using XCCloudService.Model.XCCloudRS232;

namespace XCCloudService.Business.XCCloudRS232
{
    public class MemberBusiness
    {
        public static List<t_member> merchList = new List<t_member>();

        /// <summary>
        /// 获取会员列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Model.XCCloudRS232.t_member> GetMemberList()
        {
            BLL.IBLL.XCCloudRS232.IMemberService memberService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IMemberService>();
            var memberList = memberService.GetModels(d => true);
            return memberList;
        }

        /// <summary>
        /// 根据IC卡号获取会员实体
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Model.XCCloudRS232.t_member GetMerchModel(string icCardId)
        {
            return GetMemberList().FirstOrDefault(m => m.ICCardID.ToString().Equals(icCardId));
        }
    }
}
