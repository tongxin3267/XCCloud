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
	public class Data_GoodStock_RecordService : BaseService<Data_GoodStock_Record>, IData_GoodStock_RecordService
	{
        public override void SetDal()
        {
        	
        }
        
        public Data_GoodStock_RecordService()
        	: this(false)
        {
            
        }
        
        public Data_GoodStock_RecordService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_GoodStock_RecordDAL>(resolveNew: resolveNew);
        }
	} 
}