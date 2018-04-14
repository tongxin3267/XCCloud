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
	public class Data_GoodsStockService : BaseService<Data_GoodsStock>, IData_GoodsStockService
	{
        public override void SetDal()
        {
        	
        }
        
        public Data_GoodsStockService()
        	: this(false)
        {
            
        }
        
        public Data_GoodsStockService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_GoodsStockDAL>(resolveNew: resolveNew);
        }
	} 
}