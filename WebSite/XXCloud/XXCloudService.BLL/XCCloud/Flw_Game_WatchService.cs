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
	public class Flw_Game_WatchService : BaseService<Flw_Game_Watch>, IFlw_Game_WatchService
	{
        public override void SetDal()
        {
        	
        }
        
        public Flw_Game_WatchService()
        	: this(false)
        {
            
        }
        
        public Flw_Game_WatchService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IFlw_Game_WatchDAL>(resolveNew: resolveNew);
        }
	} 
}