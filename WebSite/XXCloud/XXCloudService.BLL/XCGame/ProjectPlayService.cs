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
    public partial class ProjectPlayService : BaseService<flw_project_play>, IProjectPlayService
    {
        private IProjectPlayDAL StaffDAL;

        public ProjectPlayService(string containerName)
        {
            this.containerName = containerName;
            StaffDAL = DALContainer.Resolve<IProjectPlayDAL>(this.containerName);
            Dal = StaffDAL;
        }
        public override void SetDal()
        {

        }
    }
    
}
