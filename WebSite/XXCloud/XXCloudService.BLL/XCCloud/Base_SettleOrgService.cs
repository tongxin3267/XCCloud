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
	public class Base_SettleOrgService : BaseService<Base_SettleOrg>, IBase_SettleOrgService
	{
		public override void SetDal()
        {

        }

        public Base_SettleOrgService()
            : this(false)
        {

        }

        public Base_SettleOrgService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_SettleOrgDAL>(resolveNew: resolveNew);
        }
	} 
}