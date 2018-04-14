using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CloudRS232Server.Command.Ask
{
    public class Ask卡头进出币数据应答
    {
        public byte 机头地址 { get; set; }
        public byte 控制类型 { get; set; }
        public Int16 脉冲数 { get; set; }
        public Int32 币余额 { get; set; }
        public byte 控制信号 { get; set; }
        public UInt16 流水号 { get; set; }

        public bool 退币报警锁机头 = false;

        public Ask卡头进出币数据应答(Info.HeadInfo.机头信息 head, string IC, int Coins, CoinType cType, byte checkCode, UInt16 SN, FrameData frame, bool isPush, string PushAddr, ref string msg)
        {
            try
            {
                Info.HeadInfo.IC卡进出币控制信号结构 信号 = new Info.HeadInfo.IC卡进出币控制信号结构();
                机头地址 = Convert.ToByte(head.常规.机头地址, 16);
                流水号 = SN;
                控制类型 = (byte)cType;

                #region 内存查询方法
                Info.HeadInfo.IC卡模式进出币应答结构 投币 = 卡头进出币(head, IC, Coins, SN, cType, checkCode, ref msg, isPush, PushAddr);
                if (投币 != null)
                {
                    信号.保留0当前卡是否允许上分 = 投币.机头能上分;
                    信号.保留1当前卡是否允许退分 = 投币.机头能打票;
                    信号.保留2是否启用卡片专卡专用功能 = 投币.是否启用卡片专卡专用;
                    信号.保留3超出当日机头最大净退币上线 = 投币.超出当日机头最大净退币上线;
                    信号.保留4是否将退币上回游戏机 = 投币.是否将退币上回游戏机;
                    信号.保留5是否正在使用限时送分优惠券 = 投币.是否正在使用限时送分优惠券;
                    币余额 = 投币.币余额;
                    脉冲数 = (short)投币.发脉冲数;
                }
                else
                {
                    LogHelper.WriteLog("IC卡进出币数据有误\r\n" + msg, frame.recvData);
                }
                #endregion

                控制信号 = PubLib.GetBitByObject(信号);
            }
            catch
            {
                //LogHelper.WriteLog("IC卡进出币数据有误\r\n" + msg, frame.recvData);
                throw;
            }
        }

        Info.HeadInfo.IC卡模式进出币应答结构 卡头进出币(Info.HeadInfo.机头信息 head, string ICCard, int Coins, UInt16 SN, CoinType cType, int RepeadCode, ref string msg, bool isPush, string PushAddr)
        {
            int gInCoin, gOutCoin, headCount;
            bool skip = false;
            msg = "";
            Info.HeadInfo.IC卡模式进出币应答结构 投币应答 = new Info.HeadInfo.IC卡模式进出币应答结构();
            投币应答.机头能打票 = false;
            投币应答.机头能上分 = false;
            投币应答.锁机头 = false;
            try
            {
                //LogHelper.WriteLog("测试调用：   客户类别=" + Info.SecrityHeadInfo.客户类别.ToString() + "  cType=" + cType.ToString() + "   type=" + type);

                switch (cType)
                {
                    case CoinType.电子投币:
                        if (!电子投币(head, Coins, ICCard, RepeadCode, ref 投币应答, ref msg, ref skip))
                        {
                            return 投币应答;
                        }
                        break;
                    case CoinType.电子退币:
                        if (!电子退币SQL(head, SN, Coins, ICCard, RepeadCode, cType, ref 投币应答, ref msg))
                        {
                            return 投币应答;
                        }
                        skip = true;
                        break;
                    //case CoinType.电子存币:
                    //case CoinType.会员卡提币:
                    //    if (!电子存提币(head, Coins, cType, ICCard, ref 投币应答))
                    //    {
                    //        return 投币应答;
                    //    }
                    //    break;
                    //case CoinType.电子碎票:
                    //    if (!电子碎票(head, Coins, cType, ICCard, ref 投币应答))
                    //    {
                    //        return 投币应答;
                    //    }
                    //    break;
                    //case CoinType.远程出币通知:
                    //    {
                    //        投币应答.机头能打票 = true;
                    //        投币应答.机头能上分 = true;
                    //        skip = true;
                    //        FrmMain.GetInterface.ControlFinish(XCSocketService.ActionEnum.出币, head.订单编号, "成功", Coins.ToString());
                    //        head.状态.出币机或存币机正在数币 = false;
                    //    }
                    //    break;
                    //default:
                    //    if (!其他进出币(head, Coins, cType))
                    //    {
                    //        return 投币应答;
                    //    }
                    //    投币应答.机头能打票 = true;
                    //    投币应答.机头能上分 = true;
                    //    break;
                }
                                
                //添加投币流水账
                //存币，提币，碎票不写入流水表，有单独的处理表
                //if (cType != CoinType.电子存币 && cType != CoinType.会员卡提币 && cType != CoinType.电子碎票 && cType != CoinType.远程存币通知 && !skip)
                //{
                //    TableMemory.flw_485_coin.Table t = new TableMemory.flw_485_coin.Table();
                //    if (isPush)
                //    {
                //        t.ICCardID = (99990000 + Convert.ToInt32(head.常规.机头地址, 16)).ToString();
                //        t.HeadAddress = PushAddr;
                //        t.Balance = 0;
                //    }
                //    else
                //    {
                //        t.ICCardID = ICCard;
                //        t.HeadAddress = head.常规.机头地址;
                //        t.Balance = 投币应答.币余额;
                //    }
                //    t.Coins = Coins;
                //    if (head.彩票模式 && cType == CoinType.电子退币)
                //        t.CoinType = ((int)CoinType.IC退彩票).ToString();
                //    else
                //        t.CoinType = ((int)cType).ToString();

                //    t.RealTime = DateTime.Now;
                //    t.Segment = rAddress;
                //    TableMemory.flw_485_coin.Add(t);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 投币应答;
        }

        bool 电子投币(Info.HeadInfo.机头信息 head, int Coins, string ICCard, int RepeadCode, ref Info.HeadInfo.IC卡模式进出币应答结构 投币应答, ref string msg, ref bool SkipWriteFlw)
        {
            bool res = false;
            int exitcoin = 0;
            msg = "";
            SkipWriteFlw = true;

            if (!head.状态.是否正在使用限时送分优惠)
            {
                //head.常规.当前卡片号 = ICCard;
                //Info.HeadInfo.WriteBufHead(head.常规.路由器编号, head.常规.机头地址, ICCard);
                //res = true;
                //DataTable t = DataAccess.ExecuteQueryReturnTable(string.Format("select * from t_member where iccardid='{0}' and RepeatCode='{1}' and MemberState=1 and Lock=0", ICCard, RepeadCode));
                //int balance = Convert.ToInt32(t.Rows[0]["Balance"]);
                ////if (balance < Coins) return false;  //余额不足
                //DataTable dt = DataAccess.ExecuteQueryReturnTable(string.Format("exec SFProc '{0}','{1}','{2}','{3}','{4}'", PubLib.路由器段号, head.常规.机头地址, ICCard, Coins, 1));
                //DataRow row = dt.Rows[0];
                //投币应答.币余额 = Convert.ToInt32(row["Balance"].ToString());
                //投币应答.超出当日机头最大净退币上线 = false;
                //投币应答.发脉冲数 = Convert.ToInt32(row["PulsCount"].ToString());
                //投币应答.机头能打票 = (row["AllowOut"].ToString() == "1");
                //投币应答.机头能上分 = (row["AllowIn"].ToString() == "1");
                //投币应答.是否将退币上回游戏机 = false;
                //投币应答.是否启用卡片专卡专用 = (row["AllowZKZY"].ToString() == "1");
                //int.TryParse(row["ExitCoinNum"].ToString(), out exitcoin);
                //head.投币.最小退币数 = exitcoin;
                //投币应答.是否正在使用限时送分优惠券 = (exitcoin != 0);
                //投币应答.锁机头 = false;
                //if (投币应答.发脉冲数 > 0)
                //{
                //    head.投币.投币数 += Coins;
                //    UIClass.总投币数 += Coins;
                //}

                //if (投币应答.是否正在使用限时送分优惠券)
                //    head.状态.是否正在使用限时送分优惠 = true;
            }
            return res;
        }

        bool 电子退币SQL(Info.HeadInfo.机头信息 head, UInt16 SN, int Coins, string ICCard, int RepeadCode, CoinType cType, ref Info.HeadInfo.IC卡模式进出币应答结构 投币应答, ref string msg)
        {
            bool res = false;
            int pCoin = 0, bValue = 0;
            bool isAllowOut = true, isAllowIn = true, isAllowZKZY = false;
            bool 是否超分报警 = false;

            Info.CoinInfo coin = new Info.CoinInfo();
            Info.CoinInfo.会员卡信息 member = coin.GetMemberInfo(ICCard, head.常规.路由器编号);
            if (member == null) return res;
            member.退币时间 = DateTime.Now;

            DataAccess ac = new DataAccess();

            string sqlString = "";

            if (head.彩票模式 && cType == CoinType.电子退币)
                cType = CoinType.IC退彩票;

            Info.PushRule.GetCurRule(head.常规.游戏机编号, out pCoin, out isAllowOut, out isAllowIn, out isAllowZKZY);

            //启用退币保护，并且退币数误差在设定值正负1个范围内则触发事件
            if (head.常规.退币保护启用标志 && head.常规.退币信号超时退币个数 - 1 <= Coins && head.常规.退币信号超时退币个数 + 1 >= Coins)
            {
                msg += "触发退币保护功能\r\n";
                if (Info.GameInfo.TBProtect(head.常规.游戏机编号))
                {
                    member.机头上下分 = false;
                    退币报警锁机头 = true;
                    //Command.Ask.Ask机头锁定解锁指令 a = new Command.Ask.Ask机头锁定解锁指令(head.常规.机头地址, true);
                    sqlString = string.Format("exec TBProc '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}'",
                    head.常规,
                    head.常规.机头地址,
                    SN,
                    Coins,
                    ICCard,
                    (int)cType,
                    1,  //锁用户
                    1,  //退币保护
                    0,  //超分
                    0,  //是否添加流水
                    0); //彩票模式
                    ac.Execute(sqlString);
                    return false;
                }
            }
            else
            {
                Info.GameInfo.ClearTimeoutCount(head.常规.游戏机编号, head.常规.退币保护启用标志);
            }

            if (!head.状态.是否正在使用限时送分优惠)  //正常退币
            {
                head.常规.当前卡片号 = "";
                head.常规.是否为首次投币 = true;
                bValue += Coins;
                投币应答.锁机头 = false;
                投币应答.机头能上分 = isAllowIn;
                投币应答.机头能打票 = isAllowOut;
                msg += "正常退币数据\r\n";
                if ((0 - head.投币.盈利数 > head.投币.每天净退币上限 || Coins > head.投币.单次退币上限) && !head.状态.是否忽略超分报警 && !head.彩票模式)
                {
                    投币应答.锁机头 = true;
                    投币应答.超出当日机头最大净退币上线 = true;
                    msg += "触发超额报警\r\n";
                    是否超分报警 = true;
                }
                else
                {
                    投币应答.锁机头 = false;
                    投币应答.超出当日机头最大净退币上线 = false;
                }
                投币应答.币余额 = bValue;
                res = true;
            }
            else   //当前机头正在使用限时送分，则要判断最小退币数
            {
                if (head.投币.最小退币数 > Coins)
                {
                    //不满足退币条件
                    投币应答.币余额 = member.币余额;
                    投币应答.机头能上分 = isAllowIn;
                    投币应答.机头能打票 = isAllowOut;
                    int 倍数 = Coins / pCoin;
                    投币应答.发脉冲数 = (head.开关.启用第二路上分信号) ? 倍数 * head.参数.第二路上分线投币时给游戏机信号数 : 倍数 * head.参数.投币时给游戏机信号数;
                    投币应答.是否将退币上回游戏机 = true;
                    投币应答.是否正在使用限时送分优惠券 = true;
                    msg += "限时送分中不满足退币条件\r\n";
                    return false;
                }
                else
                {
                    //满足退币条件
                    head.常规.当前卡片号 = "";
                    head.常规.是否为首次投币 = true;
                    head.投币.退币数 += Coins;
                    head.状态.是否正在使用限时送分优惠 = false;
                    bValue += Coins;
                    投币应答.是否正在使用限时送分优惠券 = false;
                    投币应答.币余额 = bValue;
                    res = true;
                    msg += "限时送分中满足退币条件\r\n";
                }
            }


            sqlString = string.Format("exec TBProc '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}'",
                    head.常规.路由器编号,
                    head.常规.机头地址,
                    SN,
                    Coins,
                    ICCard,
                    (int)cType,
                    ((bValue > PubLib.会员余额上限 || member.退币数 - member.投币数 > PubLib.会员净退币数上限) && head.彩票模式) ? 1 : 0,
                    0,
                    是否超分报警 ? 1 : 0,
                    res ? 1 : 0,
                    head.彩票模式 ? 1 : 0);

            DataTable dt = ac.ExecuteQueryReturnTable(sqlString);
            DataRow row = dt.Rows[0];
            //有效电子投币
            if (head.彩票模式)
                bValue = Convert.ToInt32(row["lottery"].ToString());
            else
                bValue = Convert.ToInt32(row["balance"].ToString());
            投币应答.币余额 = bValue;
            投币应答.是否启用卡片专卡专用 = (row["LockHead"].ToString() == "1");
            投币应答.是否正在使用限时送分优惠券 = head.状态.是否正在使用限时送分优惠;
            msg += "找到会员信息\r\n";

            if (head.彩票模式)
            {
                member.锁会员 = false;
                member.票余额 = bValue;
            }
            else
            {
                member.币余额 = bValue;
                if (bValue > PubLib.会员余额上限 || member.退币数 - member.投币数 > PubLib.会员净退币数上限)
                {
                    member.锁会员 = true;
                    msg += "写入新余额并锁定会员\r\n";
                }
                else
                {
                    member.锁会员 = false;
                    msg += "写入新余额\r\n";
                }
            }

            member.Update();

            return res;
        }
    }
}
