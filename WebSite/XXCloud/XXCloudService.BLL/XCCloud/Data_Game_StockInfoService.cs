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
	public class Data_Game_StockInfoService : BaseService<Data_Game_StockInfo>, IData_Game_StockInfoService
	{
        public override void SetDal()
        {
        	
        }
        
        public Data_Game_StockInfoService()
        	: this(false)
        {
            
        }
        
        public Data_Game_StockInfoService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_Game_StockInfoDAL>(resolveNew: resolveNew);
        }
	} 
}