using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.XCGame;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.DAL.Base;

namespace XCCloudService.BLL.Container
{
    public class BLLContainer
    {
        public static Dictionary<string, IContainer> containerDict = new Dictionary<string, IContainer>();
        public static Dictionary<string, IContainer> containerDictNew = new Dictionary<string, IContainer>();

        /// <summary>
        /// 获取 IDal 的实例化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>(string xcGameDBName = "", bool resolveNew = false)
        {
            try
            {
                string containerName = GetContainerName(typeof(T), xcGameDBName);
                var containerDict = GetContainerDict(resolveNew);
                if (!string.IsNullOrEmpty(containerName))
                {
                    if (!containerDict.ContainsKey(containerName))
                    {
                        Initialise(containerName, resolveNew);
                    }
                    return containerDict[containerName].Resolve<T>(new NamedParameter("containerName", containerName));
                }
                else
                {
                    return default(T);
                }
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("IOC实例化出错!" + ex.Message);
            }       
        }        

        private static string GetContainerName(Type type, string xcGameDBName)
        {
            if (type.FullName.ToLower().IndexOf("xcgamemanager.") > 0)
            {
                return ContainerConstant.XCGameManaIOCContainer;
            }
            else if (type.FullName.ToLower().IndexOf("xcgamemanagerlog.") > 0)
            {
                return ContainerConstant.XCGameManaLogIOCContainer;
            }
            else if (type.FullName.ToLower().IndexOf("xccloud.") > 0)
            {
                return ContainerConstant.XCCloudGameManaIOCContainer;
            }
            else if (type.FullName.ToLower().IndexOf("XCCloudRS232.".ToLower()) > 0)
            {
                return ContainerConstant.XCCloudRS232IOCContainer;
            }
            else if (type.FullName.ToLower().IndexOf("xcgame.") > 0)
            {
                if (string.IsNullOrEmpty(xcGameDBName))
                {
                    return string.Empty;
                }
                else
                {
                    return xcGameDBName;
                }
            }
            else
            {
                return "";
            }
        }

