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
	public class Data_HeadService : BaseService<Data_Head>, IData_HeadService
	{
		public override void SetDal()
        {

        }

        public Data_HeadService()
            : this(false)
        {

        }

        public Data_HeadService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_HeadDAL>(resolveNew: resolveNew);
        }
	} 
}