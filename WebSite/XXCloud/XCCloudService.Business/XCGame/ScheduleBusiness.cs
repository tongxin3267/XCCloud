using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGame;

namespace XCCloudService.Business.XCGame
{
    public class ScheduleBusiness
    {
        public static bool OpenSchedule(string dbName, int userId, string macAddress, string diskId, string workStation, out int currentSchedule, out string errMsg)
        {
            errMsg = string.Empty;
            string checkDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            string sql = "OpenSchedule";
            SqlParameter[] parameters = new SqlParameter[8];
            parameters[0] = new SqlParameter("@CheckDate", checkDate);
            parameters[1] = new SqlParameter("@UserId", userId);
            parameters[2] = new SqlParameter("@macAddress", macAddress);
            parameters[3] = new SqlParameter("@diskId", diskId);
            parameters[4] = new SqlParameter("@workStation", workStation);
            parameters[5] = new SqlParameter("@CurrentSchedule", 0);
            parameters[5].Direction = System.Data.ParameterDirection.Output;
            parameters[6] = new SqlParameter("@OffLineUserId", 0);
            parameters[6].Direction = System.Data.ParameterDirection.Output;
            parameters[7] = new SqlParameter("@ReturnValue", 0);
            parameters[7].Direction = System.Data.ParameterDirection.ReturnValue;
            XCGameBLL.ExecuteStoredProcedureSentence(sql, dbName, parameters);

            int result = Convert.ToInt32(parameters[6].Value);
            if (result >= 1)
            {
                currentSchedule = Convert.ToInt32(parameters[5].Value);
                return true;
            }
            else
            { 
                errMsg = "开班出现错误";
                currentSchedule = 0;
                return false;
            }  
        }
    }
}
