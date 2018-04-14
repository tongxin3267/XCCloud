using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.Business.XCGameManaLog;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.XCGameManagerLog;

namespace XCCloudService.SocketService.UDP.Common
{
    public class UDPLogHelper : LogHelper
    {
        private static void SaveUPDRecordLog(UDPRecordType udpRecordType,string storeId,string logTxt)
        {
            StreamWriter wri = null;
            try
            {
                string storeChildPath = GetTextStoreChildPath(storeId);
                string timeChildPath = GetTextLogTimeChildPath();
                string dayChildPath = GetTextLogDayChildPath();
                string recordTypeChildPath = GetTextLogRecordTypeChildPath(udpRecordType);
                string logRootDirectory = string.Format("{0}{1}{2}{3}{4}{5}{6}", CommonConfig.TxtLogPath, GetTextLogChildPath(TxtLogType.UPDService), GetTextLogContentChildPath(TxtLogContentType.Record),
                    dayChildPath, storeChildPath, timeChildPath, recordTypeChildPath);
                if (!Directory.Exists(logRootDirectory))
                {
                    Directory.CreateDirectory(logRootDirectory);
                }

                string fileName = string.Format("{0}_{1}_{2}_{3}", storeChildPath, dayChildPath, timeChildPath, recordTypeChildPath).Replace("/","") + ".txt";
                FileInfo inf = new FileInfo(logRootDirectory + fileName);
                wri = new StreamWriter(logRootDirectory + fileName, true, Encoding.UTF8, 1024);
                string tip = string.Format("{0}{1}{2}", "***************************", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "***************************");
                wri.WriteLine(tip);
                wri.WriteLine(logTxt);
                wri.WriteLine("");
            }
            catch
            {

            }
            finally
            {
                if (wri != null)
                {
                    wri.Close();
                }
            }
        }

        private static string GetTextLogRecordTypeChildPath(UDPRecordType udpRecordType)
        {
            switch (udpRecordType)
            {
                case UDPRecordType.雷达通知: return "雷达通知/";
                case UDPRecordType.雷达心跳: return "雷达心跳/";
                case UDPRecordType.雷达注册: return "雷达注册/";
                case UDPRecordType.设备状态变更通知: return "设备状态变更通知/";
                case UDPRecordType.远程设备控制回复: return "远程设备控制回复/";
                case UDPRecordType.远程设备控制发送: return "远程设备控制发送/";
                default: return "其他";
            }
        }

        /// <summary>
        /// 雷达注册
        /// </summary>
        public static void SaveRadarRegisterLog(string remotePointAddress, int remotePointPort, string storeId, string segment, string deviceToken, bool registerSuccess, string requestJson, string responseJson, string logTxt)
        {
            try
            {
                string storedProcedureName = "SaveRadarRegisterLog";
                SqlParameter[] parameters = new SqlParameter[10];
                parameters[0] = new SqlParameter("@InsId", UDPRecordType.雷达注册);
                parameters[1] = new SqlParameter("@RemotePointAddress", remotePointAddress);
                parameters[2] = new SqlParameter("@RemotePointPort", remotePointPort);
                parameters[3] = new SqlParameter("@StoreId", storeId);
                parameters[4] = new SqlParameter("@Segment", segment);
                parameters[5] = new SqlParameter("@DeviceToken", deviceToken);
                parameters[6] = new SqlParameter("@RegisterSuccess", registerSuccess);
                parameters[7] = new SqlParameter("@RequestJson", requestJson);
                parameters[8] = new SqlParameter("@ResponseJson", responseJson);
                parameters[9] = new SqlParameter("@CreateTime", System.DateTime.Now);
                XCGameManaLogBLL.ExecuteStoredProcedureSentence(storedProcedureName, parameters);
            }
            catch(Exception e)
            {
                LogHelper.SaveLog(TxtLogType.LogDBExcepton, TxtLogContentType.Exception, TxtLogFileType.Time, e.Message);
            }

            SaveUPDRecordLog(UDPRecordType.雷达注册, storeId.ToString(), logTxt);
        }
        
