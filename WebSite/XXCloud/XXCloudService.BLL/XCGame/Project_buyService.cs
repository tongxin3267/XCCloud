using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL.XCGame;
using XCCloudService.Model.XCGame;

namespace XCCloudService.BLL.XCGame
{
    public partial class Project_buyService : BaseService<flw_project_buy>, IProject_buyService
    {
        private IProject_buyDAL StaffDAL;

        public Project_buyService(string containerName)
        {
            this.containerName = containerName;
            StaffDAL = DALContainer.Resolve<IProject_buyDAL>(this.containerName);
            Dal = StaffDAL;
        }
        public override void SetDal()
        {

        }
    }
   
}
