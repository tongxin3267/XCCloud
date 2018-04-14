using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Linq;
using PalletService.Utility.Dog;
using PalletService.Utility.PeopleCard;
using DeviceUtility.Utility.MemberCard;
using System.Drawing;
using System.IO;
using System.Drawing.Printing;
using DeviceUtility.Utility.Print;
using DeviceUtility.Utility.Coin;
using PalletService.TCPService.Model;
using PalletService.Common;
using PalletService.SocketService;
using PalletService.Business.Ticket;
using PalletService.Business.SysConfig;
using PalletService.Business.Store;
using PalletService.Business.Member;
using PalletService.Model.SocketData;
using PalletService.DeviceUtility.Utility.Machine;

namespace PalletService.TCPService
{
    public class CommandProcessHandler
    {
        #region 发送数据
        /// <summary>
        /// 把发送给客户端消息打包处理（拼接上谁什么时候发的什么消息）
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="message">Message.</param>
        private static byte[] PackageServerData(string msg)
        {
            byte[] content = null;
            byte[] temp = Encoding.UTF8.GetBytes(msg);
            if (temp.Length < 126)
            {
                content = new byte[temp.Length + 2];
                content[0] = 0x81;
                content[1] = (byte)temp.Length;
                Buffer.BlockCopy(temp, 0, content, 2, temp.Length);
            }
            else if (temp.Length < 0xFFFF)
            {
                content = new byte[temp.Length + 4];
                content[0] = 0x81;
                content[1] = 126;
                content[2] = (byte)(temp.Length >> 8);
                content[3] = (byte)(temp.Length & 0xFF);
                Buffer.BlockCopy(temp, 0, content, 4, temp.Length);
            }
            return content;
        }
        #endregion

