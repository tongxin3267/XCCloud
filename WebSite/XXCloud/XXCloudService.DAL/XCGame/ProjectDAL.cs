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
    public partial class ProjectDAL : BaseDAL<t_project>, IProjectDAL
    {
        public ProjectDAL(string containerName)
            : base(containerName)
        {

        }

    }
    
}
