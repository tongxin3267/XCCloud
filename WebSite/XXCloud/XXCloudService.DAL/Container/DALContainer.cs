using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XCCloudService.DAL.XCGame;
using XCCloudService.DAL.IDAL;
using XCCloudService.DAL.XCGame.IDAL;
using XCCloudService.DAL.IDAL.XCGame;
using XCCloudService.DAL.Base;

namespace XCCloudService.DAL.Container
{
    public class DALContainer
    {
        public static Dictionary<string, IContainer> containerDict = new Dictionary<string, IContainer>();
        public static Dictionary<string, IContainer> containerDictNew = new Dictionary<string, IContainer>();

        ///// <summary>
        ///// IOC 容器
        ///// </summary>
        //public static IContainer container = null;


        //public static IContainer Container
        //{
        //    set { container = value; }
        //    get { return container; }
        //}
 
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

            if (containerName.Equals(ContainerConstant.XCGameManaIOCContainer))
            {
                //XCGameMana注册
                containerNamespace = "XCCloudService.DAL.XCGameManager";
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.DeviceDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IDeviceDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.StoreDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IStoreDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.ApiRequestLogDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IApiRequestLogDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.MemberTokenDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IMemberTokenDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.MobileTokenDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IMobileTokenDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.MPOrderDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IMPOrderDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.DataMessageDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IDataMessageDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.UserRegisterDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IUserRegisterDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.TFoodsaleDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.ITFoodsaleDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.PromotionDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IPromotionDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.DataOrderDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IDataOrderDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.TicketDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.ITicketDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.UserDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IUserDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.UserTokenDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IUserTokenDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManager.AdminUserDAL>().As<XCCloudService.DAL.IDAL.XCGameManager.IAdminUserDAL>().InstancePerLifetimeScope();
            }
            else if (containerName.Equals(ContainerConstant.XCGameManaLogIOCContainer))
            {
                //XCGameManaLog注册
                containerNamespace = "XCCloudService.DAL.XCGameManagerLog";
                //builder.RegisterType<XCCloudService.DAL.XCGameManagerLog.UDPRadarRegisterLogDAL>().As<XCCloudService.DAL.IDAL.XCGameManagerLog.IUDPRadarRegisterLogDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManagerLog.UDPRadarHeatLogDAL>().As<XCCloudService.DAL.IDAL.XCGameManagerLog.IUDPRadarHeatLogDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManagerLog.UDPDeviceStateChangeLogDAL>().As<XCCloudService.DAL.IDAL.XCGameManagerLog.IUDPDeviceStateChangeLogDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManagerLog.UDPSendDeviceControlLogDAL>().As<XCCloudService.DAL.IDAL.XCGameManagerLog.IUDPSendDeviceControlLogDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManagerLog.UDPIndexLogDAL>().As<XCCloudService.DAL.IDAL.XCGameManagerLog.IUDPIndexLogDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManagerLog.UDPRadarNotifyLogDAL>().As<XCCloudService.DAL.IDAL.XCGameManagerLog.IUDPRadarNotifyLogDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCGameManagerLog.UDPDeviceControlLogDAL>().As<XCCloudService.DAL.IDAL.XCGameManagerLog.IUDPDeviceControlLogDAL>().InstancePerLifetimeScope();
            }
            else if (containerName.Equals(ContainerConstant.XCCloudGameManaIOCContainer))
            {
                //XCCloud注册
                containerNamespace = "XCCloudService.DAL.XCCloud";                 
            }
            else if (containerName.Equals(ContainerConstant.XCCloudRS232IOCContainer))
            {
                //XCCloudRS232注册
                containerNamespace = "XCCloudService.DAL.XCCloudRS232";
                //builder.RegisterType<XCCloudService.DAL.XCCloudRS232.MerchDAL>().As<XCCloudService.DAL.IDAL.XCCloudRS232.IMerchDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCCloudRS232.DataGameInfoDAL>().As<XCCloudService.DAL.IDAL.XCCloudRS232.IDataGameInfoDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCCloudRS232.DeviceDAL>().As<XCCloudService.DAL.IDAL.XCCloudRS232.IDeviceDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCCloudRS232.MerchDeviceDAL>().As<XCCloudService.DAL.IDAL.XCCloudRS232.IMerchDeviceDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCCloudRS232.MerchSegmentDAL>().As<XCCloudService.DAL.IDAL.XCCloudRS232.IMerchSegmentDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCCloudRS232.EnumDAL>().As<XCCloudService.DAL.IDAL.XCCloudRS232.IEnumDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCCloudRS232.FoodsDAL>().As<XCCloudService.DAL.IDAL.XCCloudRS232.IFoodsDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCCloudRS232.MemberDAL>().As<XCCloudService.DAL.IDAL.XCCloudRS232.IMemberDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCCloudRS232.Base_DeviceInfoDAL>().As<XCCloudService.DAL.IDAL.XCCloudRS232.IBase_DeviceInfoDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<XCCloudService.DAL.XCCloudRS232.FoodSaleDAL>().As<XCCloudService.DAL.IDAL.XCCloudRS232.IFoodSaleDAL>().InstancePerLifetimeScope();
            }
            else
            {
                //XCGame注册
                containerNamespace = "XCCloudService.DAL.XCGame";
                //builder.RegisterType<MemberDAL>().As<IMemberDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<GoodsDAL>().As<IGoodsDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<DeviceDAL>().As<IDeviceDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<FoodsDAL>().As<IFoodsDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<flw_485_coinDAL>().As<Iflw_485_coinDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<FoodsaleDAL>().As<IFoodsaleDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<ScheduleDAL>().As<IScheduleDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<MemberlevelDAL>().As<IMemberlevelDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<UserDAL>().As<IUserDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<CheckDateDAL>().As<ICheckDateDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<WorkStationDAL>().As<IWorkStationDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<Checkdate_ScheduleDAL>().As<ICheckdate_ScheduleDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<Project_buy_codelistDAL>().As<IProject_buy_codelistDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<ProjectDAL>().As<IProjectDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<Project_buyDAL>().As<IProject_buyDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<HeadDAL>().As<IHeadDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<GameDAL>().As<IGameDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<PushRuleDAL>().As<IPushRuleDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<GameFreeRuleDAL>().As<IGameFreeRuleDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<FlwGameFreeDAL>().As<IFlwGameFreeDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<ProjectPlayDAL>().As<IProjectPlayDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<FlwLotteryDAL>().As<IFlwLotteryDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<FlwTicketExitDAL>().As<IFlwTicketExitDAL>().InstancePerLifetimeScope();
                //builder.RegisterType<ParametersDAL>().As<IParametersDAL>().InstancePerLifetimeScope();
            }

            var containerDict = GetContainerDict(resolveNew);
            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            var dal = builder.RegisterAssemblyTypes(assembly).Where(t => t.Namespace.Equals(containerNamespace)).AsImplementedInterfaces();
            if (!resolveNew)
            {
                dal.InstancePerLifetimeScope();
            }

            IContainer container = builder.Build();
            containerDict[containerName] = container;
        }        
    }
}
