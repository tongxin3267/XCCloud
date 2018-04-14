using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.DAL.Base;
using XCCloudService.DAL.IDAL;
using XCCloudService.DAL.XCGame.IDAL;
using XCCloudService.Model;
using XCCloudService.Model.XCGame;

namespace XCCloudService.DAL.XCGame
{

    public partial class GameFreeRuleDAL : BaseDAL<t_game_free_rule>, IGameFreeRuleDAL
    {
        public GameFreeRuleDAL(string containerName)
            : base(containerName)
        { 
            
        }
    }
}
