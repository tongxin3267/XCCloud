using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XCCloudService.SocketService.UDP
{
    public class CallBackEvent
    {
        #region 事件

        public delegate void 机头参数设置事件(string rAddress, string hAddress);
        public static event 机头参数设置事件 OnSetParamesEvent;
        public static void SetParamesEvent(string rAddress, string hAddress)
        {
            if (OnSetParamesEvent != null)
            {
                OnSetParamesEvent(rAddress, hAddress);
            }
        }

        public delegate void 机头地址变更事件(string rAddress, string hAddress, string MCUDID);
        public static event 机头地址变更事件 OnChangeAddressEvent;
        public static void ChangeAddressEvent(string rAddress, string hAddress, string MCUDID)
        {
            if (OnChangeAddressEvent != null)
            {
                OnChangeAddressEvent(rAddress, hAddress, MCUDID);
            }
        }

        public delegate void 充值事件(string ICCardID);
        public static event 充值事件 OnChargeCoinEvent;
        public static void ChargeCoinEvent(string ICCardID)
        {
            if (OnChargeCoinEvent != null)
            {
                OnChargeCoinEvent(ICCardID);
            }
        }

        public delegate void 报警事件(TransmiteObject.机头报警结构 headInfo);
        public static event 报警事件 OnAlert;
        public static void Alert(TransmiteObject.机头报警结构 headInfo)
        {
            if (OnAlert != null)
            {
                OnAlert(headInfo);
            }
        }

        public delegate void 机头状态汇报事件(TransmiteObject.机头状态结构[] 机头);
        public static event 机头状态汇报事件 OnHeadStatusEvent;
        public static void HeadStatus(TransmiteObject.机头状态结构[] 机头)
        {
            if (OnHeadStatusEvent != null)
            {
                OnHeadStatusEvent(机头);
            }
        }

        public delegate void 路由器时间管理事件(string date, bool isSet);
        public static event 路由器时间管理事件 OnRouteTimeEvent;
        public static void RouteTime(string date, bool isSet)
        {
            if (OnRouteTimeEvent != null)
            {
                OnRouteTimeEvent(date, isSet);
            }
        }

        public delegate void 系统结账通知事件();
        public static event 系统结账通知事件 OnSystemCheckDate;
        public static void SystemCheckDate()
        {
            if (OnSystemCheckDate != null)
            {
                OnSystemCheckDate();
            }
        }

        public delegate void 设置LOGO图片事件(byte[] image, int version);
        public static event 设置LOGO图片事件 OnSetLogoImageEvent;
        public static void SetLogoImageEvent(byte[] image, int version)
        {
            if (OnSetLogoImageEvent != null)
            {
                OnSetLogoImageEvent(image, version);
            }
        }

        public delegate void 设置打印文字事件(TransmiteObject.打印文字结构 printText);
        public static event 设置打印文字事件 OnSetPrintTextEvent;
        public static void SetPrintTextEvent(TransmiteObject.打印文字结构 printText)
        {
            if (OnSetPrintTextEvent != null)
            {
                OnSetPrintTextEvent(printText);
            }
        }

        public delegate void 设置打印顺序事件(byte[] index, int version);
        public static event 设置打印顺序事件 OnSetPrintIndexEvent;
        public static void SetPrintIndexEvent(byte[] index, int version)
        {
            if (OnSetPrintIndexEvent != null)
            {
                OnSetPrintIndexEvent(index, version);
            }
        }

        public delegate void 游戏机设置变更();
        public static event 游戏机设置变更 OnGameChangeEvent;
        public static void GameChangeEvent()
        {
            if (OnGameChangeEvent != null)
            {
                OnGameChangeEvent();
            }
        }

        public delegate void 时间同步(DateTime date);
        public static event 时间同步 OnTimeSynchro;
        public static void TimeSynchro(DateTime date)
        {
            if (OnTimeSynchro != null)
            {
                OnTimeSynchro(date);
            }
        }

        public delegate void 服务器断开();
        public static event 服务器断开 OnServerDisconnect;
        public static void ServerDisconnect()
        {
            if (OnServerDisconnect != null)
            {
                OnServerDisconnect();
            }
        }

        public delegate void 客户端断开(string gid);
        public static event 客户端断开 OnClientDisconnect;
        public static void ClientDisconnect(string gid)
        {
            if (OnClientDisconnect != null)
            {
                OnClientDisconnect(gid);
            }
        }

        public delegate void 后台结账();
        public static event 后台结账 OnCheckAccount;
        public static void CheckAccount()
        {
            if (OnCheckAccount != null)
            {
                OnCheckAccount();
            }
        }

        public delegate void 机头锁定状态事件(string rAddress, string hAddress, LockTypeEnum lockType);
        public static event 机头锁定状态事件 OnHeadLockType;
        public static void HeadLockType(string rAddress, string hAddress, LockTypeEnum lockType)
        {
            if (OnHeadLockType != null)
            {
                OnHeadLockType(rAddress, hAddress, lockType);
            }
        }

        public delegate void 下载文件应答(Stream fileStream);
        public static event 下载文件应答 OnDownLoadComplite;
        public static void DownLoadComplite(byte[] data)
        {
            if (OnDownLoadComplite != null)
            {
                Stream s = new MemoryStream(data);
                OnDownLoadComplite(s);
            }
        }

        public delegate void 获取本店密码(string password);
        public static event 获取本店密码 OnGetPassword;
        public static void GetPassword(string password)
        {
            if (OnGetPassword != null)
            {
                OnGetPassword(password);
            }
        }

        public delegate void 启动远程被动退分检查(string IC卡号);
        public static event 启动远程被动退分检查 OnCheckCard;
        public static void CheckCard(string IC卡号)
        {
            if (OnCheckCard != null)
            {
                OnCheckCard(IC卡号);
            }
        }

        public delegate void 远程被动退分应答(string 错误消息, string IC卡号, bool 是否允许退分);
        public static event 远程被动退分应答 OnRemoteBackCoin;
        public static void RemoteBackCoin(string 错误消息, string IC卡号, bool 是否允许退分)
        {
            if (OnRemoteBackCoin != null)
            {
                OnRemoteBackCoin(错误消息, IC卡号, 是否允许退分);
            }
        }

        public delegate void 卡头复位();
        public static event 卡头复位 OnSystemRestart;
        public static void SystemRestart()
        {
            if (OnSystemRestart != null)
            {
                OnSystemRestart();
            }
        }

        public delegate void 机头锁定解锁(string 路由器段号, string 机头地址, bool 是否锁定, int 锁定类别);
        public static event 机头锁定解锁 OnLockUnLockHead;
        public static void LockUnLockHead(string 路由器段号, string 机头地址, bool 是否锁定, int 锁定类别)
        {
            if (OnLockUnLockHead != null)
            {
                OnLockUnLockHead(路由器段号, 机头地址, 是否锁定, 锁定类别);
            }
        }

        public delegate void 下载进度(int pIndex, int pCount);
        public static event 下载进度 OnDownloadProcess;
        public static void DownloadProcess(int pIndex, int pCount)
        {
            if (OnDownloadProcess != null)
            {
                OnDownloadProcess(pIndex, pCount);
            }
        }

        public delegate void 软件版本请求(TransmiteObject.请求软件版本号结构 req);
        public static event 软件版本请求 OnSoftVersionRequest;
        public static void SoftVersionRequest(TransmiteObject.请求软件版本号结构 req)
        {
            if (OnSoftVersionRequest != null)
            {
                OnSoftVersionRequest(req);
            }
        }

        public delegate void 软件版本应答(TransmiteObject.软件版本号应答结构 soft);
        public static event 软件版本应答 OnSoftVersionAnswer;
        public static void SoftVersionAnswer(TransmiteObject.软件版本号应答结构 soft)
        {
            if (OnSoftVersionAnswer != null)
            {
                OnSoftVersionAnswer(soft);
            }
        }

        #endregion
    }
}
