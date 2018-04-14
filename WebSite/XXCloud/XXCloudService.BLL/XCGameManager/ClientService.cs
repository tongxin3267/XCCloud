﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL.XCGameManager;
using XCCloudService.Model.XCGame;

namespace XCCloudService.BLL.XCGameManager
{
    public partial class ClientService : BaseService<client>, IClientService
    {
        private IClientDAL deviceDAL = DALContainer.Resolve<IClientDAL>();
        public override void SetDal()
        {
            Dal = deviceDAL;
        }
    }
   
}