        private static Dictionary<string, IContainer> GetContainerDict(bool resolveNew = false)
        {
            return resolveNew ? containerDictNew : containerDict;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialise(string containerName, bool resolveNew = false)
        {
            var builder = new ContainerBuilder();            
            var containerNamespace = string.Empty;

            if(containerName.Equals(ContainerConstant.XCGameManaIOCContainer))
            {
                //XCGameMana注册
                containerNamespace = "XCCloudService.BLL.XCGameManager";
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.DeviceService>().As<XCCloudService.BLL.IBLL.XCGameManager.IDeviceService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.StoreService>().As<XCCloudService.BLL.IBLL.XCGameManager.IStoreService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.ApiRequestLogService>().As<XCCloudService.BLL.IBLL.XCGameManager.IApiRequestLogService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.MemberTokenService>().As<XCCloudService.BLL.IBLL.XCGameManager.IMemberTokenService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.MobileTokenService>().As<XCCloudService.BLL.IBLL.XCGameManager.IMobileTokenService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.MPorderService>().As<XCCloudService.BLL.IBLL.XCGameManager.IMPorderService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.DataMessageService>().As<XCCloudService.BLL.IBLL.XCGameManager.IDataMessageService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.UserRegisterService>().As<XCCloudService.BLL.IBLL.XCGameManager.IUserRegisterService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.TFoodsaleService>().As<XCCloudService.BLL.IBLL.XCGameManager.ITFoodsaleService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.PromotionService>().As<XCCloudService.BLL.IBLL.XCGameManager.IPromotionService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.DataOrderService>().As<XCCloudService.BLL.IBLL.XCGameManager.IDataOrderService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.TicketService>().As<XCCloudService.BLL.IBLL.XCGameManager.ITicketService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.UserService>().As<XCCloudService.BLL.IBLL.XCGameManager.IUserService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.UserTokenService>().As<XCCloudService.BLL.IBLL.XCGameManager.IUserTokenService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManager.AdminUserService>().As<XCCloudService.BLL.IBLL.XCGameManager.IAdminUserService>().InstancePerLifetimeScope();
            }
            else if (containerName.Equals(ContainerConstant.XCGameManaLogIOCContainer))
            {
                //XCGameManaLog注册
                containerNamespace = "XCCloudService.BLL.XCGameManagerLog";
                //builder.RegisterType<XCCloudService.BLL.XCGameManagerLog.UDPRadarRegisterLogService>().As<XCCloudService.BLL.IBLL.XCGameManagerLog.IUDPRadarRegisterLogService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManagerLog.UDPRadarHeatLogService>().As<XCCloudService.BLL.IBLL.XCGameManagerLog.IUDPRadarHeatLogService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManagerLog.UDPDeviceStateChangeLogService>().As<XCCloudService.BLL.IBLL.XCGameManagerLog.IUDPDeviceStateChangeLogService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManagerLog.UDPSendDeviceControlLogService>().As<XCCloudService.BLL.IBLL.XCGameManagerLog.IUDPSendDeviceControlLogService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManagerLog.UDPIndexLogService>().As<XCCloudService.BLL.IBLL.XCGameManagerLog.IUDPIndexLogService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManagerLog.UDPRadarNotifyLogService>().As<XCCloudService.BLL.IBLL.XCGameManagerLog.IUDPRadarNotifyLogService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCGameManagerLog.UDPDeviceControlLogService>().As<XCCloudService.BLL.IBLL.XCGameManagerLog.IUDPDeviceControlLogService>().InstancePerLifetimeScope();
            }
            else if (containerName.Equals(ContainerConstant.XCCloudGameManaIOCContainer))
            {
                //XCCloud注册    
                containerNamespace = "XCCloudService.BLL.XCCloud";                
            }
            else if (containerName.Equals(ContainerConstant.XCCloudRS232IOCContainer))
            {
                //XCCloudRS232注册
                containerNamespace = "XCCloudService.BLL.XCCloudRS232";
                //builder.RegisterType<XCCloudService.BLL.XCCloudRS232.MerchService>().As<XCCloudService.BLL.IBLL.XCCloudRS232.IMerchService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCCloudRS232.DeviceService>().As<XCCloudService.BLL.IBLL.XCCloudRS232.IDeviceService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCCloudRS232.DataGameInfoService>().As<XCCloudService.BLL.IBLL.XCCloudRS232.IDataGameInfoService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCCloudRS232.MerchDeviceService>().As<XCCloudService.BLL.IBLL.XCCloudRS232.IMerchDeviceService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCCloudRS232.MerchSegmentService>().As<XCCloudService.BLL.IBLL.XCCloudRS232.IMerchSegmentService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCCloudRS232.EnumService>().As<XCCloudService.BLL.IBLL.XCCloudRS232.IEnumService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCCloudRS232.FoodsService>().As<XCCloudService.BLL.IBLL.XCCloudRS232.IFoodsService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCCloudRS232.MemberService>().As<XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCCloudRS232.Base_DeviceInfoService>().As<XCCloudService.BLL.IBLL.XCCloudRS232.IBase_DeviceInfoService>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.BLL.XCCloudRS232.FoodSaleService>().As<XCCloudService.BLL.IBLL.XCCloudRS232.IFoodSaleService>().InstancePerLifetimeScope();
            }
            else
            {
                //XCGame注册
                containerNamespace = "XCCloudService.BLL.XCGame";
                //builder.RegisterType<MemberService>().As<IMemberService>().InstancePerLifetimeScope();
                //builder.RegisterType<GoodsService>().As<IGoodsService>().InstancePerLifetimeScope();
                //builder.RegisterType<DeviceService>().As<IDeviceService>().InstancePerLifetimeScope();
                //builder.RegisterType<FoodsService>().As<IFoodsService>().InstancePerLifetimeScope();
                //builder.RegisterType<flw_485_coinService>().As<Iflw_485_coinService>().InstancePerLifetimeScope();
                //builder.RegisterType<FoodsaleService>().As<IFoodsaleService>().InstancePerLifetimeScope();
                //builder.RegisterType<ScheduleService>().As<IScheduleService>().InstancePerLifetimeScope();
                //builder.RegisterType<MemberlevelService>().As<IMemberlevelService>().InstancePerLifetimeScope();
                //builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
                //builder.RegisterType<CheckDateService>().As<ICheckDateService>().InstancePerLifetimeScope();
                //builder.RegisterType<ScheduleService>().As<IScheduleService>().InstancePerLifetimeScope();
                //builder.RegisterType<Checkdate_ScheduleService>().As<ICheckdate_ScheduleService>().InstancePerLifetimeScope();
                //builder.RegisterType<Project_buy_codelistService>().As<IProject_buy_codelistService>().InstancePerLifetimeScope();
                //builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerLifetimeScope();
                //builder.RegisterType<Project_buyService>().As<IProject_buyService>().InstancePerLifetimeScope();
                //builder.RegisterType<GameService>().As<IGameService>().InstancePerLifetimeScope();
                //builder.RegisterType<HeadService>().As<IHeadService>().InstancePerLifetimeScope();
                //builder.RegisterType<PushRuleService>().As<IPushRuleService>().InstancePerLifetimeScope();
                //builder.RegisterType<GameFreeRuleService>().As<IGameFreeRuleService>().InstancePerLifetimeScope();
                //builder.RegisterType<FlwGameFreeService>().As<IFlwGameFreeService>().InstancePerLifetimeScope();
                //builder.RegisterType<ProjectPlayService>().As<IProjectPlayService>().InstancePerLifetimeScope();
                //builder.RegisterType<FlwLotteryService>().As<IFlwLotteryService>().InstancePerLifetimeScope();
                //builder.RegisterType<FlwTicketExitService>().As<IFlwTicketExitService>().InstancePerLifetimeScope();
                //builder.RegisterType<ParametersService>().As<IParametersService>().InstancePerLifetimeScope();
            }

            var containerDict = GetContainerDict(resolveNew);
            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            var service = builder.RegisterAssemblyTypes(assembly).Where(t => t.Namespace.Equals(containerNamespace));
            if (!resolveNew)
            {
                service.AsImplementedInterfaces().InstancePerLifetimeScope();                
            }
            else
            {
                service.WithParameter(new TypedParameter(typeof(bool), true)).AsImplementedInterfaces();
            }
            
            IContainer container = builder.Build();
            containerDict[containerName] = container;
        }
       
    }
}
