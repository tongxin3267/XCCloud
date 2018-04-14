using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Business.Common;
using XCCloudService.Business.WeiXin;
using XCCloudService.Business.XCCloudRS232;
using XCCloudService.Business.XCGame;
using XCCloudService.Business.XCGameMana;
using XCCloudService.CacheService;
using XCCloudService.CacheService.WeiXin;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.WeiXin.Session;
using XCCloudService.SocketService.TCP.Business;
using XCCloudService.SocketService.UDP;

namespace XCCloudService.Utility
{
    public class ApplicationStart
    {
        public static void Init()
        {
            // 在应用程序启动时运行的代码
            try
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "********************************Application Start********************************");
                TestInit();
                TCPSocketInit();
                UDPSocketInit();
                StoreInit();
                StoreDogInit();
                MibleTokenInit();
                MemberTokenInit();
                RS232MibleTokenInit();
                XCCloudStoreInit();
                XCCloudUserInit();
                XCCloudMerchInit();
                XCGameManaDeviceInit();
                XCCloudManaUserInit();
                FilterMobileInit();
            }
            catch(Exception e)
            {
                LogHelper.SaveLog(e.Message);
            }
        }

        public static void TestInit()
        {
            StoreIDDataModel tokenDataModel = new StoreIDDataModel("S0100022", "778852013145", "lijunjie");
            XCCloudUserTokenBusiness.SetUserToken("3", (int)RoleType.StoreUser, tokenDataModel);
        }

        public static void XCGameManaDeviceInit()
        {
            try
            {
                DeviceManaBusiness.Init();
                LogHelper.SaveLog(TxtLogType.SystemInit, "ManaDevice Init Sucess");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "ManaDevice Init Fail..." + Utils.GetException(ex));
            }
        }

        public static void TCPSocketInit()
        {
            try
            {
                XCCloudService.SocketService.TCP.Server webScocket = new XCCloudService.SocketService.TCP.Server();
                TCPServiceBusiness.Server = webScocket;
                webScocket.Start(XCCloudService.SocketService.TCP.Common.TcpConfig.Port); 
                LogHelper.SaveLog(TxtLogType.SystemInit, "TCP Init Sucess");
            }
            catch(Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "TCP Init Fail..." + Utils.GetException(ex));
            }
        }

        public static void StoreInit()
        {
            try
            {
                StoreBusiness.StoreInit();
                LogHelper.SaveLog(TxtLogType.SystemInit, "StoreInit Sucess");
            }
            catch(Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "StoreInit..." + Utils.GetException(ex));
            }     
        }        
     
        public static void StoreDogInit()
        {
            try
            {
                StoreBusiness.StoreDogInit();
                LogHelper.SaveLog(TxtLogType.SystemInit, "XCGameManaStoreDogInit Sucess");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "XCGameManaStoreInit..." + Utils.GetException(ex));
            }
        }



        public static void UDPSocketInit()
        {
            try
            {
                XCCloudService.SocketService.UDP.Server.Init(XCCloudService.SocketService.UDP.Common.UDPConfig.Port);
                LogHelper.SaveLog(TxtLogType.SystemInit, "UDP Init Sucess");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "UDP Init Fail..." + Utils.GetException(ex));
            }
        }

        public static void MibleTokenInit()
        {
            try
            {
                MobileTokenBusiness.Init();
                LogHelper.SaveLog(TxtLogType.SystemInit, "MibleTokenInit Sucess");
            }
            catch(Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "MibleTokenInit Fail..." + Utils.GetException(ex));
            }
        }

        public static void RS232MibleTokenInit()
        {
            try
            {
                MobileTokenBusiness.SetRS232MobileToken();
                LogHelper.SaveLog(TxtLogType.SystemInit, "RS232MibleTokenInit Sucess");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "RS232MibleTokenInit Fail..." + Utils.GetException(ex));
            }
        }

        public static void MemberTokenInit()
        {
            try
            {
                MemberTokenBusiness.Init();
                LogHelper.SaveLog(TxtLogType.SystemInit, "MemberTokenInit Sucess");
            }
            catch(Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "MemberTokenInit Fail..." + Utils.GetException(ex));
            }
        }

        public static void XCCloudStoreInit()
        {
            try
            {
                XCCloudService.Business.XCCloud.XCCloudStoreBusiness.Init();
                LogHelper.SaveLog(TxtLogType.SystemInit, "XCCloudStore Sucess");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "XCCloudStore Fail..." + Utils.GetException(ex));
            }
        }

        public static void XCCloudMerchInit()
        {
            try
            {
                XCCloudService.Business.XCCloud.MerchBusiness.Init();
                LogHelper.SaveLog(TxtLogType.SystemInit, "XCCloudMerch Sucess");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "XCCloudMerch Fail..." + Utils.GetException(ex));
            }
        }

        public static void XCCloudUserInit()
        {
            try
            {
                XCCloudService.Business.XCCloud.UserBusiness.XcUserInit();
                LogHelper.SaveLog(TxtLogType.SystemInit, "XcUserInit Sucess");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "XcUserInit..." + Utils.GetException(ex));
            }
        }

        public static void XCCloudManaUserInit()
        {
            try
            {
                XCCloudManaUserTokenBusiness.Init();
                LogHelper.SaveLog(TxtLogType.SystemInit, "XcManaUserInit Sucess");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "XcManaUserInit..." + Utils.GetException(ex));
            }
        }

        public static void FilterMobileInit()
        {
            try
            {
                string mobileStr = System.Configuration.ConfigurationManager.AppSettings["filterMobile"].ToString();
                bool isSMSTest = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["isSMSTest"].ToString());
                string[] mobileArr = mobileStr.Split(',');
                for (int i = 0; i < mobileArr.Length; i++)
                {
                    FilterMobileBusiness.AddMobile(mobileArr[i]);
                }
                FilterMobileBusiness.IsTestSMS = isSMSTest;
            }
            catch(Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "FilterMobileInit..." + Utils.GetException(ex));
            }
        }
    }
}
