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
    public partial class Project_buy_codelistService : BaseService<flw_project_buy_codelist>, IProject_buy_codelistService
    {
        private IProject_buy_codelistDAL StaffDAL;

        public Project_buy_codelistService(string containerName)
        {
            this.containerName = containerName;
            StaffDAL = DALContainer.Resolve<IProject_buy_codelistDAL>(this.containerName);
            Dal = StaffDAL;
        }
        public override void SetDal()
        {

        }
    }
    
}