        /// <summary>
        /// 雷达心跳
        /// </summary>
        public static void SaveRadarHeatLog(string remotePointAddress, int remotePointPort, string storeId, string segment,string deviceToken, bool heatSuccess, string requestJson, string responseJson, string logTxt)
        {
            try
            {
                string storedProcedureName = "SaveRadarHeatLog";
                SqlParameter[] parameters = new SqlParameter[10];
                parameters[0] = new SqlParameter("@InsId", UDPRecordType.雷达心跳);
                parameters[1] = new SqlParameter("@RemotePointAddress", remotePointAddress);
                parameters[2] = new SqlParameter("@RemotePointPort", remotePointPort);
                parameters[3] = new SqlParameter("@StoreId", storeId);
                parameters[4] = new SqlParameter("@Segment", segment == null ? string.Empty:"");
                parameters[5] = new SqlParameter("@DeviceToken", deviceToken);
                parameters[6] = new SqlParameter("@HeatSuccess", heatSuccess);
                parameters[7] = new SqlParameter("@RequestJson", requestJson);
                parameters[8] = new SqlParameter("@ResponseJson", responseJson);
                parameters[9] = new SqlParameter("@CreateTime", System.DateTime.Now);
                XCGameManaLogBLL.ExecuteStoredProcedureSentence(storedProcedureName, parameters);
            }
            catch
            { 
                
            }

            SaveUPDRecordLog(UDPRecordType.雷达心跳, storeId.ToString(), logTxt);
        }


        /// <summary>
        /// 雷达设备状态变更
        /// </summary>
        public static void SaveDeviceStateChangeLog(string remotePointAddress,int remotePointPort,string storeId,string deviceToken,string MCUId,string status,bool changeSuccess,string requestJson,string responseJson,string logTxt)
        {
            try
            {
                string storedProcedureName = "SaveDeviceStateChangeLog";
                SqlParameter[] parameters = new SqlParameter[11];
                parameters[0] = new SqlParameter("@InsId", UDPRecordType.设备状态变更通知);
                parameters[1] = new SqlParameter("@RemotePointAddress", remotePointAddress);
                parameters[2] = new SqlParameter("@RemotePointPort", remotePointPort);
                parameters[3] = new SqlParameter("@StoreId", storeId);
                parameters[4] = new SqlParameter("@DeviceToken", deviceToken);
                parameters[5] = new SqlParameter("@MCUId", MCUId);
                parameters[6] = new SqlParameter("@Status", status);
                parameters[7] = new SqlParameter("@ChangeSuccess", changeSuccess);
                parameters[8] = new SqlParameter("@RequestJson", requestJson);
                parameters[9] = new SqlParameter("@ResponseJson", responseJson);
                parameters[10] = new SqlParameter("@CreateTime", System.DateTime.Now);
                XCGameManaLogBLL.ExecuteStoredProcedureSentence(storedProcedureName, parameters);
            }
            catch
            { 
                
            }

            SaveUPDRecordLog(UDPRecordType.设备状态变更通知, storeId.ToString(), logTxt);
        }

