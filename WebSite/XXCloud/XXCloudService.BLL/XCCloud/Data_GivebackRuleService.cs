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
	public class Data_GivebackRuleService : BaseService<Data_GivebackRule>, IData_GivebackRuleService
	{
		public override void SetDal()
        {

        }

        public Data_GivebackRuleService()
            : this(false)
        {

        }

        public Data_GivebackRuleService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_GivebackRuleDAL>(resolveNew: resolveNew);
        }
	} 
}