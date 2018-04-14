using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCGameManagerLog;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL.XCGameManagerLog;
using XCCloudService.Model.XCGameManagerLog;


namespace XCCloudService.BLL.XCGameManagerLog
{
    public partial class UDPIndexLogService : BaseService<t_UDPIndex>, IUDPIndexLogService
    {
        private IUDPIndexLogDAL deviceDAL = DALContainer.Resolve<IUDPIndexLogDAL>();
        public override void SetDal()
        {
            Dal = deviceDAL;
        }
    }
}
