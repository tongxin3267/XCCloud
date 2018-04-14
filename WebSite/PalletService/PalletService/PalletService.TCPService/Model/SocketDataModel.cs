using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PalletService.TCPService.Model
{

    /// <summary>
    /// socket通讯服务的数据模式
    /// </summary>
    [DataContract]
    public class SocketDataModel<TSendObject>
    {
        public SocketDataModel(string messageType, string sysId, object sendObj)
        {
            this.MessageType = messageType;
            this.SysId = sysId;
            this.SendObject = (TSendObject)sendObj;
        }

        /// <summary>
        /// socket服务类型枚举对应的值(用于对服务方式进行判断)
        /// </summary>
        [DataMember(Name = "msgType", Order = 1)]
        public string MessageType { set; get; }

        /// <summary>
        /// 发送方对象
        /// </summary>
        [DataMember(Name = "sObj", Order = 2)]
        public TSendObject SendObject { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "sysId", Order = 1)]
        public string SysId { set; get; }

        /// <summary>
        /// 数据内容
        /// </summary>
        [DataMember(Name = "data", Order = 4)]
        public object Data { set; get; }
    }

    /// <summary>
    /// socket通讯服务的数据模式
    /// </summary>
    [DataContract]
    public class SocketDataModel<TSendObject, TDataObject>
    {
        public SocketDataModel(string messageType, string sysId, object sendObj, object dataObj)
        {
            this.MessageType = messageType;
            this.SysId = sysId;
            this.SendObject = (TSendObject)sendObj;
            this.Data = (TDataObject)dataObj;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "sysId", Order = 1)]
        public string SysId { set; get; }

        /// <summary>
        /// socket服务类型枚举对应的值(用于对服务方式进行判断)
        /// </summary>
        [DataMember(Name = "msgType", Order = 2)]
        public string MessageType { set; get; }

        /// <summary>
        /// 发送方对象
        /// </summary>
        [DataMember(Name = "sObj", Order = 3)]
        public TSendObject SendObject { set; get; }


        /// <summary>
        /// 数据内容
        /// </summary>
        [DataMember(Name = "data", Order = 4)]
        public TDataObject Data { set; get; }
    }



    /// <summary>
    /// 消息推送发送方对象模式
    /// </summary>
    [DataContract]
    public class MemberSendObject
    {
        public MemberSendObject(string token)
        {
            this.Token = token;
        }


        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember(Name = "token", Order = 1)]
        public string Token { set; get; }
    }


    [DataContract]
    public class UserSendObject
    {
        public UserSendObject(string userToken,string storeId)
        {
            this.UserToken = userToken;
            this.StoreId = storeId;
        }


        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember(Name = "userToken", Order = 1)]
        public string UserToken { set; get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember(Name = "storeId", Order = 2)]
        public string StoreId { set; get; }
    }

    /// <summary>
    /// 消息接收方对象模式
    /// </summary>
    [DataContract]
    public class MemberReceiveObject
    {
        public MemberReceiveObject(string mobile)
        {
            this.Mobile = mobile;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember(Name = "mobile", Order = 2)]
        public string Mobile { set; get; }
    }

    [DataContract]
    public class CoinObj
    {
        [DataMember(Name = "action", Order = 1)]
        public string Action { set; get; }

        [DataMember(Name = "coins", Order = 1)]
        public int Coins { set; get; }
    }

    [DataContract]
    public class DogAnswerObj
    {
        public DogAnswerObj(string result_code, string answerMsgType, string dogId,string answerMsg)
        {
            this.Result_Code = result_code;
            this.AnswerMsgType = answerMsgType;
            this.AnswerMessage = answerMsg;
            this.DogId = dogId;
        }

        [DataMember(Name = "result_code", Order = 1)]
        public string Result_Code { set; get; }

        [DataMember(Name = "answerMsgType", Order = 2)]
        public string AnswerMsgType { set; get; }

        [DataMember(Name = "dogId", Order = 3)]
        public string DogId { set; get; }

        [DataMember(Name = "answerMsg", Order = 4)]
        public string AnswerMessage { set; get; }
    }
}
