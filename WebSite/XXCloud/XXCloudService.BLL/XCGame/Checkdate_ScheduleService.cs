using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL;
using XCCloudService.DAL.XCGame.IDAL;
using XCCloudService.Model;
using XCCloudService.Model.XCGame;

namespace XCCloudService.BLL.XCGame
{
    public partial class Checkdate_ScheduleService : BaseService<flw_checkdate_schedule>, ICheckdate_ScheduleService
    {
        private ICheckdate_ScheduleDAL StaffDAL;

        public Checkdate_ScheduleService(string containerName)
        {
            this.containerName = containerName;
            StaffDAL = DALContainer.Resolve<ICheckdate_ScheduleDAL>(this.containerName);
            Dal = StaffDAL;
        }
        public override void SetDal()
        {

        }
    }
}
