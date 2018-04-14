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
    public partial class TicketService : BaseService<t_ticket>, ITicketService
    {
        private ITicketDAL ticketDAL = DALContainer.Resolve<ITicketDAL>();
        public override void SetDal()
        {
            Dal = ticketDAL;
        }
    }
}
