using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL.XCCloud;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Model.XCCloud;
namespace XCCloudService.BLL.XCCloud
{
	public class Base_UserGroup_GrantService : BaseService<Base_UserGroup_Grant>, IBase_UserGroup_GrantService
	{
		public override void SetDal()
        {

        }

        public Base_UserGroup_GrantService()
            : this(false)
        {

        }

        public Base_UserGroup_GrantService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_UserGroup_GrantDAL>(resolveNew: resolveNew);
        }
	} 
}