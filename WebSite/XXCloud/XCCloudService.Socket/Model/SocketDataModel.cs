using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.SocketService.TCP.Model
{
    #region "socket通讯服务的数据模式"

        /// <summary>
        /// socket通讯服务的数据模式
        /// </summary>
        [DataContract]
        public class SocketDataModel<TSendObject, TReceiveObject>
        {
            public SocketDataModel(string messageType, string sysId,object sendObj, object receiveObj, object data)
            {
                this.MessageType = messageType;
                this.SysId = sysId;
                this.SendObject = (TSendObject)sendObj;
                this.ReceiveObject = (TReceiveObject)receiveObj;
                this.Data = data;
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
            /// 接受方对象
            /// </summary>
            [DataMember(Name = "rObj", Order = 3)]
            public TReceiveObject ReceiveObject { set; get; }

            /// <summary>
            /// 数据内容
            /// </summary>
            [DataMember(Name = "data", Order = 4)]
            public object Data { set; get; }
        }

    #endregion

    #region "消息推送模式"

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
        public class RadarSendObject
        {
            public RadarSendObject(string storeId,string segment)
            {
                this.StoreId = storeId;
                this.Segment = segment;
            }


            /// <summary>
            /// 用户ID
            /// </summary>
            [DataMember(Name = "storeId", Order = 1)]
            public string StoreId { set; get; }

            /// <summary>
            /// 雷达设备
            /// </summary>
            [DataMember(Name = "segment", Order = 2)]
            public string Segment { set; get; }
        }

    #endregion

        [DataContract]
        public class RadarAnswerObj
        {
            public RadarAnswerObj(string result_code, string answerMsgType, string answerMsg)
            {
                this.Result_Code = result_code;
                this.AnswerMsgType = answerMsgType;
                this.AnswerMessage = answerMsg;
            }

            [DataMember(Name = "result_code", Order = 1)]
            public string Result_Code { set; get; }

            [DataMember(Name = "answerMsgType", Order = 2)]
            public string AnswerMsgType { set; get; }

            [DataMember(Name = "answerMsg", Order = 3)]
            public string AnswerMessage { set; get; }
        }

        /// <summary>
        /// socket通讯服务的数据模式
        /// </summary>
        [DataContract]
        public class SocketDataModel<TSendObject, TReceiveObject,TDataObj>
        {
            public SocketDataModel(string messageType, string sysId, object sendObj, object receiveObj, object data)
            {
                this.MessageType = messageType;
                this.SysId = sysId;
                this.SendObject = (TSendObject)sendObj;
                this.ReceiveObject = (TReceiveObject)receiveObj;
                this.Data = (TDataObj)data;
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
            /// 接受方对象
            /// </summary>
            [DataMember(Name = "rObj", Order = 3)]
            public TReceiveObject ReceiveObject { set; get; }

            /// <summary>
            /// 数据内容
            /// </summary>
            [DataMember(Name = "data", Order = 4)]
            public TDataObj Data { set; get; }
        }


    }
