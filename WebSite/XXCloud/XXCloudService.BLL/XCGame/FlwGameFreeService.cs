using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL;
using XCCloudService.DAL.XCGame.IDAL;
using XCCloudService.Model;
using XCCloudService.Model.XCGame;

namespace XCCloudService.BLL.XCGame
{
    public partial class FlwGameFreeService : BaseService<flw_game_free>, IFlwGameFreeService
    {
        private IFlwGameFreeDAL StaffDAL;

        public FlwGameFreeService(string containerName)
        {
            this.containerName = containerName;
            StaffDAL = DALContainer.Resolve<IFlwGameFreeDAL>(this.containerName);
            Dal = StaffDAL;
        }
        public override void SetDal()
        {

        }
    }
}
