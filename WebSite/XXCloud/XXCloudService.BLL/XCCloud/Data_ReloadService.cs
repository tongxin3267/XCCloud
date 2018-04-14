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
	public class Data_ReloadService : BaseService<Data_Reload>, IData_ReloadService
	{
		public override void SetDal()
        {

        }

        public Data_ReloadService()
            : this(false)
        {

        }

        public Data_ReloadService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_ReloadDAL>(resolveNew: resolveNew);
        }
	} 
}