using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.DAL.Base;
using XCCloudService.DAL.IDAL.XCGame;
using XCCloudService.Model.XCGame;

namespace XCCloudService.DAL.XCGame
{
    public partial class flw_485_savecoinDAL : BaseDAL<flw_485_savecoin>, Iflw_485_savecoinDAL
    {
        public flw_485_savecoinDAL(string containerName)
            : base(containerName)
        { 
            
        }
    }
}
