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
    public partial class ClientService : BaseService<client>, IClientService
    {
        private IClientDAL clientDal = DALContainer.Resolve<IClientDAL>();
        public override void SetDal()
        {
            Dal = clientDal;
        }
    }
}
