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
	public class Base_GoodsInfoService : BaseService<Base_GoodsInfo>, IBase_GoodsInfoService
	{
		public override void SetDal()
        {

        }

        public Base_GoodsInfoService()
            : this(false)
        {

        }

        public Base_GoodsInfoService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_GoodsInfoDAL>(resolveNew: resolveNew);
        }
	} 
}