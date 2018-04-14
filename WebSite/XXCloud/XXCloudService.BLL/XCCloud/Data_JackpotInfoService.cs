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
	public class Data_JackpotInfoService : BaseService<Data_JackpotInfo>, IData_JackpotInfoService
	{
        public override void SetDal()
        {
        	
        }
        
        public Data_JackpotInfoService()
        	: this(false)
        {
            
        }
        
        public Data_JackpotInfoService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_JackpotInfoDAL>(resolveNew: resolveNew);
        }
	} 
}