using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using XXCloudService.RadarServer.Command.Ask;
using System.Data;

namespace XXCloudService.RadarServer.Info
{
    public class MemberInfo
    {
        //public bool IC卡查询(string ICCardID, string hAddress, int RepeadCode, out int Balance, out int Coins, out bool isAllowIn, out bool isAllowOut, out bool isAllowZKZY, out byte cardType, out IC卡权限结构 权限)
        //{
        //    Balance = 0;
        //    Coins = 0;
        //    cardType = 0;
        //    isAllowIn = false;
        //    isAllowOut = false;
        //    isAllowZKZY = false;
        //    权限 = new IC卡权限结构();

        //    DataAccess ac = new DataAccess();
        //    string sql = string.Format("select ICCardID, MemberLevelID,Lock,Lottery ,HasUpDownCoin, RepeatCode from t_member where ICCardID='{0}' and RepeatCode='{1}' and MemberState=1 and EndDate>=CONVERT(varchar(10),GETDATE(),120)", ICCardID, RepeadCode);
        //    DataTable dt = ac.ExecuteQueryReturnTable(sql);

        //    HeadInfo.机头信息 head = HeadInfo.GetHeadInfo(PubLib.路由器段号, hAddress);
        //    if (head == null) return false;
        //    //判断是否为员工卡
        //    string sql = string.Format("select * from u_users where ICCardID='{0}' and State=1", ICCardID);
        //    DataTable dt = DataAccess.ExecuteQueryReturnTable(sql);
        //    if (dt.Rows.Count == 0)
        //    {
        //        head.状态.超级解锁卡标识 = false;
        //        //查询是否为会员卡
        //        if (head.类型 == Info.HeadInfo.设备类型.碎票机)
        //        {
        //            sql = string.Format("select ICCardID, MemberLevelID,Lock,Lottery ,HasUpDownCoin, RepeatCode from t_member where ICCardID='{0}' and RepeatCode='{1}' and MemberState=1 and EndDate>=CONVERT(varchar(10),GETDATE(),120)", ICCardID, RepeadCode);
        //            dt = DataAccess.ExecuteQueryReturnTable(sql);
        //            if (dt.Rows.Count > 0)
        //            {
        //                DataRow row = dt.Rows[0];
        //                Balance = Convert.ToInt32(row["Lottery"].ToString());


        //                PushRule.GetCurRule(head.常规.游戏机编号, Convert.ToInt32(row["MemberLevelID"].ToString()), out Coins, out isAllowOut, out isAllowIn, out isAllowZKZY);

        //                if (row["HasUpDownCoin"].ToString() == "0")
        //                {
        //                    isAllowOut = false;
        //                    isAllowIn = false;
        //                }

        //                Init(ICCardID);

        //                cardType = 1;
        //            }
        //            else
        //            {
        //                cardType = 0;
        //            }
        //        }
        //        else
        //        {
        //            sql = string.Format("select ICCardID, MemberLevelID,Lock,Balance ,HasUpDownCoin, RepeatCode from t_member where ICCardID='{0}' and RepeatCode='{1}' and MemberState=1 and EndDate>=date_format(GETDATE(),'%Y-%m-%d')", ICCardID, RepeadCode);
        //            sql = string.Format("select ICCardID, MemberLevelID,Lock,Balance ,HasUpDownCoin, RepeatCode from t_member where ICCardID='{0}' and RepeatCode='{1}' and MemberState=1 and EndDate>=GETDATE()", ICCardID, RepeadCode);
        //            dt = DataAccess.ExecuteQueryReturnTable(sql);
        //            if (dt.Rows.Count > 0)
        //            {
        //                //当前卡有效
        //                sql = string.Format("SELECT b.ID,l.RemainCount FROM t_game_project gp,t_head h,flw_project_buy_codelist l,flw_project_buy b where gp.GameID=h.GameID and gp.ProjectID=l.ProjectID and l.BuyID=b.ID and l.EndTime>GETDATE() and l.RemainCount>0 and h.HeadAddress='{0}' and h.Segment='{1}' and b.ICCardID='{2}'", hAddress, PubLib.路由器段号, ICCardID);
        //                DataTable subdt = DataAccess.ExecuteQueryReturnTable(sql);
        //                if (subdt.Rows.Count > 0)
        //                {
        //                    //有门票消费，显示门票余额
        //                    isAllowOut = false;
        //                    isAllowIn = true;
        //                    isAllowZKZY = false;
        //                    Balance = Convert.ToInt32(subdt.Rows[0]["RemainCount"].ToString());
        //                    Coins = 1;
        //                }
        //                else
        //                {
        //                    //正常扣币消费
        //                    DataRow row = dt.Rows[0];
        //                    PushRule.GetCurRule(head.常规.游戏机编号, Convert.ToInt32(row["MemberLevelID"].ToString()), out Coins, out isAllowOut, out isAllowIn, out isAllowZKZY);
        //                    if (row["HasUpDownCoin"].ToString() == "0")
        //                    {
        //                        isAllowOut = false;
        //                        isAllowIn = false;
        //                    }
        //                    Balance = Convert.ToInt32(row["Balance"].ToString());
        //                }
        //                Init(ICCardID);
        //                if (Balance >= 0)
        //                    cardType = 1;
        //                else
        //                {
        //                    sql = "update t_member set `Lock`=1 where iccardid='" + ICCardID + "'";
        //                    DataAccess.Execute(sql);
        //                    cardType = 0;
        //                }
        //            }
        //            else
        //            {
        //                cardType = 0;
        //            }
        //        }
        //        head.常规.管理卡号 = "";
        //    }
        //    else
        //    {
        //        head.常规.管理卡号 = dt.Rows[0]["ICCardID"].ToString();
        //        权限.bit0专卡专用解锁 = (dt.Rows[0]["HasUnlock"].ToString() == "1");
        //        权限.bit1退分锁定解锁 = (dt.Rows[0]["HasExitUnlock"].ToString() == "1");
        //        权限.bit7超级卡 = (dt.Rows[0]["UserName"].ToString().ToLower() == "admin");
        //        head.状态.超级解锁卡标识 = 权限.bit7超级卡;
        //        cardType = 2;
        //    }

        //    return true;
        //}
    }
}
