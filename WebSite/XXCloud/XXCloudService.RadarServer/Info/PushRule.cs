using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using XCCloudService.BLL.CommonBLL;

namespace XXCloudService.RadarServer.Info
{
    public static class PushRule
    {
        class 游戏机投币规则
        {
            public string 游戏机编号;
            public int 会员级别;
            public bool 允许投币;
            public bool 允许退币;
            public int 周;
            public int 扣币数量;
            public int 优先级;
            public DateTime 开始时间;
            public DateTime 结束时间;
        }

        public class 当前游戏机投币规则
        {
            public string 游戏机编号;
            public int 会员级别;
            public bool 允许退币;
            public bool 允许投币;
            public bool 是否开启卡片专卡专用;
            public int 扣币数量;
        }

        public class 送分规则
        {
            public string 规则编号 = "";
            public string 游戏机编号 = "";
            public string 会员级别 = "";
            public DateTime 开始时间;
            public DateTime 结束时间;
            public int 扣币数;
            public int 送币数;
            public int 最小退币数;
            public string 启用状态;
        }

        static List<游戏机投币规则> 游戏机投币规则列表 = new List<游戏机投币规则>();
        public static List<当前游戏机投币规则> 当前规则 = new List<当前游戏机投币规则>();
        public static List<送分规则> 当前送分规则 = new List<送分规则>();

        //初始化用户送分券
        public static void InitUserFreeRule()
        {
            //DataAccess ac = new DataAccess();
            //DataTable dt = ac.ExecuteQueryReturnTable("select * from flw_game_free");
            //foreach (DataRow row in dt.Rows)
            //{
            //    Info.CoinInfo.更新送分券信息(row["RuleID"].ToString(), row["ICCardID"].ToString());
            //}
        }

