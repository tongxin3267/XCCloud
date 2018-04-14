﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.DAL.Base;
using XCCloudService.DAL.IDAL.XCGame;
using XCCloudService.Model.XCGame;

namespace XCCloudService.DAL.XCGame
{
    public partial class ScheduleDAL : BaseDAL<flw_schedule>, IScheduleDAL
    {
        public ScheduleDAL(string containerName)
            : base(containerName)
        { 
            
        }
    }
}