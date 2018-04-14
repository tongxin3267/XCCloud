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
	public class Data_GameFreeRule_ListService : BaseService<Data_GameFreeRule_List>, IData_GameFreeRule_ListService
	{
		public override void SetDal()
        {

        }

        public Data_GameFreeRule_ListService()
            : this(false)
        {

        }

        public Data_GameFreeRule_ListService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_GameFreeRule_ListDAL>(resolveNew: resolveNew);
        }
	} 
}