        public static void RefreshFreeRule()
        {
            try
            {
                //DataAccess ac = new DataAccess();
                当前送分规则 = new List<送分规则>();
                //DataTable dt = ac.ExecuteQueryReturnTable("select * from t_game_free_rule where GETDATE() BETWEEN StartTime and EndTime and State='启用'");
                DataTable dt = XCCloudRS232BLL.ExecuterSqlToTable("select * from t_game_free_rule where GETDATE() BETWEEN StartTime and EndTime and State='启用'");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        送分规则 gz = new 送分规则();
                        gz.规则编号 = row["ID"].ToString();
                        gz.会员级别 = row["MemberLevelID"].ToString();
                        gz.结束时间 = Convert.ToDateTime(row["EndTime"]);
                        gz.开始时间 = Convert.ToDateTime(row["StartTime"]);
                        gz.扣币数 = Convert.ToInt32(row["NeedCoin"]);
                        gz.启用状态 = row["State"].ToString();
                        gz.送币数 = Convert.ToInt32(row["FreeCoin"]);
                        gz.游戏机编号 = row["GameID"].ToString();
                        gz.最小退币数 = Convert.ToInt32(row["ExitCoin"]);

                        var r = 当前送分规则.Where(p => p.规则编号 == row["ID"].ToString());
                        if (r.Count() > 0)
                        {
                            送分规则 gz1 = new 送分规则();
                            r.First().会员级别 = gz.会员级别;
                            r.First().结束时间 = gz.结束时间;
                            r.First().开始时间 = gz.开始时间;
                            r.First().扣币数 = gz.扣币数;
                            r.First().启用状态 = gz.启用状态;
                            r.First().送币数 = gz.送币数;
                            r.First().游戏机编号 = gz.游戏机编号;
                            r.First().最小退币数 = gz.最小退币数;
                        }
                        else
                        {
                            当前送分规则.Add(gz);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
            }
        }

        public static void InitRule()
        {
            try
            {
                //DataAccess ac = new DataAccess();
                游戏机投币规则列表 = new List<游戏机投币规则>();
                //DataTable dt = ac.ExecuteQueryReturnTable("select * from t_push_rule");
                DataTable dt = XCCloudRS232BLL.ExecuterSqlToTable("select * from t_push_rule");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        游戏机投币规则 rule = new 游戏机投币规则();
                        rule.游戏机编号 = r["game_id"].ToString();
                        rule.会员级别 = Convert.ToInt32(r["member_level_id"]);
                        rule.结束时间 = Convert.ToDateTime(DateTime.Now.ToLongDateString() + " " + Convert.ToDateTime(r["end_time"]).ToString("HH:mm:ss"));
                        rule.开始时间 = Convert.ToDateTime(DateTime.Now.ToLongDateString() + " " + Convert.ToDateTime(r["begin_time"]).ToString("HH:mm:ss"));
                        rule.扣币数量 = Convert.ToInt32(r["coin"]);
                        rule.优先级 = Convert.ToInt32(r["level"]);
                        rule.允许退币 = (r["allow_out"].ToString() == "是");
                        rule.允许投币 = (r["allow_in"].ToString() == "是");
                        rule.周 = Convert.ToInt32(r["week"]);

                        游戏机投币规则列表.Add(rule);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
            }
        }

        public static void RefreshRule(DateTime d)
        {
            try
            {
                //InitRule();

                //RefreshFreeRule();
                //DataAccess ac = new DataAccess(); //****
                //DataTable mind = ac.ExecuteQueryReturnTable("select r.ExitCoin,f.HeadID from flw_game_free f,t_game_free_rule r where f.RuleID=r.ID ORDER BY RealTime DESC");
                //foreach (HeadInfo.机头信息 head in HeadInfo.GetAllHead())
                //{
                //    DataRow r = mind.Select("HeadID='" + head.常规.机头编号 + "'").FirstOrDefault() as DataRow;
                //    if (r != null)
                //    {
                //        head.投币.最小退币数 = Convert.ToInt32(mind.Rows[0][0].ToString());
                //    }
                //}

                //DataTable dtLevel = ac.ExecuteQueryReturnTable("select * from t_memberlevel");
                //DataTable dtGame = ac.ExecuteQueryReturnTable("select * from Data_GameInfo");
                DataTable dtGame = XCCloudRS232BLL.ExecuterSqlToTable("select * from Data_GameInfo");
                int week = (int)d.DayOfWeek;
                foreach (DataRow grow in dtGame.Rows)
                {
                    string gameID = grow["GroupID"].ToString();
                    bool gIn, gOut, lIn, lOut, gz, lz;

                    gIn = (grow["AllowElecPush"].ToString() == "1");
                    gOut = (grow["AllowElecOut"].ToString() == "1");
                    lIn = true;
                    lOut = (grow["AllowElecOut"].ToString() == "1");
                    gz = (grow["GuardConvertCard"].ToString() == "1");
                    lz = false;

                    if (游戏机投币规则列表.Where(p => p.游戏机编号 == gameID).Count() > 0)
                    {
                        var rlist = 游戏机投币规则列表.Where(p => p.游戏机编号 == gameID && p.周 == week && Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + p.开始时间.ToString("HH:mm:ss")) <= d && Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + p.结束时间.ToString("HH:mm:ss")) >= d).OrderByDescending(p => p.优先级);
                        if (rlist.Count() > 0)
                        {
                            游戏机投币规则 rule = rlist.First();
                            SetCurRule(gameID, rule.扣币数量, (gOut && lOut && rule.允许退币), (gIn && lIn && rule.允许投币), (gz || lz));
                        }
                        else
                        {
                            //当前游戏机没有投币规则则默认继承游戏机参数
                            if (grow["UseSecondPush"].ToString() == "1")
                            {
                                //启用第二路上分信号
                                SetCurRule(gameID, Convert.ToInt32(grow["SecondReduceFromCard"]), Convert.ToInt32(grow["AllowElecOut"]) == 1, grow["AllowElecPush"].ToString() == "1", (gz || lz));
                            }
                            else
                            {
                                SetCurRule(gameID, Convert.ToInt32(grow["PushReduceFromCard"]), Convert.ToInt32(grow["AllowElecOut"]) == 1, grow["AllowElecPush"].ToString() == "1", (gz || lz));
                            }
                        }
                    }
                    else
                    {
                        //当前游戏机没有投币规则则默认继承游戏机参数
                        if (grow["UseSecondPush"].ToString() == "1")
                        {
                            //启用第二路上分信号
                            SetCurRule(gameID, Convert.ToInt32(grow["SecondReduceFromCard"]), lOut, lIn, (gz || lz));
                        }
                        else
                        {
                            SetCurRule(gameID, Convert.ToInt32(grow["PushReduceFromCard"]), lOut, lIn, (gz || lz));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
            }
        }
        static void SetCurRule(string gameID, int Coin, bool AllowOut, bool AllowIn, bool AllowZKZY)
        {
            try
            {
                var clist = 当前规则.Where(p => p.游戏机编号 == gameID);
                if (clist.Count() == 0)
                {
                    当前游戏机投币规则 cRule = new 当前游戏机投币规则();
                    cRule.扣币数量 = Coin;
                    cRule.游戏机编号 = gameID;
                    cRule.允许退币 = AllowOut;
                    cRule.允许投币 = AllowIn;
                    cRule.是否开启卡片专卡专用 = AllowZKZY;

                    当前规则.Add(cRule);
                }
                else
                {
                    当前游戏机投币规则 cRule = clist.First();
                    cRule.允许退币 = AllowOut;
                    cRule.允许投币 = AllowIn;
                    cRule.扣币数量 = Coin;
                    cRule.是否开启卡片专卡专用 = AllowZKZY;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
            }
        }
        public static bool GetCurRule(string gameID, out int Coin, out bool AllowOut, out bool AllowIn, out bool AllowZKZY)
        {
            Coin = 0;
            AllowOut = false;
            AllowIn = false;
            AllowZKZY = false;

            try
            {
                var clist = 当前规则.Where(p => p.游戏机编号 == gameID);
                if (clist.Count() > 0)
                {
                    当前游戏机投币规则 cRule = clist.First();
                    Coin = cRule.扣币数量;
                    AllowOut = cRule.允许退币;
                    AllowIn = cRule.允许投币;
                    AllowZKZY = cRule.是否开启卡片专卡专用;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
            }
            return false;
        }

        public static string ShowRule(string hAddress, string rAddress)
        {
            //try
            //{
            //    StringBuilder sb = new StringBuilder();
            //    HeadInfo.机头信息 head = HeadInfo.GetHeadInfo(rAddress, hAddress);
            //    var clist = 当前规则.Where(p => p.游戏机编号 == head.常规.游戏机编号);
            //    if (clist.Count() > 0)
            //    {
            //        foreach (当前游戏机投币规则 cRule in clist)
            //        {
            //            sb.AppendFormat("会员级别：{0}\r\n", cRule.会员级别);
            //            sb.AppendFormat("扣币数量：{0}\r\n", cRule.扣币数量);
            //            sb.AppendFormat("允许退币：{0}\r\n", cRule.允许退币);
            //            sb.AppendFormat("允许投币：{0}\r\n", cRule.允许投币);
            //            sb.AppendFormat("是否开启卡片专卡专用：{0}\r\n", cRule.是否开启卡片专卡专用);
            //        }
            //    }
            //    return sb.ToString();
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.WriteLog(ex);
            //}
            return "";
        }

        public static bool 是否使用限时送分优惠(string hAddress, CoinInfo.会员卡信息 member, int Coins, out int freeCoin, out int ID)
        {
            ID = 0;
            freeCoin = 0;
            //try
            //{
            //    HeadInfo.机头信息 h = HeadInfo.GetHeadInfo(PubLib.路由器段号, hAddress);
            //    if (h != null)
            //    {
            //        DateTime curD = DateTime.Now;
            //        var frees = 当前送分规则.Where(p => p.会员级别 == member.会员级别.ToString() && p.开始时间 <= curD && p.结束时间 >= curD && p.扣币数 == Coins && p.游戏机编号.IndexOf(h.常规.游戏机编号) >= 0);
            //        if (frees.Count() > 0)
            //        {
            //            ID = Convert.ToInt32(frees.First().规则编号);
            //            DataTable d = DataAccess.ExecuteQueryReturnTable(string.Format("select count(id) as c from flw_game_free where ruleid='{0}' and iccardid='{1}';", ID, member.会员卡号));
            //            if (d.Rows.Count > 0)
            //            {
            //                //判断是否使用过此规则
            //                if (d.Rows[0][0].ToString() != "0")
            //                {
            //                    return false;
            //                }
            //            }
            //            h.投币.最小退币数 = frees.First().最小退币数;
            //            freeCoin = frees.First().送币数;
            //            return true;
            //        }
            //    }
            //}
            //catch
            //{
            //    throw;
            //}
            return false;
        }
    }
}
