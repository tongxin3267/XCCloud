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
	public class Data_WorkstationService : BaseService<Data_Workstation>, IData_WorkstationService
	{
		public override void SetDal()
        {

        }

        public Data_WorkstationService()
            : this(false)
        {

        }

        public Data_WorkstationService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_WorkstationDAL>(resolveNew: resolveNew);
        }
	} 
}