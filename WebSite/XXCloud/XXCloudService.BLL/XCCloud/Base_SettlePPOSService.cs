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
	public class Base_SettlePPOSService : BaseService<Base_SettlePPOS>, IBase_SettlePPOSService
	{
		public override void SetDal()
        {

        }

        public Base_SettlePPOSService()
            : this(false)
        {

        }

        public Base_SettlePPOSService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_SettlePPOSDAL>(resolveNew: resolveNew);
        }
	} 
}