        public static void ExceptionAnswer(Socket sockeClient)
        {
            var msgObj = new
            {
                result_code = 0,
                answerMsgType = Convert.ToInt16(TCPMessageType.服务器应答出错).ToString(),
                answerMsg = "服务器应答出错"
            };
            var msg = Utils.SerializeObject(msgObj);
            byte[] msgBuffer = PackageServerData(msg);
            sockeClient.BeginSendTo(msgBuffer, 0, msgBuffer.Length, SocketFlags.None, sockeClient.RemoteEndPoint, new AsyncCallback(SendCallBack), sockeClient);
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="commonObject"></param>
        /// <param name="SessionPool"></param>
        /// <param name="IP"></param>
        public static void Register(Socket sockeClient,string msg, Dictionary<string, Session> sessionPool, string IP)
        {
            string answerMsgType = Convert.ToInt32(TCPMessageType.用户客户端注册).ToString();
            string mobile = string.Empty;
            string errMsg = string.Empty;
            //解析消息对象
            SocketDataModel<UserSendObject> socketDataModel = Utils.DataContractJsonDeserializer<SocketDataModel<UserSendObject>>(msg);
            UserSendObject sendObj = socketDataModel.SendObject;
            string userToken = sendObj.UserToken;
            //设置客户端对象
            List<string> repeatIPList = new List<string>();
            var pair = sessionPool.Where(p => p.Value.SendClientId.Equals(userToken));
            foreach (var p in pair)
            {
                if (!p.Value.IP.Equals(IP))
                {
                    repeatIPList.Add(p.Value.IP);
                }
            }

            foreach (var v in repeatIPList)
            {
                sessionPool.Remove(v);
            }

            sessionPool[IP].SendClientId = userToken;
            sessionPool[IP].SockeClient = sockeClient;
            //应答数据
            SendSuccessData(answerMsgType, sessionPool, IP, "注册成功", null);
        }

        /// <summary>
        /// 消息推送
        /// </summary>
        /// <param name="socketDataModel"></param>
        /// <param name="SessionPool"></param>
        /// <param name="IP"></param>
        public static void Push(string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {

        }

        public static void GetDogId(string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {
            string dogId = string.Empty;
            string answerMsgType = Convert.ToInt32(TCPMessageType.读取加密狗).ToString();

            //读取加密狗
            if (DogUtility.GetBogID(out dogId))
            {
                var dataObj = new { dogId = dogId };
                SendSuccessData(answerMsgType, sessionPool, sendIP, "获取加密狗号成功", dataObj);
            }
            else
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "获取加密狗出错");
            } 
        }

        /// <summary>
        /// 读取加密狗
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sessionPool"></param>
        /// <param name="sendIP"></param>
        public static void GetDogIdAndMachineName(string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {
            string receiveIP = string.Empty;
            string dogId = string.Empty;
            string computerName = string.Empty;
            string answerMsgType = Convert.ToInt32(TCPMessageType.读取加密狗和机器名).ToString();

            //读取加密狗
            if (!DogUtility.GetBogID(out dogId))
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "获取加密狗号和机器名出错");
            }

            //读取机器名
            if (MachineUtility.GetMachineName(out computerName))
            {
                var dataObj = new { machineName = computerName, dogId = dogId };
                SendSuccessData(answerMsgType, sessionPool, sendIP, "获取加密狗号和机器名成功", dataObj);
            }
            else
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "获取加密狗号和机器名出错");
            }
        }


        /// <summary>
        /// 读取机器名
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sessionPool"></param>
        /// <param name="sendIP"></param>
        public static void GetMachineName(string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {
            string receiveIP = string.Empty;
            string computerName = string.Empty;
            string answerMsgType = Convert.ToInt32(TCPMessageType.读取机器名).ToString();
            //读取机器名
            if (MachineUtility.GetMachineName(out computerName))
            {
                var dataObj = new { machineName = computerName };
                SendSuccessData(answerMsgType, sessionPool, sendIP, "获取机器名成功", dataObj);
            }
            else
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "获取机器名出错");
            }
        }


        /// <summary>
        /// 读取身份证信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sessionPool"></param>
        /// <param name="sendIP"></param>
        public static void GetPeopleCard(string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {
            string receiveIP = string.Empty;
            string dogId = string.Empty;
            string answerMsgType = Convert.ToInt32(TCPMessageType.读取身份证).ToString();

            //读取身份证信息
            PeopleCardUtility utility = new PeopleCardUtility();
            PeopleCardModel peopleCardModel = new PeopleCardModel();
            string cardMsg = string.Empty;
            if (utility.ReadIDCard(out peopleCardModel, out cardMsg))
            {
                var cardData = new
                {
                    cardNo = peopleCardModel.身份证号,
                    name = peopleCardModel.姓名,
                    gender = peopleCardModel.性别,
                    nation = peopleCardModel.民族,
                    birthDay = peopleCardModel.生日,
                    address = peopleCardModel.住址,
                    signingOrg = peopleCardModel.签发机关,
                    makeDate = peopleCardModel.发证日期,
                    expirationDate = peopleCardModel.有效期限,
                    imageUrl = ""
                };

                var msgObj = new
                {
                    result_code = 1,
                    answerMsgType = answerMsgType,
                    answerMsg = "获取身份证信息成功",
                    cardData = cardData
                };
                SendData(msgObj, sessionPool, sendIP);
            }
            else
            {
                var msgObj = new
                {
                    result_code = 0,
                    answerMsgType = answerMsgType,
                    answerMsg = cardMsg,
                };
                SendData(msgObj, sessionPool, sendIP);
            }
        }

        /// <summary>
        /// 读取IC卡信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sessionPool"></param>
        /// <param name="sendIP"></param>
        public static void GetICCard(string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {
            string receiveIP = string.Empty;
            string dogId = string.Empty;
            string answerMsgType = Convert.ToInt32(TCPMessageType.读取IC卡).ToString();
            string cardMsg = string.Empty;
            string storePassword = string.Empty;
            string icCardId = string.Empty;
            string repeatCode = string.Empty;
            string memberRepeatCode = string.Empty;
            string errMsg = string.Empty;
            //解析消息对象
            SocketDataModel<UserSendObject> socketDataModel = Utils.DataContractJsonDeserializer<SocketDataModel<UserSendObject>>(msg);
            UserSendObject sendObj = socketDataModel.SendObject;
            //接受信息方是否存在
            if (!GetIP(sessionPool, sendObj.UserToken, out receiveIP))
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "用户未注册");
                return;
            }
            //获取店密码接口
            string userToken = sendObj.UserToken;
            if (!StoreBusiness.GetStorePassword(userToken, out storePassword, out errMsg))
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "获取门店信息出错");
                return;
            }

            object result_data = null;
            
            //读取IC卡信息
            if (ICCardUtility.GetICCardID(storePassword,out icCardId,out repeatCode, out errMsg))
            {
                if (MemberBusiness.GetMember(userToken, sendObj.StoreId, icCardId, ref result_data, out errMsg))
                {
                    SendSuccessData(answerMsgType, sessionPool, sendIP, "读取会员信息成功", result_data);
                }
                else
                {
                    SendFailData(answerMsgType, sessionPool, sendIP, errMsg);
                }      
            }
            else
            {
                SendFailData(answerMsgType, sessionPool, sendIP, errMsg);
            }
        }

        public static void CreateICCard(string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {
            string sRepeatCode = (new Random()).Next(0,255).ToString();
            string receiveIP = string.Empty;
            string dogId = string.Empty;
            string storePassword = string.Empty;
            string newICCardId = string.Empty;
            string errMsg = string.Empty;
            string answerMsgType = Convert.ToInt32(TCPMessageType.办理新IC卡).ToString();
            //解析消息对象
            SocketDataModel<UserSendObject, MemberRegisterModel> socketDataModel = Utils.DataContractJsonDeserializer<SocketDataModel<UserSendObject, MemberRegisterModel>>(msg);
            UserSendObject sendObj = socketDataModel.SendObject;
            MemberRegisterModel memberRegisterModel = (MemberRegisterModel)(socketDataModel.Data);
            newICCardId = memberRegisterModel.ICCardId;
            //接受信息方是否存在
            if (!GetIP(sessionPool, sendObj.UserToken, out receiveIP))
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "用户未注册");
                return;
            }
            //获取店密码接口
            string userToken = sendObj.UserToken;
            if (!StoreBusiness.GetStorePassword(userToken, out storePassword, out errMsg))
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "获取门店信息出错");
                return;
            }
            //验证是否可以办卡
            if (ICCardUtility.CheckNullCard(storePassword))
            {
                if (MemberBusiness.CheckMemberCanRegister(userToken, memberRegisterModel.StoreId, memberRegisterModel.Mobile, out errMsg))
                {
                    //办理新卡
                    if (ICCardUtility.CreateICCard(newICCardId, sRepeatCode, storePassword, out errMsg, true))
                    {
                        memberRegisterModel.RepeatCode = sRepeatCode;
                        object result_data = null;
                        int repeatCode = 0;
                        if (MemberBusiness.MemberRegister(memberRegisterModel,out repeatCode,out errMsg,ref result_data))
                        {
                            SendSuccessData(answerMsgType, sessionPool, sendIP, "会员卡开通成功", result_data);
                        }
                        else
                        {
                            SendFailData(answerMsgType, sessionPool, sendIP, errMsg);
                        }
                    }
                    else
                    {
                        SendFailData(answerMsgType, sessionPool, sendIP, errMsg);
                    }
                }
                else
                {
                    SendFailData(answerMsgType, sessionPool, sendIP, errMsg);
                }
            }
            else
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "本卡已使用，请插入空卡");
            }
        }


        public static void ExitICCard(string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {
            string receiveIP = string.Empty;
            string dogId = string.Empty;
            string storePassword = string.Empty;
            string newICCardId = string.Empty;
            string errMsg = string.Empty;
            string answerMsgType = Convert.ToInt32(TCPMessageType.退卡).ToString();
            //解析消息对象
            SocketDataModel<UserSendObject> socketDataModel = Utils.DataContractJsonDeserializer<SocketDataModel<UserSendObject>>(msg);
            UserSendObject sendObj = socketDataModel.SendObject;
            //接受信息方是否存在
            if (!GetIP(sessionPool, sendObj.UserToken, out receiveIP))
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "用户未注册");
                return;
            }
            //获取店密码接口
            string userToken = sendObj.UserToken;
            if (!StoreBusiness.GetStorePassword(userToken, out storePassword, out errMsg))
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "获取门店信息出错");
                return;
            }
            //清理卡片
            if (!ICCardUtility.RecoveryICCard(storePassword, out errMsg))
            {
                SendFailData(answerMsgType, sessionPool, sendIP, errMsg);
                return;
            }
            else
            {
                SendSuccessData(answerMsgType, sessionPool, sendIP, "会员卡回收成功",null);
            }
        }

        public static void Print(string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {
            string answerMsgType = Convert.ToInt32(TCPMessageType.打印小票).ToString();

            object msgObj = Utils.DeserializeObject(msg);
            object ticketData= Utils.GetJsonObjectValue(msgObj, "data");
            object ticketTypeObj = Utils.GetJsonObjectValue(ticketData, "ticketType");
            if (ticketTypeObj != null)
            {
                //获取票据参数
                var data = ((Dictionary<string, object>)ticketData)["ticketData"];
                Dictionary<string, object> dict = TicketBusiness.GetTicketParams(ticketTypeObj.ToString(), (Dictionary<string, object>)data);
                //打印票据
                PrintUtility printUtility = new PrintUtility(dict, SysConfigBusiness.PrinterName);
                printUtility.Print();
            }
            else
            {
                SendFailData(answerMsgType, sessionPool, sendIP, "小票类型不正确");
            }
        }

        public static void Coin(string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {
            SocketDataModel<UserSendObject, CoinObj> socketDataModel = Utils.DataContractJsonDeserializer<SocketDataModel<UserSendObject, CoinObj>>(msg);
            UserSendObject sendObj = socketDataModel.SendObject;
            CoinObj coinObj = socketDataModel.Data;
            CoinUtility.GetSaleCoinerParameter();
            CoinUtility coin = new CoinUtility();
            coin.SendInit();
            coin.SendOutCoin(coinObj.Coins);
            coin.SerialClose();
        }

        /// <summary>
        /// 发送用户为注册信息
        /// </summary>
        /// <param name="answerMsgType"></param>
        private static void SendFailData(string answerMsgType, Dictionary<string, Session> sessionPool, string sendIP,string errMsg)
        {
            var msgObj = new
            {
                result_code = 0,
                answerMsgType = answerMsgType,
                answerMsg = errMsg,
            };
            SendData(msgObj, sessionPool, sendIP);
        }

        private static void SendSuccessData(string answerMsgType, Dictionary<string, Session> sessionPool, string sendIP, string answerMsg,object dataObj)
        {
            if (dataObj == null)
            {
                var msgObj = new
                {
                    result_code = 1,
                    answerMsgType = answerMsgType,
                    answerMsg = answerMsg
                };
                SendData(msgObj, sessionPool, sendIP);
            }
            else
            {
                var msgObj = new
                {
                    result_code = 1,
                    answerMsgType = answerMsgType,
                    answerMsg = answerMsg,
                    dataObj = dataObj
                };
                SendData(msgObj, sessionPool, sendIP);
            }
        }


        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sessionPool"></param>
        /// <param name="ip"></param>
        private static void SendData(object data, Dictionary<string, Session> sessionPool,string ip)
        {
            var msg = Utils.SerializeObject(data);
            byte[] msgBuffer = PackageServerData(msg);
            sessionPool[ip].SockeClient.BeginSendTo(msgBuffer, 0, msgBuffer.Length, SocketFlags.None, sessionPool[ip].SockeClient.RemoteEndPoint, new AsyncCallback(SendCallBack), sessionPool[ip].SockeClient);
        }

        public static bool GetIP(Dictionary<string, Session> sessionPool,string mobile, out string ip)
        {
            ip = string.Empty;
            string receiveClientID = mobile;
            KeyValuePair<string, Session> pair = sessionPool.FirstOrDefault(p => p.Value.SendClientId.Equals(receiveClientID));
            if (pair.Key == null)
            {
                return false;
            }
            else
            {
                ip = pair.Key;
                return true;
            }
        }

        public static void SendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket Client = (Socket)ar.AsyncState;
                Client.EndSend(ar);
            }
            catch (Exception e)
            {
                //LogHelper.WriteLog(e);
            }
        }
    }
}
