using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGame;

namespace XCCloudService.Business.XCCloud
{
    public class ScheduleBusiness
    {
        //开通班次
        public static bool OpenSchedule(string storeId, int userId, string scheduleName, string workStation, out int currentSchedule,out string openTime, out string errMsg)
        {
            errMsg = string.Empty;
            string checkDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            string sql = "OpenSchedule";
            SqlParameter[] parameters = new SqlParameter[9];
            parameters[0] = new SqlParameter("@CheckDate", checkDate);
            parameters[1] = new SqlParameter("@StoreId", storeId);
            parameters[2] = new SqlParameter("@UserId", userId);
            parameters[3] = new SqlParameter("@scheduleName", scheduleName);
            parameters[4] = new SqlParameter("@workStation", workStation);
            parameters[5] = new SqlParameter("@CurrentSchedule", 0);
            parameters[5].Direction = System.Data.ParameterDirection.Output;
            parameters[6] = new SqlParameter("@OpenTime",System.Data.SqlDbType.VarChar,20);
            parameters[6].Direction = System.Data.ParameterDirection.Output;
            parameters[7] = new SqlParameter("@ErrMsg", System.Data.SqlDbType.VarChar, 50);
            parameters[7].Direction = System.Data.ParameterDirection.Output;
            parameters[8] = new SqlParameter("@ReturnValue", 0);
            parameters[8].Direction = System.Data.ParameterDirection.ReturnValue;
            XCCloudBLL.ExecuteStoredProcedureSentence(sql,parameters);

            int result = Convert.ToInt32(parameters[8].Value);
            if (result >= 1)
            {
                currentSchedule = Convert.ToInt32(parameters[5].Value);
                openTime = parameters[6].Value.ToString();
                return true;
            }
            else
            {
                errMsg = parameters[7].Value.ToString();
                currentSchedule = 0;
                openTime = string.Empty;
                return false;
            }  
        }
    }
}
