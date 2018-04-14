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
	public class Flw_Game_WinPrizeService : BaseService<Flw_Game_WinPrize>, IFlw_Game_WinPrizeService
	{
        public override void SetDal()
        {
        	
        }
        
        public Flw_Game_WinPrizeService()
        	: this(false)
        {
            
        }
        
        public Flw_Game_WinPrizeService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IFlw_Game_WinPrizeDAL>(resolveNew: resolveNew);
        }
	} 
}