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
    public partial class GameFreeRuleService : BaseService<t_game_free_rule>, IGameFreeRuleService
    {
        private IGameFreeRuleDAL StaffDAL;

        public GameFreeRuleService(string containerName)
        {
            this.containerName = containerName;
            StaffDAL = DALContainer.Resolve<IGameFreeRuleDAL>(this.containerName);
            Dal = StaffDAL;
        }
        public override void SetDal()
        {

        }
    }
}
