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
  
    public partial class Project_buy_codelistDAL : BaseDAL<flw_project_buy_codelist>, IProject_buy_codelistDAL
    {
        public Project_buy_codelistDAL(string containerName)
            : base(containerName)
        {

        }
    }
}
