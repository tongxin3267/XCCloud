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
	public class Data_GoodInventoryService : BaseService<Data_GoodInventory>, IData_GoodInventoryService
	{
		public override void SetDal()
        {

        }

        public Data_GoodInventoryService()
            : this(false)
        {

        }

        public Data_GoodInventoryService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_GoodInventoryDAL>(resolveNew: resolveNew);
        }
	} 
}