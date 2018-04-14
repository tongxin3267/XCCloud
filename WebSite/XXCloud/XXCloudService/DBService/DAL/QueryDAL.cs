using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.DBService.Model;
using XCCloudService.DBService.SQLDAL;
using XCCloudService.Common;
using System.Data;
using System.Text;

namespace XCCloudService.DBService.DAL
{
    public class QueryDAL
    {
        /// <summary>
        /// 获取初始模板数据
        /// </summary>
        /// <param name="type">模板名</param>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public static void GetInit(string pageName, string processName, int userId, ref List<InitModel>  listInitModel, ref List<Dict_SystemModel> listDict_SystemModel)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(" declare @PageName varchar(50) = '{0}' ", pageName));
            sb.Append(string.Format(" declare @ProcessName varchar(50) = '{0}' ", processName));
            sb.Append(string.Format(" declare @UserID int = {0} ",userId));
            sb.Append(string.Format(" if not exists (select 0 from Search_Template where UserID = @UserID ) "));
            sb.Append(string.Format("   set @UserID = 0 "));
            sb.Append(@" select * from Search_Template a inner join Search_Template_Detail b on a.ID = b.TempID  
                         where UserID = @UserID and PageName = @PageName and ProcessName = @ProcessName ");
            sb.Append(@" select * from Dict_System ");
            DataAccess ac = new DataAccess(DataAccessDB.XCCloudDB);
            DataSet ds = ac.ExecuteQuery(sb.ToString());
            listInitModel = Utils.GetModelList<InitModel>(ds.Tables[0]);
            listDict_SystemModel = Utils.GetModelList<Dict_SystemModel>(ds.Tables[1]);
        }

        public static void GetInitModel(int id, string field, ref InitModel initModel, ref List<Dict_SystemModel> listDict_SystemModel)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(" declare @TempID int = {0} ", id));
            sb.Append(string.Format(" declare @FieldName varchar(50) = '{0}' ", field));
            sb.Append(string.Format(" declare @DictID int = -1 "));
            sb.Append(@" select top 1 * from Search_Template_Detail where TempID = @TempID and FieldName = @FieldName ");
            sb.Append(@" select @DictID=DictID from Search_Template_Detail where TempID = @TempID and FieldName = @FieldName ");
            sb.Append(@" select * from Dict_System where PID=@DictID ");
            DataAccess ac = new DataAccess(DataAccessDB.XCCloudDB);
            DataSet ds = ac.ExecuteQuery(sb.ToString());
            initModel = Utils.GetModelList<InitModel>(ds.Tables[0]).FirstOrDefault();
            listDict_SystemModel = Utils.GetModelList<Dict_SystemModel>(ds.Tables[1]);
        }
    }
}