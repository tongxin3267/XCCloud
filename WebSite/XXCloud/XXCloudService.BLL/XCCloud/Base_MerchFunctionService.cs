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
	public class Base_MerchFunctionService : BaseService<Base_MerchFunction>, IBase_MerchFunctionService
	{
		public override void SetDal()
        {

        }

        public Base_MerchFunctionService()
            : this(false)
        {

        }

        public Base_MerchFunctionService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_MerchFunctionDAL>(resolveNew: resolveNew);
        }
	} 
}