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
	public class Data_CoinInventoryService : BaseService<Data_CoinInventory>, IData_CoinInventoryService
	{
		public override void SetDal()
        {

        }

        public Data_CoinInventoryService()
            : this(false)
        {

        }

        public Data_CoinInventoryService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_CoinInventoryDAL>(resolveNew: resolveNew);
        }
	} 
}