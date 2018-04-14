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
	public class Base_StoreWeight_GameService : BaseService<Base_StoreWeight_Game>, IBase_StoreWeight_GameService
	{
		public override void SetDal()
        {

        }

        public Base_StoreWeight_GameService()
            : this(false)
        {

        }

        public Base_StoreWeight_GameService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_StoreWeight_GameDAL>(resolveNew: resolveNew);
        }
	} 
}