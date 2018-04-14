using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    public class Base_User_StoreListResponseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
    }
}
