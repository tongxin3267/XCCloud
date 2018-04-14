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
    public class UDPSocketAnswerBusiness
    {
        private static UInt32 sn = 0 ;

        public static Hashtable UDPSocketAnswer
        {
            set { UDPSocketAnswerCache.UDPSocketAnswer = value; }
            get { return UDPSocketAnswerCache.UDPSocketAnswer; }
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
        public static string SetAnswer(UDPSocketAnswerModel answerModel)
        {
            UDPSocketAnswerCache.AddAnswer(answerModel.SN, answerModel);
            return sn.ToString();
        }


        /// <summary>
        /// 获取应答对象
        /// </summary>
        /// <param name="sn"></param>
        public static UDPSocketAnswerModel GetAnswerModel(string sn)
        {
            return (UDPSocketAnswerModel)UDPSocketAnswerCache.GetAnswer(sn);
        }

        /// <summary>
        /// 流水号对应的缓存应答对象是否存在
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static bool ExistSN(string sn)
        {
            return UDPSocketAnswerCache.Exist(sn);
        }

        public static void Remove(string sn)
        {
            UDPSocketAnswerCache.Remove(sn);
        }
    }
}
