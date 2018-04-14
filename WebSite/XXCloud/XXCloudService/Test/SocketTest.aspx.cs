using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Business.Common;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.Common;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Model.WeiXin.Message;
using XCCloudService.SocketService.TCP.Business;
using XCCloudService.SocketService.TCP.Model;
using XCCloudService.Utility;
using XCCloudService.WeiXin;
using XCCloudService.WeiXin.Config;
using XCCloudService.WeiXin.Message;
using XCCloudService.WeiXin.WeixinOAuth;

namespace XXCloudService.Test
{
    public partial class SocketTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UDPSocketAnswerModel answerModel = new UDPSocketAnswerModel("192.168.1.145",50,null,"1234567890",System.DateTime.Now,"15618920033","100027","0001","20171014300027",10,"23432");
            UDPSocketAnswerBusiness.SetAnswer(answerModel);

            string key = "100027" + "_" + "0001";
            string token = "62fb63fcc33c246999246cb440e24b1b0";
            XCGameRouteDeviceCache.AddToken(key, token);

        }

        private void GetOpenId()
        { 
            string accsess_token = string.Empty;
            string refresh_token = string.Empty;
            string openId = string.Empty;
            string code = Request["code"] ?? "";
            TokenMana.GetTokenForScanQR(code, out accsess_token, out refresh_token, out openId);
            Response.Write(openId);
        }

        private void MessagePush()
        {
            LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "MessagePush");
            string openId = "oNWocwX2vXScYZkP6m3UVxfmom-o";
            string errMsg = "";
            OrderPaySuccessDataModel dataModel = new OrderPaySuccessDataModel("5yuan10bi", "够币", "测试11","2017年12月25日 15:09:29","110000000000");
            if (MessageMana.PushMessage(WeiXinMesageType.OrderPaySuccess, openId, dataModel, out errMsg))
            {
                LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "true");
            }
            else
            {
                LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, errMsg);
            }
        }


        private void OrderFailPush()
        {
            LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "MessagePush");
            string openId = "oNWocwX2vXScYZkP6m3UVxfmom-o";
            string errMsg = "";
            OrderFailSuccessDataModel dataModel = new OrderFailSuccessDataModel("110000000000", "100", "支付宝支付");
            if (MessageMana.PushMessage(WeiXinMesageType.OrderFailSuccess, openId, dataModel, out errMsg))
            {
                LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "true");
            }
            else
            {
                LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, errMsg);
            }
        }
        private void sendData()
        {
            //出币后向客户端发送成功信息
            //UDPSocketAnswerModel answerModel = UDPSocketAnswerBusiness.GetAnswerModel(model.SN);
            RadarSendObject sendObj = new RadarSendObject("100010", "001");
            MemberReceiveObject receiveObj = new MemberReceiveObject("15618920033");
            string messageType = Convert.ToInt16(TCPMessageType.雷达操作回复).ToString();
            string sysId = Convert.ToInt16(XCCloudService.Common.Enum.SysIdEnum.UDP).ToString();
            var dataObj = new { result_code = 1 };
            SocketDataModel<RadarSendObject, MemberReceiveObject> socketDataModel = new SocketDataModel<RadarSendObject, MemberReceiveObject>(messageType, sysId, sendObj, receiveObj, Utils.SerializeObject(dataObj));
            //TCPClientServiceForUDPBusiness.Send(Utils.DataContractJsonSerializer(socketDataModel));
        }

    }
}