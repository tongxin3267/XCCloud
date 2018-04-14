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
    public partial class flw_485_coinDAL : BaseDAL<flw_485_coin>, Iflw_485_coinDAL
    {
        public flw_485_coinDAL(string containerName)
            : base(containerName)
        { 
            
        }
    }
}
