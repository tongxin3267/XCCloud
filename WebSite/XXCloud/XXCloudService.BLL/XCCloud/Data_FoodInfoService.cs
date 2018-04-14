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
	public class Data_FoodInfoService : BaseService<Data_FoodInfo>, IData_FoodInfoService
	{
		public override void SetDal()
        {

        }

        public Data_FoodInfoService()
            : this(false)
        {

        }

        public Data_FoodInfoService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_FoodInfoDAL>(resolveNew: resolveNew);
        }
	} 
}