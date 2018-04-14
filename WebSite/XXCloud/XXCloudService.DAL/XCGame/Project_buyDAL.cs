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
    public partial class Project_buyDAL : BaseDAL<flw_project_buy>, IProject_buyDAL
    {
        public Project_buyDAL(string containerName)
            : base(containerName)
        {

        }

    }
   
}
