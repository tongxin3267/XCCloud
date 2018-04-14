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
	public class Data_ProjectInfoService : BaseService<Data_ProjectInfo>, IData_ProjectInfoService
	{
        public override void SetDal()
        {
            
        }

        public Data_ProjectInfoService()
            : this(false)
        {

        }

        public Data_ProjectInfoService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_ProjectInfoDAL>(resolveNew: resolveNew);
        }
	} 
}