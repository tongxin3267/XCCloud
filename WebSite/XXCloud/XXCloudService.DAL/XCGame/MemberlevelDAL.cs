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
    public partial class MemberlevelDAL : BaseDAL<t_memberlevel>, IMemberlevelDAL
    {
        public MemberlevelDAL(string containerName)
            : base(containerName)
        { 
            
        }
   
    }
}
