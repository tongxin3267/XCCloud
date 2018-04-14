using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL.XCGameManager;
using XCCloudService.Model.XCGameManager;

namespace XCCloudService.BLL.XCGameManager
{
    public partial class ApiRequestLogService : BaseService<t_apiRequestLog>, IApiRequestLogService
    {
        private IApiRequestLogDAL apirequestlogDAL = DALContainer.Resolve<IApiRequestLogDAL>();
        public override void SetDal()
        {
            Dal = apirequestlogDAL;
        }
    }
  
}
