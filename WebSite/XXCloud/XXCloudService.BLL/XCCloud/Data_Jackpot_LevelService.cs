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
	public class Data_Jackpot_LevelService : BaseService<Data_Jackpot_Level>, IData_Jackpot_LevelService
	{
        public override void SetDal()
        {
        	
        }
        
        public Data_Jackpot_LevelService()
        	: this(false)
        {
            
        }
        
        public Data_Jackpot_LevelService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_Jackpot_LevelDAL>(resolveNew: resolveNew);
        }
	} 
}