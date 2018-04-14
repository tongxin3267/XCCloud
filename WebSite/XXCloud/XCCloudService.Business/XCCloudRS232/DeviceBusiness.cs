using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.Model.CustomModel.XCCloudRS232;
using XCCloudService.Model.XCCloudRS232;
using XCCloudService.Common.Extensions;
using XCCloudService.Common.Enum;
using XXCloudService.RadarServer.Info;

namespace XCCloudService.Business.XCCloudRS232
{
    public class DeviceBusiness
    {
        #region 控制器详细信息
        /// <summary>
        /// 控制器详细信息
        /// </summary>
        /// <returns></returns>
        public static DataTable GetRouterDetail()
        {
            #region Sql语句
            string strSql = @"
                            SELECT a.ID, a.Token AS DeviceToken, a.DeviceName, a.DeviceType, a.SN, a.Status, b.MerchName, b.Mobile, b.State
                            FROM Base_DeviceInfo a
                            INNER JOIN Base_MerchInfo b ON a.MerchID = b.ID
                            WHERE a.DeviceType = 0
                            ";
            #endregion

            DataTable table = XCCloudRS232BLL.ExecuterSqlToTable(strSql);
            return table;
        }
        #endregion

        #region 获取外设集合
        /// <summary>
        /// 获取外设集合
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCoinDeviceList()
        {
            #region Sql语句
            string strSql = @"
                            SELECT c.Token AS RouterToken, a.ID AS DeviceId, a.Token AS DeviceToken, a.DeviceName, a.DeviceType, a.Status AS [State], a.SN, b.HeadAddress
                            FROM Base_DeviceInfo a
                            INNER JOIN Data_MerchDevice b ON b.DeviceID = a.ID
                            INNER JOIN Base_DeviceInfo c ON c.ID = b.ParentID
                            ";
            #endregion

            DataTable table = XCCloudRS232BLL.ExecuterSqlToTable(strSql);
            return table;
        }
        #endregion

        #region 获取终端设备集合
        /// <summary>
        /// 获取终端设备集合
        /// </summary>
        /// <returns></returns>
        public static DataTable GetTerminalDeviceList()
        {
            #region Sql语句
            string strSql = @"
                            SELECT c.Token AS RouterToken, a.ID AS DeviceId, a.Token AS DeviceToken, a.DeviceName, a.DeviceType, a.Status AS [State], a.SN, b.HeadAddress, b.GroupID
                            FROM Base_DeviceInfo a
                            INNER JOIN Data_MerchSegment b ON b.DeviceID = a.ID
                            INNER JOIN Base_DeviceInfo c ON c.ID = b.ParentID
                            ";
            #endregion

            DataTable table = XCCloudRS232BLL.ExecuterSqlToTable(strSql);
            return table;
        }
        #endregion

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Model.XCCloudRS232.Base_DeviceInfo> GetDeviceList()
        {
            BLL.IBLL.XCCloudRS232.IDeviceService deviceService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IDeviceService>();
            var deviceList = deviceService.GetModels(d => true);
            return deviceList;
        }

        /// <summary>
        /// 根据token获取设备实体
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Model.XCCloudRS232.Base_DeviceInfo GetDeviceModel(string token)
        {
            return GetDeviceList().FirstOrDefault(m => m.Token.Equals(token));
        }

        /// <summary>
        /// 根据设备ID获取设备实体
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Model.XCCloudRS232.Base_DeviceInfo GetDeviceModelById(int deviceId)
        {
            return GetDeviceList().FirstOrDefault(m => m.ID == deviceId);
        }

        /// <summary>
        /// 根据商户ID及设备令牌获取设备实体
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Model.XCCloudRS232.Base_DeviceInfo GetDeviceModel(int merchId, string deviceToken)
        {
            return GetDeviceList().Single(m => m.MerchID == merchId && m.Token.Equals(deviceToken));
        }

        /// <summary>
        /// 更新设备
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool UpdateDevice(Model.XCCloudRS232.Base_DeviceInfo d)
        {
            BLL.IBLL.XCCloudRS232.IDeviceService deviceService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IDeviceService>();
            bool ret = deviceService.Update(d);
            return ret;
        }

