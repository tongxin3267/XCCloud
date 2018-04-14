using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Common.Enum
{
    /// <summary>
    /// 应用系统的Id
    /// </summary>
    public enum SysIdEnum
    { 
        WXSAPP = 0,//微信小程序
        Radar = 101,//雷达服务程序
        UDP = 102//UDP服务程序
    }


    public enum XCGameManaDeviceStoreType
    { 
        Store = 0,//门店
        Merch =1  //商户
    }

    //设备控制枚举
    public enum DevieControlTypeEnum
    {
        出币 = 1,
        存币 = 2,
        解除报警 = 3,
        远程锁定 = 4,
        远程解锁 = 5,
        投币 = 6,
        退币 = 7
    }

    public enum UDPSocketClientType
    {
        串口通讯服务 = 1,
        数据库服务 = 2,
        地图服务 = 3,
        短信服务 = 4,
        文件服务 = 5,
        后台客户端 = 6,
        吧台客户端 = 7,
        服务中心升级程序 = 8,
        服务中心 = 9,
    }

    /// <summary>
    /// 短信验证码枚举
    /// </summary>
    public enum SMSType
    {
        // 广告类
        Advertisement = 0,

        //验证码类
        VerificationCode = 1
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

    /// <summary>
    /// 充值类型
    /// </summary>
    public enum RechargeType
    {
        Cash = 0, //现金充值
        Coin = 0, //币充值
    }

    /// <summary>
    /// 充值方式
    /// </summary>
    public enum RechargeMode
    {
        OffLine = 0,//线下充值
        OnLine = 1, //线上充值
    }

    public enum TokenType
    { 
        Member = 0,//会员模式
        Mobile = 1,//手机模式
        MemberAndMobile = 2//会员模式与手机模式
    }


    public enum TxtLogType
    {
        [LogFolderAttribute(FolderName = "SystemInit")]
        SystemInit = 0,//系统初始化
        [LogFolderAttribute(FolderName = "UPDService")]
        UPDService = 1,//UDP服务
        [LogFolderAttribute(FolderName = "TCPService")]
        TCPService = 2,//TCP服务
        [LogFolderAttribute(FolderName = "WeiXin")]
        WeiXin = 3,//微信
        [LogFolderAttribute(FolderName = "WeiXinPay")]
        WeiXinPay = 4,//微信支付
        [LogFolderAttribute(FolderName = "Api")]
        Api = 5,
        [LogFolderAttribute(FolderName = "AliPay")]
        AliPay = 6,//支付宝支付
        [LogFolderAttribute(FolderName = "PPosPay")]
        PPosPay = 7, //新大陆支付
        [LogFolderAttribute(FolderName = "LcswPay")]
        LcswPay = 8, //扫呗
        [LogFolderAttribute(FolderName = "DinPay")]
        DinPay = 9, //智付
        [LogFolderAttribute(FolderName = "WorkFlow")]
        WorkFlow = 10, //工作流
        [LogFolderAttribute(FolderName = "WorkFlow")]
        LogDBExcepton = 11 //工作流
    }
    
    public enum TxtLogContentType
    {
        [LogFolderAttribute(FolderName = "Common")]
        Common = 0,//通用数据记录
        [LogFolderAttribute(FolderName = "Exception")]
        Exception = 1,//异常
        [LogFolderAttribute(FolderName = "Debug")]
        Debug = 2, //支付支付使用
        Record = 3//记录特定日志数据
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
        会员客户端注册 = 1,
        推送消息 = 2,
        雷达操作回复 = 3,
        业务授权应答 = 4,
        会员客户端注册应答 = 101
    }

    /// <summary>
    /// tcp应答消息类型
    /// </summary>
    public enum TCPAnswerMessageType
    {
        出币 = 1,
        存币 = 2
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

    public enum SAppMessageType
    {
        MemberFoodSaleNotify = 0,//会员购买套餐
        MemberCoinsOperationNotify = 1
    }

    public enum SAppMessageParamsModelType
    {
        Cache = 0,//缓存
        RealTime = 1//实时参数 
    }

    public enum WeiXinMesageType
    {
        BuySuccessNotify = 0,//购买成功通知
        MemberRechargeNotify = 1,//会员充值通知
        UserRegisterRemind = 2, //新用户注册审批提醒
        OrderPaySuccess = 3, //订单支付成功
        OrderFailSuccess = 4,//订单支付失败
        MerchNewPassword = 5,//商户登录密码
        MerchResetPassword = 6,//商户重置密码
        StoreRegisterRemind = 7,//开店申请通知
        XcUserNewPassword = 8,//莘宸用户登录密码
        XcUserResetPassword = 9,//莘宸用户重置密码
        XCGameGetCoinSuccess = 10//莘宸用户提币成功
    }

    public enum OrderType
    { 
        WeiXin = 0,
        Ali = 1
    }

    //设备绑定状态枚举
    public enum DeviceBoundStateEnum : int
    {
        /// <summary>
        /// 已绑定
        /// </summary>
        [Description("已绑定")]
        Bound = 1,

        /// <summary>
        /// 未绑定
        /// </summary>
        [Description("未绑定")]
        NotBound = 0
    }

    //设备类型枚举
    public enum DeviceTypeEnum : int
    {
        /// <summary>
        /// 控制器
        /// </summary>
        [Description("控制器")]
        Router = 0,

        /// <summary>
        /// 提(售)币机
        /// </summary>
        [Description("售币机")]
        SlotMachines = 1,

        /// <summary>
        /// 存币机
        /// </summary>
        [Description("存币机")]
        DepositMachine = 2,

        /// <summary>
        /// 出票器
        /// </summary>
        [Description("出票器")]
        Clerk = 3,

        /// <summary>
        /// 卡头
        /// </summary>
        [Description("卡头")]
        Terminal = 4
    }

    //分组类型枚举
    //0 娃娃机
    //1 压分机
    //2 推土机
    //3 剪刀机
    //4 彩票机
    //5 枪战机
    //6 VR设备
    //7 鱼机
    public enum GroupTypeEnum : int
    {
        /// <summary>
        /// 娃娃机
        /// </summary>
        [Description("娃娃机")]
        娃娃机 = 0,

        /// <summary>
        /// 压分机
        /// </summary>
        [Description("压分机")]
        压分机 = 1,

        /// <summary>
        /// 推土机
        /// </summary>
        [Description("推土机")]
        推土机 = 2,

        /// <summary>
        /// 剪刀机
        /// </summary>
        [Description("剪刀机")]
        剪刀机 = 3,

        /// <summary>
        /// 彩票机
        /// </summary>
        [Description("彩票机")]
        彩票机 = 4,

        /// <summary>
        /// 枪战机
        /// </summary>
        [Description("枪战机")]
        枪战机 = 5,

        /// <summary>
        /// VR设备
        /// </summary>
        [Description("VR设备")]
        VR设备 = 6,

        /// <summary>
        /// 鱼机
        /// </summary>
        [Description("鱼机")]
        鱼机 = 7
    }

    //分组类型枚举
    //0 未激活
    //1 启用
    //2 停用
    //3 在线
    //4 离线
    //5 报警
    //6 锁定
    //7 工作中
    public enum DeviceStatusEnum : int
    {
        /// <summary>
        /// 未激活
        /// </summary>
        [Description("未激活")]
        未激活 = 0,

        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        启用 = 1,

        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        停用 = 2,

        /// <summary>
        /// 在线
        /// </summary>
        [Description("在线")]
        在线 = 3,

        /// <summary>
        /// 离线
        /// </summary>
        [Description("离线")]
        离线 = 4,

        /// <summary>
        /// 报警
        /// </summary>
        [Description("报警")]
        报警 = 5,

        /// <summary>
        /// 锁定
        /// </summary>
        [Description("锁定")]
        锁定 = 6,

        /// <summary>
        /// 工作中
        /// </summary>
        [Description("工作中")]
        工作中 = 7
    }    

    public enum SettleType
    {
        None = 0,//不采用第三方结算
        Org = 1,//微信支付官方通道
        PPOS = 2,//新大陆
        LKL = 3 //拉卡拉
    }

    public enum StoreState
    {
        Invalid = 0,//无效
        Valid = 1,//有效
        Suspend = 2,//暂停
        Cancel = 3, //注销
        Open=4 //开业
    }

    public enum WorkType
    {
        UserCheck = 0,//用户审核
        StoreCheck = 1//门店审核        
    }

    public enum WorkState
    {
        Pending = 0, //待审核
        Pass = 1, //审核通过
        Reject = 2 //审核拒绝
    }

    public enum RoleType
    {
        XcUser = 0, //莘宸普通员工
        XcAdmin = 1, //莘宸管理员
        StoreUser = 2, //门店用户
        MerchUser = 3 //商户用户
    }

    public enum MerchType
    {
        Normal = 1, //普通商户
        Heavy = 2, //大客户
        Agent = 3 //代理商
    }

    public enum UserType
    {
        Xc = 0,       //莘宸用户
        Normal = 1,  //普通商户
        Heavy = 2,  //大客户
        Agent = 3, //代理商
        Store = 4, //门店用户
        StoreBoss = 5 //门店老板
    }

    public enum CreateType
    {
        Xc = 1,   //莘宸管理创建，用户编号为莘宸员工编号
        Agent = 2 //代理商创建，用户编号为代理商商户号
    }

    public enum MerchState
    {
        Stop = 0,   //停用
        Normal = 1 //正常
    }

    public enum MerchTag
    {
        Game = 0,    //游乐行业
        Lottery = 1 //博彩行业
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    { 
        //0登录1注销2换班3加记录4改记录5删记录6数据备份7查询8清记录9短信查账10短信清账11网站登录12网站注销13网站查账14网站改密码15网站清账16网站兑币17打印18打开
    }

    public enum DigitStatus
    {
        Unused = 0,//未使用
        Inuse = 1,//使用中
        Cancel = 2//作废
    }

    public enum UserStatus
    {
        None = 0,//未审核
        Pass = 1,//已审核,在职
        Leave = 2,//离职
        Lock = 3,//锁定
        Vacation = 4//休假
    }

    //套餐
    public enum FoodType
    {
        Coin = 0,//售币
        Member = 1,//入会
        Digit = 2,//数字币
        Good = 3,//商品
        Ticket = 4,//门票
        Beverage = 5,//餐饮
        Mixed = 6//混合
    }

    public enum FoodDetailType
    {
        Coin = 0,//代币
        Beverage = 1,//餐饮
        Good = 2,//商品
        Ticket = 3,//门票
        Extended = 4,//扩展赠送跟会员级别绑定
        Digit = 5//扩展赠送跟会员级别绑定
    }

    public enum RechargeManner
    {
        Manual = 0,//手动选择实物币或充值到卡
        Coin = 1,//只允许实物币
        Card = 2//只允许充值到卡
    }

    public enum FoodState
    {
        Invalid = 0,//无效（下架）
        Valid = 1//有效（上架）
    }

    public enum GoodType
    {
        Good = 0,//销售商品
        Gift = 1,//兑换礼品
        BGYP = 2,//办公用品
        ZDSB = 3,//终端设备
        XTHC = 4,//系统耗材
        Coin = 5,//代币
        Digit = 6,//数字币
        Card = 7,//会员卡
        JP = 8,//奖品
        Other = 9//其他
    }

    public enum FeeType
    {
        Count = 0,//计次
        Time = 1,//计时
        Day = 2,//日票
        Month = 3,//月票
        Year = 4//年票
    }

    //混合套餐中使用
    public enum WeightType
    {
        Percent = 0,//比例
        Money = 1//金额
    }

    //优惠时段，时段类型
    public enum PeriodType
    {
        WorkDay = 0,//工作日
        Holiday = 1,//节假日
        Double = 2,//双休制
        Custom = 3//自定义
    }

    //连锁门店权重配置中使用
    public enum ChainStoreWeightType
    {
        Whole = 0,//按全场
        Game = 1//按游戏机
    }

    //连锁门店余额通用设定
    public enum ChainStoreRuleType : int 
    {
        /// <summary>
        /// 代币
        /// </summary>
        [Description("代币")]
        代币 = 0,

        /// <summary>
        /// 积分
        /// </summary>
        [Description("积分")]
        积分 = 1,

        /// <summary>
        /// 彩票
        /// </summary>
        [Description("彩票")]
        彩票 = 2,

        /// <summary>
        /// 储值金
        /// </summary>
        [Description("储值金")]
        储值金 = 3
    }

    public enum DeviceType
    {
        /// <summary>
        /// 卡头
        /// </summary>
        [Description("卡头")]
        卡头 = 0,

        /// <summary>
        /// 碎票机
        /// </summary>
        [Description("碎票机")]
        碎票机 = 1,

        /// <summary>
        /// 存币机
        /// </summary>
        [Description("存币机")]
        存币机 = 2,

        /// <summary>
        /// 提币机
        /// </summary>
        [Description("提币机")]
        提币机 = 3,

        /// <summary>
        /// 售币机
        /// </summary>
        [Description("售币机")]
        售币机 = 4,

        /// <summary>
        /// 投币机
        /// </summary>
        [Description("投币机")]
        投币机 = 5,

        /// <summary>
        /// 存币机
        /// </summary>
        [Description("自助机")]
        自助机 = 6,

        /// <summary>
        /// 闸机
        /// </summary>
        [Description("闸机")]
        闸机 = 7
    }

    public enum DeviceStatus
    {
        Stop = 0,//停用
        Normal = 1,//正常
        Service = 2//检修
    }

    public enum DeviceBindType
    {
        In = 0,//签入
        Out = 1//签出
    }

    public enum StockFlag
    {
        In = 0,//入库
        Out = 1//出库
    }

    //优惠券类别
    public enum CouponType
    {
        Cash = 0,//代金
        Discount = 1,//折扣
        Charge = 2//兑换
    }

    //兑换方式
    public enum ChargeType
    {
        Good = 0,//礼品
        Project = 1,//门票
        Coin = 2//代币
    }

    //派发方式
    public enum SendType
    {
        Consume = 0,//消费
        Orient = 1,//定向
        Jackpot = 2,//抽奖
        Delivery = 3 //街边派送
    }

    //实物券标记
    public enum EntryCouponFlag
    {
        Digit = 0,//电子优惠券
        Entry = 1 //实物优惠券
    }

    /// <summary>
    /// 支付方式
    /// </summary>
    public enum PaymentChannel
    {
        /// <summary>
        /// 微信
        /// </summary>
        [Description("010")]
        WXPAY = 1, 

        /// <summary>
        /// 支付宝
        /// </summary>
        [Description("020")]
        ALIPAY = 2
    }

    /// <summary>
    /// 结算类型
    /// 0 不采用三方结算
    /// 1 微信支付宝官方通道
    /// 2 新大陆
    /// 3 拉卡拉
    /// 4 扫呗
    /// </summary>
    public enum SelttleType
    {
        /// <summary>
        /// 不使用第三方支付
        /// </summary>
        NotThird = 0,
        /// <summary>
        /// 微信支付宝官方通道
        /// </summary>
        AliWxPay = 1,
        /// <summary>
        /// 新大陆
        /// </summary>
        StarPos = 2,
        Lakala = 3,
        /// <summary>
        /// 扫呗
        /// </summary>
        LcswPay = 4,
        /// <summary>
        /// 智付
        /// </summary>
        DinPay = 5
    }

    public enum OrderState
    {
        /// <summary>
        /// 未结算
        /// </summary>
        Unsettled = 0,

        /// <summary>
        /// 未付款
        /// </summary>
        Unpaid = 1,

        /// <summary>
        /// 已付款
        /// </summary>
        AlreadyPaid = 2,

        /// <summary>
        /// 异常支付警报
        /// </summary>
        Alarm = 3
    }
}
