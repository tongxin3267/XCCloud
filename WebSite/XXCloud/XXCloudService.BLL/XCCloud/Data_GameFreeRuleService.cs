using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL.XCCloud;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Model.XCCloud;
namespace XCCloudService.BLL.XCCloud
{
	public class Data_GameFreeRuleService : BaseService<Data_GameFreeRule>, IData_GameFreeRuleService
	{
		public override void SetDal()
        {

        }

        public Data_GameFreeRuleService()
            : this(false)
        {

        }

        public Data_GameFreeRuleService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_GameFreeRuleDAL>(resolveNew: resolveNew);
        }
	} 
}