        #region 查询控制器报警日志
        /// <summary>
        /// 查询控制器报警日志
        /// </summary>
        /// <param name="routerToken">控制器ID</param>
        /// <param name="sDate">开始时间</param>
        /// <param name="eDate">结束时间</param>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public static DataTable GetRouterWarnLog(string routerToken, string deviceSN, string headAddress, string alertType, string sDate = null, string eDate = null, int pageIndex = 1, int pageSize = 10)
        {
            string strWhere = string.Empty;
            if(!string.IsNullOrWhiteSpace(deviceSN))
            {
                strWhere = string.Format(" AND r.SN LIKE '%{0}%'", deviceSN);
            }

            if (!string.IsNullOrWhiteSpace(headAddress))
            {
                strWhere += string.Format(" AND r.HeadAddress = '{0}'", headAddress);
            }

            if (!string.IsNullOrWhiteSpace(alertType))
            {
                strWhere += string.Format(" AND f.AlertType = '{0}'", alertType);
            }

            if (!string.IsNullOrWhiteSpace(sDate) && string.IsNullOrWhiteSpace(eDate))
            {
                strWhere += string.Format(" AND HappenTime >= '{0}'", sDate);
            }
            else if (string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere += string.Format(" AND HappenTime <= '{0}'", eDate);
            }
            else if (!string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere += string.Format(" AND HappenTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }
            else
            {
                sDate = DateTime.Now.ToString("yyyy-MM-dd");
                eDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                strWhere += string.Format(" AND HappenTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }

            #region Sql语句
            #region 分页

            //            string strSql = @"
            //                WITH Device AS
            //                            (
            //                            SELECT ID, DeviceName, Token, DeviceType, SN FROM Base_DeviceInfo
            //                            ),
            //                            RouterDevice
            //                            AS
            //                            (
            //                            SELECT a.DeviceID, c.DeviceName, c.DeviceType, c.SN, a.HeadAddress, b.Token AS RouterToken FROM Data_MerchDevice a
            //                            INNER JOIN Device b ON b.ID = a.ParentID
            //                            INNER JOIN Device c ON c.ID = a.DeviceID
            //                            WHERE b.Token = '{0}'
            //                            UNION
            //                            SELECT a.DeviceID, c.DeviceName, c.DeviceType, c.SN, a.HeadAddress, b.Token FROM Data_MerchSegment a
            //                            INNER JOIN Device b ON b.ID = a.ParentID
            //                            INNER JOIN Device c ON c.ID = a.DeviceID
            //                            UNION
            //                            SELECT id, DeviceName, DeviceType, SN, '', Token FROM Device
            //                            WHERE DeviceType = 0 AND Token = '{0}'
            //                            ),
            //                            Alarm AS 
            //                            (
            //                            SELECT ID, ICCardID, MerchID, f.DeviceID, r.DeviceName, r.DeviceType, r.SN, AlertType, HappenTime, EndTime, [State], LockGame, LockMember, AlertContent, r.HeadAddress, r.RouterToken FROM flw_game_alarm f
            //                            INNER JOIN RouterDevice r ON r.DeviceID = f.DeviceID
            //                            WHERE r.RouterToken = '{0}' {1}
            //                            )
            //                            SELECT ID, ICCardID, MerchID, DeviceID, DeviceName, SN,
            //                            (CASE DeviceType WHEN 0 THEN '控制器' WHEN 1 THEN '提(售)币机' WHEN 2 THEN '存币机' WHEN 3 THEN '出票器' WHEN 4 THEN '卡头' END) AS DeviceType,
            //                            AlertType, CONVERT(varchar(100), HappenTime, 120) AS HappenTime, 
            //                            CONVERT(varchar(100), EndTime, 120) AS EndTime, [State], LockGame, LockMember, AlertContent, HeadAddress, RouterToken
            //                            FROM 
            //                            (
            //                                SELECT  ROW_NUMBER() OVER (ORDER BY HappenTime DESC) rownumber , ID, ICCardID, MerchID, DeviceID, DeviceName, DeviceType, SN, AlertType, HappenTime, EndTime, [State], LockGame, LockMember, AlertContent, HeadAddress, RouterToken
            //                                FROM Alarm
            //                            ) AS a
            //                            WHERE rownumber BETWEEN {2} AND {3}"; 
            #endregion

            string strSql = @"
                            WITH Device AS
                            (
                            SELECT ID, DeviceName, Token, DeviceType, SN FROM Base_DeviceInfo
                            ),
                            RouterDevice
                            AS
                            (
                            SELECT a.DeviceID, c.DeviceName, c.DeviceType, c.SN, a.HeadAddress, b.Token AS RouterToken FROM Data_MerchDevice a
                            INNER JOIN Device b ON b.ID = a.ParentID
                            INNER JOIN Device c ON c.ID = a.DeviceID
                            WHERE b.Token = '{0}'
                            UNION
                            SELECT a.DeviceID, c.DeviceName, c.DeviceType, c.SN, a.HeadAddress, b.Token FROM Data_MerchSegment a
                            INNER JOIN Device b ON b.ID = a.ParentID
                            INNER JOIN Device c ON c.ID = a.DeviceID
                            UNION
                            SELECT id, DeviceName, DeviceType, SN, '', Token FROM Device
                            WHERE DeviceType = 0 AND Token = '{0}'
                            ),
                            Alarm AS 
                            (
                            SELECT ID, ICCardID, MerchID, f.DeviceID, r.DeviceName, r.DeviceType, r.SN, AlertType, HappenTime, EndTime, [State], LockGame, LockMember, AlertContent, r.HeadAddress, r.RouterToken FROM flw_game_alarm f
                            INNER JOIN RouterDevice r ON r.DeviceID = f.DeviceID
                            WHERE r.RouterToken LIKE '%{0}%' {1}
                            )
                            SELECT ID, ICCardID, MerchID, DeviceID, DeviceName, SN,
                            (CASE DeviceType WHEN 0 THEN '控制器' WHEN 1 THEN '提(售)币机' WHEN 2 THEN '存币机' WHEN 3 THEN '出票器' WHEN 4 THEN '卡头' END) AS DeviceType,
                            AlertType, CONVERT(varchar(100), HappenTime, 120) AS HappenTime, 
                            CONVERT(varchar(100), EndTime, 120) AS EndTime, [State], LockGame, LockMember, AlertContent, HeadAddress, RouterToken
                            FROM Alarm
                            ORDER BY HappenTime DESC
                            ";
            #endregion

            strSql = string.Format(strSql, routerToken, strWhere, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);

            DataTable table = XCCloudRS232BLL.ExecuterSqlToTable(strSql);
            return table;
        }
        #endregion
    }
}
