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
	public class Flw_Game_FreeService : BaseService<Flw_Game_Free>, IFlw_Game_FreeService
	{
		public override void SetDal()
        {

        }

        public Flw_Game_FreeService()
            : this(false)
        {

        }

        public Flw_Game_FreeService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IFlw_Game_FreeDAL>(resolveNew: resolveNew);
        }
	} 
}