        /// <summary>
        /// 向雷达发送设备控制指令
        /// </summary>
        public static void SaveUDPSendDeviceControlLog(string storeId, string mobile, string mcuId, string orderId, string segment, string sn, int coins, int action,string requestJson)
        {
            try
            {
                string storedProcedureName = "SaveUDPSendDeviceControlLog";
                SqlParameter[] parameters = new SqlParameter[11];
                parameters[0] = new SqlParameter("@InsId", UDPRecordType.远程设备控制发送);
                parameters[1] = new SqlParameter("@StoreId", storeId);
                parameters[2] = new SqlParameter("@Mobile", mobile);
                parameters[3] = new SqlParameter("@Segment", segment);
                parameters[4] = new SqlParameter("@MCUId", mcuId);
                parameters[5] = new SqlParameter("@Action", action);
                parameters[6] = new SqlParameter("@Coins", coins);
                parameters[7] = new SqlParameter("@SN", sn);
                parameters[8] = new SqlParameter("@OrderId", orderId);
                parameters[9] = new SqlParameter("@RequestJson", requestJson);
                parameters[10] = new SqlParameter("@CreateTime", System.DateTime.Now);
                XCGameManaLogBLL.ExecuteStoredProcedureSentence(storedProcedureName, parameters);
            }
            catch
            { 
                
            }

            string logTxt = requestJson;
            SaveUPDRecordLog(UDPRecordType.远程设备控制发送, storeId, logTxt);
        }

        public static void SaveUDPRadarNotifyLog(string storeId, string orderId,string deviceToken,string sn, int coins, int action, string result ,string remotePointAddress,int remotePointPort,string requestJson,string responseJson,bool success,string logTxt)
        {
            try
            {
                string storedProcedureName = "SaveUDPRadarNotifyLog";
                SqlParameter[] parameters = new SqlParameter[14];
                parameters[0] = new SqlParameter("@InsId", UDPRecordType.雷达通知);
                parameters[1] = new SqlParameter("@RemotePointAddress", remotePointAddress);
                parameters[2] = new SqlParameter("@RemotePointPort", remotePointPort);
                parameters[3] = new SqlParameter("@StoreId", storeId);
                parameters[4] = new SqlParameter("@DeviceToken", deviceToken);
                parameters[5] = new SqlParameter("@Coins",coins);
                parameters[6] = new SqlParameter("@SN", sn);
                parameters[7] = new SqlParameter("@OrderId", orderId);
                parameters[8] = new SqlParameter("@Result", result);
                parameters[9] = new SqlParameter("@Action", action);
                parameters[10] = new SqlParameter("@Success", success);
                parameters[11] = new SqlParameter("@RequestJson", requestJson);
                parameters[12] = new SqlParameter("@ResponseJson", responseJson);
                parameters[13] = new SqlParameter("@CreateTime", System.DateTime.Now);
                XCGameManaLogBLL.ExecuteStoredProcedureSentence(storedProcedureName, parameters);
            }
            catch
            { 
                
            }

            SaveUPDRecordLog(UDPRecordType.雷达通知, storeId.ToString(), logTxt);
        }

        public static void SaveUDPDeviceControlLog(string storeId,string orderId,string remotePointAddress,int remotePointPort,string sn,string requestJson,string responseJson,bool bSuccess,string logTxt)
        {
            try
            {
                string storedProcedureName = "SaveUDPDeviceControlLog";
                SqlParameter[] parameters = new SqlParameter[10];
                parameters[0] = new SqlParameter("@InsId", UDPRecordType.远程设备控制回复);
                parameters[1] = new SqlParameter("@RemotePointAddress", remotePointAddress);
                parameters[2] = new SqlParameter("@RemotePointPort", remotePointPort);
                parameters[3] = new SqlParameter("@StoreId", storeId);
                parameters[4] = new SqlParameter("@OrderId", orderId);
                parameters[5] = new SqlParameter("@SN", sn);
                parameters[6] = new SqlParameter("@Success", bSuccess);
                parameters[7] = new SqlParameter("@RequestJson", requestJson);
                parameters[8] = new SqlParameter("@ResponseJson", responseJson);
                parameters[9] = new SqlParameter("@CreateTime", System.DateTime.Now);
                XCGameManaLogBLL.ExecuteStoredProcedureSentence(storedProcedureName, parameters);
            }
            catch
            { 
                
            }

            SaveUPDRecordLog(UDPRecordType.远程设备控制回复, storeId, logTxt);
        }
    }
}
