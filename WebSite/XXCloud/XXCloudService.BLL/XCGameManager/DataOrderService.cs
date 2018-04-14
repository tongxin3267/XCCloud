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
    public partial class DataOrderService : BaseService<Data_Order>, IDataOrderService
    {
        private IDataOrderDAL deviceDAL = DALContainer.Resolve<IDataOrderDAL>();
        public override void SetDal()
        {
            Dal = deviceDAL;
        }
    }
   
}
