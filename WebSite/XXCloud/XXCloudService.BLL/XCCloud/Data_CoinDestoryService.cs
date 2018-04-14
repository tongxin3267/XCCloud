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
	public class Data_CoinDestoryService : BaseService<Data_CoinDestory>, IData_CoinDestoryService
	{
		public override void SetDal()
        {

        }

        public Data_CoinDestoryService()
            : this(false)
        {

        }

        public Data_CoinDestoryService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_CoinDestoryDAL>(resolveNew: resolveNew);
        }
	} 
}