using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.DAL.Base;
using XCCloudService.DAL.IDAL.XCGame;
using XCCloudService.Model.XCGame;

namespace XCCloudService.DAL.XCGame
{
    public partial class ProjectPlayDAL : BaseDAL<flw_project_play>, IProjectPlayDAL
    {
        public ProjectPlayDAL(string containerName)
            : base(containerName)
        {

        }

    }
    
}
