using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Model.XCGameManager;

namespace XCCloudService.Business.XCGameMana
{
    public class Tcps
    {
        public bool show(string ManageType, string DataJson)
        {
            string xcGameDBName = "XCGameManagerDB";
            ITCPService tcpService = BLLContainer.Resolve<ITCPService>();
            string sql = "exec InsertTCP @ManageType,@DataJson,@Return output ";
            SqlParameter[] parameters = new SqlParameter[3];
            parameters[0] = new SqlParameter("@ManageType", ManageType);
            parameters[1] = new SqlParameter("@DataJson", DataJson);
            parameters[2] = new SqlParameter("@Return", 0);
            parameters[2].Direction = System.Data.ParameterDirection.Output;
            t_TCP member = tcpService.SqlQuery(sql, xcGameDBName, parameters).FirstOrDefault<t_TCP>();
            if (member == null)
            {
                return false;
            }
            return true;
        }
        public void del()
        {
            string xcGameDBName = "XCGameManagerDB";
            ITCPService tcpService = BLLContainer.Resolve<ITCPService>();
            DateTime time = DateTime.Now.AddDays(-3);
            var menulist = tcpService.GetModels(p => p.CreateTime > time).ToList();
            if (menulist.Count > 0)
            {
                
                for (int i = 0; i < menulist.Count; i++)
                {
                   
                    tcpService.Delete(menulist[i], xcGameDBName);
                }
            }
        }
    }
}
