using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.Common;

namespace XCCloudService.Business.Common
{
    /// <summary>
    /// UPDsocket应答缓存
    /// </summary>
    public class UDPSocketStoreQueryAnswerBusiness
    {
        private static UInt32 sn = 0 ;

        public static Hashtable Answer
        {
            set { UDPSocketStoreQueryAnswerCache.SocketAnswer = value; }
            get { return UDPSocketStoreQueryAnswerCache.SocketAnswer; }
        }

        /// <summary>
        /// 获取SN
        /// </summary>
        /// <returns></returns>
        public static string GetSN()
        {
            if (sn == UInt32.MaxValue)
            {
                sn = 0;
            }
            sn++;
            return sn.ToString();
        }
        
        /// <summary>
        /// 设置应答缓存
        /// </summary>
        /// <param name="answerModel">应答模式对象，用于接收端无响应时重新发送</param>
        /// <returns></returns>
        public static string SetAnswer(UDPSocketStoreQueryAnswerModel answerModel)
        {
            UDPSocketStoreQueryAnswerCache.AddAnswer(answerModel.SN, answerModel);
            return sn.ToString();
        }


        public static string AddAnswer(string sn,string storeId,string radarToken)
        {
            UDPSocketStoreQueryAnswerModel answerModel = new UDPSocketStoreQueryAnswerModel(sn, storeId, radarToken);
            UDPSocketStoreQueryAnswerCache.AddAnswer(sn, answerModel);
            return sn.ToString();
        }

        /// <summary>
        /// 获取应答对象
        /// </summary>
        /// <param name="sn"></param>
        public static UDPSocketStoreQueryAnswerModel GetAnswerModel(string sn)
        {
            return (UDPSocketStoreQueryAnswerModel)UDPSocketStoreQueryAnswerCache.GetAnswer(sn);
        }

        public static UDPSocketStoreQueryAnswerModel GetAnswerModel(string sn,int status)
        {
            UDPSocketStoreQueryAnswerModel model = (UDPSocketStoreQueryAnswerModel)UDPSocketStoreQueryAnswerCache.GetAnswer(sn);
            if (model != null)
            {
                if (model.Status == status)
                {
                    return model;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 流水号对应的缓存应答对象是否存在
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static bool ExistSN(string sn)
        {
            return UDPSocketStoreQueryAnswerCache.Exist(sn);
        }

        public static bool ExistSN(string sn,int status)
        {
            UDPSocketStoreQueryAnswerModel model = (UDPSocketStoreQueryAnswerModel)UDPSocketStoreQueryAnswerCache.GetAnswer(sn);
            if (model != null)
            {
                if (model.Status == status)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }


        public static void Remove(string sn)
        {
            UDPSocketStoreQueryAnswerCache.Remove(sn);
        }

        public static void UpdateStuate(string sn,int status)
        {
            UDPSocketStoreQueryAnswerModel answerModel = (UDPSocketStoreQueryAnswerModel)UDPSocketStoreQueryAnswerCache.GetAnswer(sn);
            answerModel.Status = status;
        }
    }
}
