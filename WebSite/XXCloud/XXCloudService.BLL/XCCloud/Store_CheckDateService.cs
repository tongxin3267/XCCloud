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
	public class Store_CheckDateService : BaseService<Store_CheckDate>, IStore_CheckDateService
	{
		public override void SetDal()
        {

        }

        public Store_CheckDateService()
            : this(false)
        {

        }

        public Store_CheckDateService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IStore_CheckDateDAL>(resolveNew: resolveNew);
        }
	} 
}