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
    public partial class FoodsService : BaseService<t_foods>, IFoodsService
    {
        private IFoodsDAL StaffDAL;

        public FoodsService(string containerName)
        {
            this.containerName = containerName;
            StaffDAL = DALContainer.Resolve<IFoodsDAL>(this.containerName);
            Dal = StaffDAL;
        }
        public override void SetDal()
        {

        }
    }
    
}
