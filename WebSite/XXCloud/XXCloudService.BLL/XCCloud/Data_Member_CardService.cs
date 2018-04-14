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
	public class Data_Member_CardService : BaseService<Data_Member_Card>, IData_Member_CardService
	{
		public override void SetDal()
        {

        }

        public Data_Member_CardService()
            : this(false)
        {

        }

        public Data_Member_CardService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_Member_CardDAL>(resolveNew: resolveNew);
        }
	} 
}