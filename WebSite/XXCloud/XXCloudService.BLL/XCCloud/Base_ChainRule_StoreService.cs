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
	public class Base_ChainRule_StoreService : BaseService<Base_ChainRule_Store>, IBase_ChainRule_StoreService
	{
		public override void SetDal()
        {

        }

        public Base_ChainRule_StoreService()
            : this(false)
        {

        }

        public Base_ChainRule_StoreService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_ChainRule_StoreDAL>(resolveNew: resolveNew);
        }
	} 
}