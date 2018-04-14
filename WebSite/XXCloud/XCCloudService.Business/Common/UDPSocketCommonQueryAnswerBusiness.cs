using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.CacheService;
using XCCloudService.Model.CustomModel.Common;

namespace XCCloudService.Business.Common
{
    public class UDPSocketCommonQueryAnswerBusiness
    {

        public static Hashtable Answer
        {
            set { UDPSocketCommonQueryAnswerCache.SocketAnswer = value; }
            get { return UDPSocketCommonQueryAnswerCache.SocketAnswer; }
        }


        public static void AddAnswer(string id, UDPSocketCommonQueryAnswerModel answerModel)
        {
            UDPSocketCommonQueryAnswerCache.Remove(id);
            UDPSocketCommonQueryAnswerCache.AddAnswer(id, answerModel);
        }

        /// <summary>
        /// 获取应答对象
        /// </summary>
        /// <param name="sn"></param>
        public static UDPSocketCommonQueryAnswerModel GetAnswerModel(string id)
        {
            return (UDPSocketCommonQueryAnswerModel)UDPSocketCommonQueryAnswerCache.GetAnswer(id);
        }

        public static UDPSocketCommonQueryAnswerModel GetAnswerModel(string id, int status)
        {
            UDPSocketCommonQueryAnswerModel model = (UDPSocketCommonQueryAnswerModel)UDPSocketCommonQueryAnswerCache.GetAnswer(id);
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
        public static bool ExistSN(string id)
        {
            return UDPSocketStoreQueryAnswerCache.Exist(id);
        }

        public static bool ExistSN(string id, int status)
        {
            UDPSocketCommonQueryAnswerModel model = (UDPSocketCommonQueryAnswerModel)UDPSocketStoreQueryAnswerCache.GetAnswer(id);
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


        public static void Remove(string id)
        {
            UDPSocketStoreQueryAnswerCache.Remove(id);
        }

        public static void UpdateStuate(string id, int status)
        {
            UDPSocketCommonQueryAnswerModel answerModel = (UDPSocketCommonQueryAnswerModel)UDPSocketStoreQueryAnswerCache.GetAnswer(id);
            answerModel.Status = status;
        }
    }
}
