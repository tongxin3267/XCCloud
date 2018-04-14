﻿using System;
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
	public class Data_GameInfoService : BaseService<Data_GameInfo>, IData_GameInfoService
	{
		public override void SetDal()
        {

        }

        public Data_GameInfoService()
            : this(false)
        {

        }

        public Data_GameInfoService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_GameInfoDAL>(resolveNew: resolveNew);
        }
	} 
}