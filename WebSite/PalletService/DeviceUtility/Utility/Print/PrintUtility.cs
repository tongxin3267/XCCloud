using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DeviceUtility.Utility.Print
{
    public  class PrintUtility
    {
        private Dictionary<string, object> printParams = new Dictionary<string, object>();
        private string printerName = string.Empty;
        public PrintUtility(Dictionary<string, object> printParams,string printerName)
        {
            this.printParams = printParams;
            this.printerName = printerName;
        }

        public Dictionary<string, object> PrintParams
        {
            set { this.printParams = value; }
            get { return this.printParams; }
        }

        #region--字段--
        /// <summary>
        /// 待打印的正文
        /// </summary>
        public static StringBuilder content;
        /// <summary>
        /// 对应业务流水号
        /// </summary>
        public static string ID = "";
        /// <summary>
        /// 待打印的套票条码二维码
        /// </summary>
        public static string FoodTicketKey = "";
        #endregion

        #region--内部函数--
        /// <summary>
        /// 打印事件(核心流程)
        /// </summary>
        private void Printer_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font titleFont = new Font("宋体", 12, FontStyle.Bold);//标题字体
            Font txtFont = new Font("宋体", 9, FontStyle.Regular);//正文文字
            Brush brush = new SolidBrush(Color.Black);//画刷

            Image img = null;
            if (File.Exists(@"post_logo.png"))
            {
                img = Image.FromFile(@"post_logo.png");
                e.Graphics.DrawImage(img, new Rectangle(new Point(0, 0), new Size(180, 60))); //打LOGO
            }

            //打印标题
            if (this.printParams.ContainsKey(PrintBlock.Title))
            { 
                e.Graphics.DrawString(this.printParams[PrintBlock.Title].ToString(), titleFont, brush, new Point(0, 65));
            }

            //打印正文
            if (this.printParams.ContainsKey(PrintBlock.Content))
            {
                e.Graphics.DrawString(this.printParams[PrintBlock.Content].ToString(), txtFont, brush, new Point(0, 95));    //打内容
            }

            //打印二维码
            if (this.printParams.ContainsKey(PrintBlock.Image))
            {
                e.Graphics.DrawImage(this.printParams[PrintBlock.Content] as Image, new Rectangle(new Point(0, 95), new Size(180, 180))); //打二维码
            }

            e.Graphics.DrawString(".", txtFont, brush, new Point(0, 200));   //进纸
        }
        /// <summary>
        /// 底部广告
        /// </summary>
        private static object GetFoot()
        {
            StringBuilder s = new StringBuilder();
            //s.AppendLine(Parameter.GetBarParameter(BarParameterName.小票广告1));
            //s.AppendLine(Parameter.GetBarParameter(BarParameterName.小票广告2));
            s.AppendLine("\t\t\t -   -");
            s.AppendLine("\t\t\t   - ");
            //s.AppendLine("------------------------------");
            return s;
        }
        /// <summary>
        /// 打印内容的头部
        /// </summary>
        private static StringBuilder GetHead()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("打印时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            s.AppendLine("流水号:" + ID);
            s.AppendLine("收银员:" + "赵文迪"); //User.CurrentUser.RealName "张三丰"
            s.AppendLine("------------------------------");
            return s;
        }
        /// <summary>
        /// 得到会员信息的打印内容
        /// </summary>
        private static StringBuilder GetMember(string ICCardID, bool isPoint = false)
        {
            StringBuilder s = new StringBuilder();
            //Member m = new MemberBLL().GetMember(ICCardID);
            //if (null == m) return s;
            s.AppendLine("会员级别" + "\t" + "普通会员");
            s.AppendLine("IC卡号" + "\t\t" + ICCardID);
            s.AppendLine("剩余币数" + "\t" + "1000");
            if (isPoint) s.AppendLine("剩余积分" + "\t" + "100");
            s.AppendLine("------------------------------");
            return s;
        }
        /// <summary>
        /// 计算原有币数
        /// 参数 当前币数 操作币数 增或减
        /// </summary>
        private static string GetOldBalance(string Balance, string Coins, bool IsAdd = false)
        {
            int b = Convert.ToInt32(Balance);
            int c = Convert.ToInt32(Coins);
            if (IsAdd) return (b + c).ToString();
            return (b - c).ToString();
        }
        #endregion

        #region--公共函数--
        public static void Show(Graphics g)
        {
            Font titleFont = new Font("宋体", 12, FontStyle.Bold);//标题字体
            Font txtFont = new Font("宋体", 9, FontStyle.Regular);//正文文字
            Brush brush = new SolidBrush(Color.Black);//画刷

            Image img = null;
            if (File.Exists(@"post_logo.png"))
            {
                img = Image.FromFile(@"post_logo.png");
            }
            if (null != img)
            {
                g.DrawImage(img, new Rectangle(new Point(10, 0), new Size(180, 60))); //打LOGO
            }
            //g.DrawString(Parameter.GetBarParameter(BarParameterName.小票标题), titleFont, brush, new Point(10, 65));   //打标题
            string s = GetHead() + GetFood().ToString() + GetFoot().ToString();
            g.DrawString(s, txtFont, brush, new Point(10, 95));    //打内容
        }
        /// <summary>
        /// 待打印的正文
        /// </summary>
        public void Print()
        {
            PrintDocument p = new PrintDocument();
            p.PrintPage += new PrintPageEventHandler(Printer_PrintPage);
            p.PrinterSettings.PrinterName = printerName;
            p.Print();
        }
        /// <summary>
        /// 得到门票业务的打印内容
        /// </summary>
        public static StringBuilder GetFoodTicket()
        {
            StringBuilder s = new StringBuilder();
            //string sql = String.Format("select * from flw_food_ticket_sale f where f.ID=(select max(ID) from flw_food_ticket_sale where WorkStation='{0}')", CommonValue.WorkStation);
            //DataSet d = WickyDAL.Query(sql);
            //if (d.Tables[0].Rows.Count != 1) return s;

            //DataRow r = d.Tables[0].Rows[0];
            //ID = r["ID"].ToString();
            //string RuleID = r["RuleID"].ToString();
            //sql = String.Format("select * from t_food_ticket_rule f where RuleID={0}", RuleID);
            //DataRow r1 = WickyDAL.Query(sql).Tables[0].Rows[0];

            s.AppendLine("套餐名称" + "\t" + "1元2币");
            s.AppendLine("序列号" + "\t\t" + "100000");
            s.AppendLine("有效期" + "\t\t" + "365" + "天");
            s.AppendLine("消费币数" + "\t" + "1000");
            FoodTicketKey = "54";


            return s;
        }
        /// <summary>
        /// 得到套餐业务的打印内容
        /// </summary>
        public static StringBuilder GetFood()
        {
            StringBuilder s = new StringBuilder();
            //string sql = String.Format("select * from flw_food_sale f,t_foods t where f.ID=(select max(ID) from flw_food_sale where WorkStation='{0}') and f.FoodID=t.FoodID and f.FlowType in ('0','1')", CommonValue.WorkStation);
            //DataSet d = WickyDAL.Query(sql);
            //if (d.Tables[0].Rows.Count != 1) return s;
            //DataRow r = d.Tables[0].Rows[0];
            ID = "10";
            s.AppendLine("购买套餐" + "\t" + "1元2币");
            if ("20" != "0.00")
            {
                s.AppendLine("入会押金" + "\t" + "20");
            }
            if ("40" != "0.00")
            {
                s.AppendLine("手续费" + "\t\t" + "40");
            }
            s.AppendLine("购币数" + "\t\t" + "10");
            s.AppendLine("总金额" + "\t\t" + "2000");
            s.AppendLine("------------------------------");
            s.Append(GetMember("20"));
            return s;
        }
        /// <summary>
        /// 得到退币业务的打印内容
        /// </summary>
        public static StringBuilder GetExitCoin()
        {
            StringBuilder s = new StringBuilder();
            //string sql = String.Format("select * from flw_coin_exit f where f.ID=(select MAX(id) as ID from flw_coin_exit where WorkStation='{0}')", CommonValue.WorkStation);
            //DataSet d = WickyDAL.Query(sql);
            //if (d.Tables[0].Rows.Count != 1) return s;
            //DataRow r = d.Tables[0].Rows[0];
            ID = "10";
            s.AppendLine("兑币：      " + "100");
            s.AppendLine("金额：      " + "25.5");
            s.AppendLine("当前余额：  " + "40");
            s.AppendLine("------------------------------");
            s.Append(GetMember("50"));
            return s;
        }
        /// <summary>
        /// 得到送币业务的打印内容
        /// </summary>
        public static StringBuilder GetFree()
        {
            StringBuilder s = new StringBuilder();
            //string sql = String.Format("select * from flw_coin_sale f where f.ID=(select max(ID) from flw_coin_sale where WorkStation='{0}' and WorkType in ('2','5','8'))", CommonValue.WorkStation);
            //DataSet d = WickyDAL.Query(sql);
            //if (d.Tables[0].Rows.Count != 1) return s;
            //DataRow r = d.Tables[0].Rows[0];
            ID = "10";
            s.AppendLine("赠送币数" + "\t" + "10");
            s.AppendLine("原有币数" + "\t" + GetOldBalance("200", "100"));
            s.AppendLine("赠送方式" + "\t" + ("5" == "5" ? "电子币" : "实物币"));
            s.AppendLine("------------------------------");
            s.Append(GetMember("10"));
            return s;
        }
        /// <summary>
        /// 得到存币业务的打印内容
        /// </summary>
        public static StringBuilder GetSave()
        {
            StringBuilder s = new StringBuilder();
            //string sql = String.Format("select * from flw_coin_sale f where f.ID=(select max(ID) from flw_coin_sale where WorkStation='{0}' and WorkType='4')", CommonValue.WorkStation);
            //DataSet d = WickyDAL.Query(sql);
            //if (d.Tables[0].Rows.Count != 1) return s;
            //DataRow r = d.Tables[0].Rows[0];
            ID = "10";
            s.AppendLine("原有币数" + "\t" + GetOldBalance("200", "100"));
            s.AppendLine("存币数" + "\t\t" + "50");
            s.AppendLine("------------------------------");
            s.Append(GetMember("10"));
            return s;
        }
        /// <summary>
        /// 得到提币业务的打印内容
        /// </summary>
        public static StringBuilder GetCoin()
        {
            StringBuilder s = new StringBuilder();
            //string sql = String.Format("select * from flw_coin_sale f where f.ID=(select max(ID) from flw_coin_sale where WorkStation='{0}' and WorkType in ('3','6','7'))", CommonValue.WorkStation);
            //DataSet d = WickyDAL.Query(sql);
            //if (d.Tables[0].Rows.Count != 1) return s;
            //DataRow r = d.Tables[0].Rows[0];
            ID = "10";
            s.AppendLine("原有币数" + "\t" + GetOldBalance("100", "100", true));
            s.AppendLine("提取币数" + "\t" + "100");
            s.AppendLine("------------------------------");
            s.Append(GetMember("100"));
            return s;
        }
        /// <summary>
        /// 得到积分换币业务的打印内容
        /// </summary>
        public static StringBuilder GetPoint()
        {
            StringBuilder s = new StringBuilder();
            //string sql = String.Format("select * from flw_rebate f where f.ID=(select max(ID) from flw_rebate where WorkStation='{0}')", CommonValue.WorkStation);
            //DataSet d = WickyDAL.Query(sql);
            //if (d.Tables[0].Rows.Count != 1) return s;
            //DataRow r = d.Tables[0].Rows[0];
            ID = "10";
            s.AppendLine("消耗积分" + "\t" + "1000");
            s.AppendLine("兑换币数" + "\t" + "50");
            s.AppendLine("原有币数" + "\t" + GetOldBalance("100", "100"));
            s.AppendLine("------------------------------");
            s.Append(GetMember("10", true));
            return s;
        }
        /// <summary>
        /// 得到商品销售业务的打印内容
        /// </summary>
        public static StringBuilder GetGoods()
        {
            StringBuilder s = new StringBuilder();
            //string sql = String.Format("select f.GoodsID,f.PayType,f.ICCardID,fd.GoodsName,fd.Quantity,g.Price,g.Coin as Coin,g.Point as Point,(fd.Quantity*g.Price) as totalprice from flw_goods f,flw_good_detail fd,t_goods g where f.GoodsID=(select max(GoodsID) from flw_goods where WorkStation='{0}') and f.GoodsID=fd.GoodsID and g.Barcode=fd.Barcode", CommonValue.WorkStation);
            //DataSet d = WickyDAL.Query(sql);
            //if (d.Tables[0].Rows.Count < 1) return s;
            //DataRow r1 = d.Tables[0].Rows[0];
            ID = "10";
            s.AppendLine("商品名称" + "\t" + "数量" + "\t" + "金额");
            double m = 0.00;
            int c = 0;
            int p = 0;
            //for (int i = 0; i < d.Tables[0].Rows.Count; i++)
            //{
            //    DataRow r = d.Tables[0].Rows[i];
            //    s.AppendLine(r["GoodsName"].ToString() + "\t\t" + r["Quantity"].ToString() + "\t￥" + r["totalprice"].ToString());
            //    m += Convert.ToDouble(r["totalprice"]);
            //    c += Convert.ToInt32(r["Coin"]);
            //    p += Convert.ToInt32(r["Point"]);
            //}
            s.AppendLine("------------------------------");
            s.AppendLine("合计金额" + "\t￥" + m.ToString());
            string type = "现金";
            if ("2" == "2") type = "电子币:" + c;
            else if ("3" == "3") type = "积分:" + p;
            s.AppendLine("支付方式" + "\t" + type);
            s.AppendLine("------------------------------");
            s.Append(GetMember("10", true));
            return s;
        }
        public static StringBuilder GetTicketInfo(int ID, int All, int curIndex)
        {
            StringBuilder s = new StringBuilder();
            //string sql = string.Format("select * from flw_project_buy_codelist where buyid='{0}'", ID);
            //DataAccess ac = new DataAccess();
            //DataTable dtFlwCode = ac.ExecuteQueryReturnTable(sql);
            //if (dtFlwCode.Rows.Count > 0)
            //{
            //DataRow rowFlwCode = dtFlwCode.Rows[0];
            //sql = string.Format("select * from flw_project_buy a where id='{0}'", ID);
            //DataTable dtFlwBuy = ac.ExecuteQueryReturnTable(sql);
            //DataRow row = dtFlwBuy.Rows[0];

            FoodTicketKey = "100";
            s.AppendLine("【第" + curIndex + "/" + All + "张门票】");
            s.AppendLine("购买门票ID    " + "10");
            s.AppendLine("购买门票名    " + "测试");
            s.AppendLine("购买数量      " + "10");
            s.AppendLine("金额          " + "100");
            s.AppendLine("有效期至：    " + DateTime.Now);
            s.AppendLine("------------------------------");
            //}

            return s;
        }
        #endregion
    }
}
