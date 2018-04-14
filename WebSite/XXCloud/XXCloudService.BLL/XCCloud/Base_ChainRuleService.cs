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
	public class Base_ChainRuleService : BaseService<Base_ChainRule>, IBase_ChainRuleService
	{
		public override void SetDal()
        {

        }

        public Base_ChainRuleService()
            : this(false)
        {

        }

        public Base_ChainRuleService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_ChainRuleDAL>(resolveNew: resolveNew);
        }
	} 
}