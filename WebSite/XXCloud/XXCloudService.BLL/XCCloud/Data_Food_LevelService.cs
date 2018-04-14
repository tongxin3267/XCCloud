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
	public class Data_Food_LevelService : BaseService<Data_Food_Level>, IData_Food_LevelService
	{
		public override void SetDal()
        {

        }

        public Data_Food_LevelService()
            : this(false)
        {

        }

        public Data_Food_LevelService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_Food_LevelDAL>(resolveNew: resolveNew);
        }
	} 
}