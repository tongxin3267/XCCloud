using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Common.Enum;


namespace XCCloudService.Business
{
    public class SMSBusiness
    {
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="type">0-（广告类1012812）;1-（验证码类1012818）</param>
        /// <param name="mobile"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string Send(string type,string mobile,string body)
        {
            string smsname = CommonConfig.SMSName;
            string smspassword = CommonConfig.SMSPassWord;
            string smsid = GetSMSType(type);
            string url = "http://cf.51welink.com/submitdata/service.asmx";
            Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");
            byte[] byteArray = encoding.GetBytes(body); //转化
            string pl = Encoding.UTF8.GetString(byteArray);
            string strUrl = string.Format("{0}/g_Submit?sname={1}&spwd={2}&scorpid=&sprdid={3}&sdst={4}&smsg={5}",
                url,
                smsname,
                smspassword,
                smsid,
                mobile,
                pl
                );
            HttpWebRequest hwrq = (HttpWebRequest)WebRequest.Create(strUrl);
            hwrq.Method = "GET";
            HttpWebResponse hwrp = (HttpWebResponse)hwrq.GetResponse();
            Stream stream = hwrp.GetResponseStream();
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            string state = doc["CSubmitState"]["State"].InnerXml;
            return state;
        }

        /// <summary>
        /// 获取短信类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSMSType(string type)
        {
            if (int.Parse(type) == (int)(SMSType.Advertisement))
            {
                return "1012812";//广告类 
            }
            else if (int.Parse(type) == (int)(SMSType.VerificationCode))
            {
                return "1012818";//验证码类
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取短信余额
        /// </summary>
        /// <returns></returns>
        public static void GetBalance(string pid,out string state,out int remain)
        {
            string smsname = System.Configuration.ConfigurationManager.AppSettings["SmsName"] ?? "";
            string smspassword = System.Configuration.ConfigurationManager.AppSettings["SmsPassWord"] ?? "";
            string strUrl = string.Format("{0}/Sm_GetRemain?sname={1}&spwd={2}&scorpid=&sprdid={3}",
                "http://cf.51welink.com/submitdata/service.asmx", smsname, smspassword, pid);
            HttpWebRequest hwrq = (HttpWebRequest)WebRequest.Create(strUrl);
            hwrq.Method = "GET";
            HttpWebResponse hwrp = (HttpWebResponse)hwrq.GetResponse();
            Stream stream = hwrp.GetResponseStream();
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            state = doc["CRemain"]["State"].InnerXml;
            remain = int.Parse(doc["CRemain"]["Remain"].InnerXml);
        }

        /// <summary>
        /// pid是否有效
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static bool CheckPid(string pid)
        {
            if (!pid.Equals("1012812") && !pid.Equals("1012818"))
            {
                return false;//广告类 
            }
            return true;
        }

        /// <summary>
        /// 获取短信验证码
        /// </summary>
        /// <returns></returns>
        public static bool GetSMSCode(out string code)
        {
            int num = 1;
            code = Utils.getNumRandomCode(6);

            //如果生成的验证码与缓存中的重复
            while (num <= 3 && SMSCodeCache.IsExist(code))
            {
                num += 1;
                code = Utils.getNumRandomCode(6);
            }

            if (num == 3)
            {
                if (SMSCodeCache.IsExist(code))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool SendSMSCode(string templateID,string mobile,string code,out string errMsg)
        {
            errMsg = string.Empty;
            string templateContent = string.Empty;
            string suffix = string.Empty;
            bool userSuffix = false;

            if (!GetSMSTemplateContent(templateID, out templateContent, out userSuffix))
            {
                errMsg = "短信模板不存在";
                return false;
            }

            if (!GetSMSSuffix(out suffix))
            {
                errMsg = "短信后缀不存在";
                return false;
            }

            string body = string.Format(templateContent, code) + ( userSuffix == true ? suffix:"");
            string state = Send("1", mobile, body);
            if (state == "0")
            {
                return true;
            }
            else
            {
                errMsg = "短信发送失败（状态:" + state + "）";
                return false;
            }      
        }

        /// <summary>
        /// 获取短信模板内容
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="templateContent">模板内容</param>
        /// <returns></returns>
        private static bool GetSMSTemplateContent(string templateId, out string templateContent, out bool useSmssuffix)
        {
            useSmssuffix = false;
            templateContent = string.Empty;
            string xmlFilePath = HttpContext.Current.Server.MapPath("/Config/SMSCodeTemplate.xml");
            XmlDocument xd = new XmlDocument();
            xd.Load(xmlFilePath);
            var node = xd.SelectSingleNode(string.Format("/sms/smstemplates/smstemplate[@id={0}]", templateId));
            if (node == null)
            {
                return false;
            }

            var templateNode = node.SelectSingleNode("content");
            if (templateNode == null)
            {
                return false;
            }

            var smssuffix = node.SelectSingleNode("smssuffix");
            if (smssuffix == null)
            {
                return false;
            }

            useSmssuffix = (smssuffix.InnerXml.Trim() == "1") ? true : false;

            templateContent = templateNode.InnerXml.Trim();
            return true;
        }

        /// <summary>
        /// 获取短信后缀
        /// </summary>
        /// <returns></returns>
        private static bool GetSMSSuffix(out string suffix)
        {
            suffix = string.Empty;
            string xmlFilePath = HttpContext.Current.Server.MapPath("/Config/SMSCodeTemplate.xml");
            XmlDocument xd = new XmlDocument();
            xd.Load(xmlFilePath);
            var node = xd.SelectSingleNode("/sms/smssuffix");
            if (node == null)
            {
                return false;
            }

            suffix = node.InnerXml.Trim();
            return true;
        }
    }
}