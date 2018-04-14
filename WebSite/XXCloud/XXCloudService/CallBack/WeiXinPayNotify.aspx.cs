using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Pay.WeiXinPay.Lib;

namespace XXCloudService.CallBack
{
    public partial class WeiXinPayNotify : System.Web.UI.Page
    {
        public WxPayData GetNotifyData()
        {
            LogHelper.SaveLog(TxtLogType.WeiXin,TxtLogContentType.Common,TxtLogFileType.Time, "GetNotifyData");

            //接收从微信后台POST过来的数据
            System.IO.Stream s = Request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();

            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            try
            {
                data.FromXml(builder.ToString());
                if (data.CheckSign())
                {
                    string result_code = data.GetValue("result_code").ToString();
                    if (result_code == "SUCCESS")
                    {
                        string out_trade_no = data.GetValue("out_trade_no").ToString();
                        string trade_no = data.GetValue("transaction_id").ToString();
                        if (MPOrderBusiness.UpdateOrderForPaySuccess(out_trade_no, trade_no))
                        {
                            WxPayData res = new WxPayData();
                            res.SetValue("return_code", "SUCCESS");
                            res.SetValue("return_msg", "OK");
                            LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Time, res.ToXml());
                            Response.Write(res.ToXml());
                            Response.End();
                        }
                        LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Time, "Response.End()");
                    }
                }
                else
                { 
                    
                }
            }
            catch (WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                Response.Write(res.ToXml());
                Response.End();
            }
            return data;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            WxPayData data = GetNotifyData();
        }
    }
}