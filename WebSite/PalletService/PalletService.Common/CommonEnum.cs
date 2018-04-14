using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PalletService.Common
{
    public enum TicketEnum
    { 
        FoodSale = 0,//套餐销售
        member = 1,//会员信息
        Recharge = 2,//充值
        ExitCoin = 3,//退币
        Fee = 4,//送币
        Save=5, //存币
        Currency=6,//提币
        Point=7,//积分换币
        Goods=8,//销售业务
        TicketInfo=9,//门票销售
    }


    /// <summary>
    /// 设备状态
    /// </summary>
    public enum DeviceState
    {
        Offline = 0,//设备离线
        Normal = 1,// 设备正常
        OutofMoney = 2,//出币中
        Fault = 3,//设备故障
        Locking = 4//设备锁定
    }




    public enum TxtLogType
    {
        SystemInit = 0,//系统初始化
        UPDService = 1,//UDP服务
        TCPService = 2,//TCP服务
        WeiXin = 3,//微信
        WeiXinPay = 4//微信支付
    }

    public enum TxtLogContentType
    {
        Common = 0,//系统初始化
        Exception = 1,//异常
        Debug = 2 //支付支付使用
    }

    public enum TxtLogFileType
    {
        Day = 0,//按日期生成文件，批量记录
        Time = 1//按时间生成文件，批量记录
    }

    /// <summary>
    /// tcpsocket消息类型
    /// </summary>
    public enum TCPMessageType
    {
        服务器应答出错 = 400,
        用户客户端注册 = 0,
        读取机器名 = 1,
        读取加密狗 = 2,
        读取身份证 = 3,
        读取IC卡 = 4,
        办理新IC卡 = 5,
        退卡 = 6,
        写入IC卡 = 7, 
        打印小票 = 8,
        出币= 9,
        读取加密狗和机器名 = 10
    }


    /// <summary>
    /// socket客户端类型
    /// </summary>
    public enum SocketClientType
    {
        UnknownClient = 0,//未知客户端
        WebClient = 1,//web客户端
        Client = 2//接口访问的客户端,通过接口请求
    }
}
