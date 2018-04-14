using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.SocketService.TCP.Model;

namespace XCCloudService.SocketService.TCP.Common
{
    public class TCPClientHelper
    {
        /// <summary>
        /// 是否客户端链接
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="socketClientType"></param>
        /// <returns></returns>
        public static bool IsClientConnctionRequest(string msg, out SocketClientType socketClientType)
        {
            socketClientType = SocketClientType.UnknownClient;
            if (msg.Contains("Sec-WebSocket-Key"))
            {
                socketClientType = SocketClientType.WebClient;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证IP是否已连接
        /// </summary>
        /// <param name="SessionPool"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsConnected(Dictionary<string, Session> SessionPool,string ip)
        {
            if (SessionPool[ip] != null && SessionPool[ip].SocketClientType != SocketClientType.UnknownClient)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 解析消息
        /// </summary>
        /// <param name="SessionPool"></param>
        /// <param name="ip"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        //public static bool AnalysisMsg(Dictionary<string, Session> SessionPool, string ip, string msg,ref object socketDataModel,out messageType)
        //{
        //    int messageType = 0;
        //    try
        //    {
        //        var msgObj = Utils.DeserializeObject(msg);
        //        if (Utils.JsonObjectExistProperty(msgObj, "msgType"))
        //        {
        //            messageType = int.Parse(Utils.GetJsonObjectValue(msgObj, "msgType").ToString());
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    if (messageType == (int)(MessageTypeEnum.客户端注册))
        //    {
        //        socketDataModel = Utils.DataContractJsonDeserializer<SocketDataModel<MemberSendObject, MemberReceiveObject>>(msg);
        //        MemberSendObject sendObj = ((SocketDataModel<MemberSendObject, MemberReceiveObject>)(socketDataModel)).SendObject;
        //        SessionPool[ip].SendClientId = sendObj.StoreId + "_" + sendObj.Mobile;
        //        return true;
        //    }

        //    return false;
        //}

    }
}
