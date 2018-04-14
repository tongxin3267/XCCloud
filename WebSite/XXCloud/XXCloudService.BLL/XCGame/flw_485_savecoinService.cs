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
    public partial class flw_485_savecoinService : BaseService<flw_485_savecoin>, Iflw_485_savecoinService
    {
        private Iflw_485_savecoinDAL StaffDAL;

        public flw_485_savecoinService(string containerName)
        {
            this.containerName = containerName;
            StaffDAL = DALContainer.Resolve<Iflw_485_savecoinDAL>(this.containerName);
            Dal = StaffDAL;
        }
        public override void SetDal()
        {

        }
    }
    
}
