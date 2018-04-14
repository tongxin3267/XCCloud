using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.CacheService;
using XCCloudService.Model.CustomModel.Common;


namespace XCCloudService.Business.Common
{
    public class TCPAnswerSocketBusiness
    {
        public static void Add(string clientId, Socket socketClient)
        {
            TCPAnswerSocketModel model = new TCPAnswerSocketModel(socketClient, clientId);
            TCPAnswerSocketCache.Add(clientId, model);
        }

        public static bool ExistSocket(string clientId,ref TCPAnswerSocketModel model)
        { 
            if(TCPAnswerSocketCache.Exist(clientId))
            {
                model = (TCPAnswerSocketModel)(TCPAnswerSocketCache.GetValue(clientId));
                return true;
            }
            else
            {
                model = null;
                return false;
            }
        }

        public static void Remove(string clientId)
        {
            TCPAnswerSocketCache.Remove(clientId);
        }
    }
}
