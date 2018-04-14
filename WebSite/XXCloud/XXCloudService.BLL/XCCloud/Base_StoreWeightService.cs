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
	public class Base_StoreWeightService : BaseService<Base_StoreWeight>, IBase_StoreWeightService
	{
		public override void SetDal()
        {

        }

        public Base_StoreWeightService()
            : this(false)
        {

        }

        public Base_StoreWeightService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_StoreWeightDAL>(resolveNew: resolveNew);
        }
	} 
}