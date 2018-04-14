using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.WeiXin.SAppMessage
{
    public class MemberCoinsDataModel
    {
        public MemberCoinsDataModel()
        { 
            
        }

        public string StoreName { set; get; }

        public string Type { set; get; }

        public int Coins { set; get; }

        public string Date { set; get; }

        public string Mobile { set; get; }

        public int Balance { set; get; }

        public string Remark { set; get; }
    }
}
