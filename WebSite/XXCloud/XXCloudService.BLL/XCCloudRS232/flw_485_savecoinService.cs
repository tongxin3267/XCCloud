﻿using System;
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
    public partial class flw_485_savecoinService : BaseService<flw_485_savecoin>, Iflw_485_savecoinService
    {
        private Iflw_485_savecoinDAL deviceDAL = DALContainer.Resolve<Iflw_485_savecoinDAL>();
        public override void SetDal()
        {
            Dal = deviceDAL;
        }
    }
}
