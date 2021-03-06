﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCCloudService.SocketService.UDP
{
    public enum TransmiteEnum
    {
        雷达注册授权 = 0xf0,
        设备状态变更通知 = 0x10,
        雷达心跳 = 0xf1,
        远程设备控制指令 = 0x81,
        雷达通知指令 = 0x12,
        远程门店账目查询指令 = 0x70,
        远程门店账目应答通知指令 = 0x21,
        远程门店会员卡数据请求 = 0x72,
        远程门店门票数据请求 = 0x73,
        远程门店门票操作请求 = 0x74,
        远程门店彩票数据请求 = 0x75,
        远程门店彩票操作请求 = 0x76,
        远程门店出票条码数据请求 = 0x77,
        远程门店出票条码操作请求 = 0x78,
        远程门店运行参数数据请求 = 0x79,
        远程门店会员转账操作请求 = 0x7a,
        远程门店员工手机号校验请求 = 0x7b,
        黄牛卡信息查询请求 = 0x7c,

        雷达注册授权响应 = 0x90,
        设备状态变更通知响应 = 0x80,
        雷达心跳响应 = 0x91,
        远程设备控制指令响应 = 0x11,
        雷达通知指令响应 = 0x82,
        远程门店账目查询指令响应 = 0x20,
        远程门店账目应答通知指令响应 = 0x71,
        远程门店会员卡数据请求响应 = 0x22,
        远程门店门票数据请求响应 = 0x23,
        远程门店门票操作请求响应 = 0x24,
        远程门店彩票数据请求响应 = 0x25,
        远程门店彩票操作请求响应 = 0x26,
        远程门店出票条码数据请求响应 = 0x27,
        远程门店出票条码操作请求响应 = 0x28,
        远程门店运行参数数据请求响应 = 0x29,
        远程门店会员转账操作请求响应 = 0x2a,
        远程门店员工手机号校验请求响应 = 0x2b,
        黄牛卡信息查询请求响应 = 0x2c
     }

    public enum UDPRecordType
    { 
        雷达注册 = 0,
        雷达心跳 = 1,
        设备状态变更通知 = 2,
        远程设备控制发送 = 3,
        远程设备控制回复 = 4,
        雷达通知 = 5
    }



    public enum LockTypeEnum
    {
        超限解锁一次 = 1,
        超限解锁当日 = 2,
        机头锁定 = 3,
        机头解锁 = 4,
    }
}
