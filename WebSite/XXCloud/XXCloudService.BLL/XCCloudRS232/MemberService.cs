using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCCloudRS232;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL.XCCloudRS232;
using XCCloudService.Model.XCCloudRS232;

namespace XCCloudService.BLL.XCCloudRS232
{
    public partial class MemberService : BaseService<t_member>, IMemberService
    {
        private IMemberDAL memberDAL = DALContainer.Resolve<IMemberDAL>();
        public override void SetDal()
        {
            Dal = memberDAL;
        }
    }
}
