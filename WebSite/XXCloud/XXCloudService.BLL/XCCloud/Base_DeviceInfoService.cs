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
	public class Base_DeviceInfoService : BaseService<Base_DeviceInfo>, IBase_DeviceInfoService
	{
        public override void SetDal()
        {

        }

        public Base_DeviceInfoService()
            : this(false)
        {

        }

        public Base_DeviceInfoService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_DeviceInfoDAL>(resolveNew: resolveNew);           
        }
	} 
}