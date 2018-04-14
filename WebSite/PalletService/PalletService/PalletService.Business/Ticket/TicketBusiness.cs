
using DeviceUtility.Utility.Print;
using PalletService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace PalletService.Business.Ticket
{
    public class TicketBusiness
    {
        public static Dictionary<string, object> GetTicketParams(string ticketType, Dictionary<string, object> data)
        {
            switch (Convert.ToInt16(ticketType))
            {

                case (int)(TicketEnum.FoodSale):
                    return GetFoodSaleTicket(data);
                case (int)(TicketEnum.member):
                    return GetMemberTicket(data);
                case (int)(TicketEnum.Recharge):
                    return GetRechargeTicket(data);
                case (int)(TicketEnum.ExitCoin):
                    return GetExitCoinTicket(data);
                case (int)(TicketEnum.Fee):
                    return GetFree(data);
                case (int)(TicketEnum.Save):
                    return GetSave(data);
                case (int)(TicketEnum.Currency):
                    return GetCurrency(data);
                case (int)(TicketEnum.Point):
                    return GetPoint(data);
                case (int)(TicketEnum.Goods):
                    return GetGoods(data);
                case (int)(TicketEnum.TicketInfo):
                    return GetTicketInfo(data);
                    
                default: return null;
            }
        }

        private static Dictionary<string, object> GetFoodSaleTicket(Dictionary<string, object> data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            StringBuilder content = new StringBuilder();
            content.AppendLine(string.Format("套餐名称\t{0}", data["foodName"].ToString()));
            content.AppendLine(string.Format("序列号\t\t{0}", data["no"].ToString()));
            content.AppendLine(string.Format("有效期\t\t{0}", data["validityDate"].ToString()));
            content.AppendLine(string.Format("消费币数\t{0}", data["coins"].ToString()));
            dict.Add(PrintBlock.Title, "套餐销售");
            dict.Add(PrintBlock.Content, content.ToString());
            return dict;
        }
        private static Dictionary<string, object> GetMemberTicket(Dictionary<string, object> data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            StringBuilder content = new StringBuilder();
            content.AppendLine(string.Format("会员级别\t{0}", data["memberlenvel"].ToString()));
            content.AppendLine(string.Format("IC卡号\t\t{0}", data["iccard"].ToString()));
            content.AppendLine(string.Format("剩余币数\t{0}", data["coins"].ToString()));
            content.AppendLine(string.Format("剩余积分\t{0}", data["Integral"].ToString())); 
            dict.Add(PrintBlock.Title, "会员信息");
            dict.Add(PrintBlock.Content, content.ToString());
            return dict;
        }
        private static Dictionary<string, object> GetRechargeTicket(Dictionary<string, object> data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            StringBuilder content = new StringBuilder();
            content.AppendLine(string.Format("充值金额\t{0}", data["rechargemoeney"].ToString()));
            content.AppendLine(string.Format("充值币数\t{0}", data["rechargecoins"].ToString()));
            content.AppendLine(string.Format("充值时间\t{0}", data["rechargetime"].ToString()));
            dict.Add(PrintBlock.Title, "充值");
            dict.Add(PrintBlock.Content, content.ToString());
            return dict;
        }
        private static Dictionary<string, object> GetExitCoinTicket(Dictionary<string, object> data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            StringBuilder content = new StringBuilder();
            content.AppendLine(string.Format("兑币\t\t{0}", data["moneyexchange"].ToString()));
            content.AppendLine(string.Format("金额\t\t{0}", data["money"].ToString()));
            content.AppendLine(string.Format("当前余额\t{0}", data["currentmoney"].ToString()));
            dict.Add(PrintBlock.Title, "退币业务");
            dict.Add(PrintBlock.Content, content.ToString());
            return dict;
        }
        private static Dictionary<string, object> GetFree(Dictionary<string, object> data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            StringBuilder content = new StringBuilder();
            content.AppendLine(string.Format("赠送币数\t{0}", data["coins"].ToString()));
            content.AppendLine(string.Format("原有币数\t{0}", data["feecoins"].ToString()));
            content.AppendLine(string.Format("赠送方式\t{0}", data["feemode"].ToString()));
            dict.Add(PrintBlock.Title, "送币业务");
            dict.Add(PrintBlock.Content, content.ToString());
            return dict;
        }
        private static Dictionary<string, object> GetSave(Dictionary<string, object> data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            StringBuilder content = new StringBuilder();
            content.AppendLine(string.Format("原有币数\t{0}", data["coins"].ToString()));
            content.AppendLine(string.Format("存币数\t\t{0}", data["savecoins"].ToString()));
            dict.Add(PrintBlock.Title, "存币业务");
            dict.Add(PrintBlock.Content, content.ToString());
            return dict;
        }

        private static Dictionary<string, object> GetCurrency(Dictionary<string, object> data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            StringBuilder content = new StringBuilder();
            content.AppendLine(string.Format("原有币数\t{0}", data["coins"].ToString()));
            content.AppendLine(string.Format("提取币数\t{0}", data["currencycoins"].ToString()));
            dict.Add(PrintBlock.Title, "提币业务");
            dict.Add(PrintBlock.Content, content.ToString());
            return dict;
        }

        private static Dictionary<string, object> GetPoint(Dictionary<string, object> data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            StringBuilder content = new StringBuilder();
            content.AppendLine(string.Format("消耗积分\t{0}", data["consumecoins"].ToString()));
            content.AppendLine(string.Format("兑换币数\t{0}", data["exchangecoins"].ToString()));
            content.AppendLine(string.Format("原有币数\t{0}", data["coins"].ToString()));
            dict.Add(PrintBlock.Title, "积分换币");
            dict.Add(PrintBlock.Content, content.ToString());
            return dict;
        }
        private static Dictionary<string, object> GetGoods(Dictionary<string, object> data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            StringBuilder content = new StringBuilder();
            content.AppendLine("商品名称" + "\t" + "数量" + "\t" + "金额");
            content.AppendLine(string.Format("{0}"+"\t\t"+"{1}"+"\t"+"{2}", data["commodityname"].ToString(), data["number"].ToString(), data["money"].ToString()));
            content.AppendLine("------------------------------");           
            content.AppendLine(string.Format("合计金额\t￥{0}", data["totalmoney"].ToString()));
            content.AppendLine(string.Format("支付方式\t{0}", data["paymentmethod"].ToString()));
            content.AppendLine("------------------------------");
            dict.Add(PrintBlock.Title, "销售业务");
            dict.Add(PrintBlock.Content, content.ToString());
            return dict;
        }
        private static Dictionary<string, object> GetTicketInfo(Dictionary<string, object> data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            StringBuilder content = new StringBuilder();

            content.AppendLine(string.Format("【第{0}张门票】", data["index"].ToString()));
            content.AppendLine(string.Format("购买门票ID\t{0}", data["ticketid"].ToString()));
            content.AppendLine(string.Format("购买门票名\t{0}", data["ticketname"].ToString()));
            content.AppendLine(string.Format("购买数量\t{0}", data["ticketnumber"].ToString()));
            content.AppendLine(string.Format("金额￥\t\t{0}", data["money"].ToString()));
            content.AppendLine(string.Format("有效期至\t{0}", data["tickettime"].ToString()));
            content.AppendLine("------------------------------");
            dict.Add(PrintBlock.Title, "门票销售");
            dict.Add(PrintBlock.Content, content.ToString());
            return dict;
        }
        
    }
}
