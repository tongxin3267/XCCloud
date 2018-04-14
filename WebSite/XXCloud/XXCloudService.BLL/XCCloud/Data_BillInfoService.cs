using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL.XCCloud;
using XCCloudService.Model.XCCloud;

namespace XCCloudService.BLL.XCCloud
{
	public class Data_BillInfoService : BaseService<Data_BillInfo>, IData_BillInfoService
	{
		public override void SetDal()
        {

        }

        public Data_BillInfoService()
            : this(false)
        {

        }

        public Data_BillInfoService(bool resolveNew)
        {

            Dal = DALContainer.Resolve<IData_BillInfoDAL>(resolveNew: resolveNew);

        }
	} 
}