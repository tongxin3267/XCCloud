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
	public class Data_DigitCoinDestroyService : BaseService<Data_DigitCoinDestroy>, IData_DigitCoinDestroyService
	{
		public override void SetDal()
        {

        }

        public Data_DigitCoinDestroyService()
            : this(false)
        {

        }

        public Data_DigitCoinDestroyService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_DigitCoinDestroyDAL>(resolveNew: resolveNew);
        }
	} 
}