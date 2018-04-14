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
	public class Data_Food_WorkStationService : BaseService<Data_Food_WorkStation>, IData_Food_WorkStationService
	{
        public override void SetDal()
        {
        	
        }
        
        public Data_Food_WorkStationService()
        	: this(false)
        {
            
        }
        
        public Data_Food_WorkStationService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_Food_WorkStationDAL>(resolveNew: resolveNew);
        }
	} 
}