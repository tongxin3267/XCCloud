using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.WeiXin.Message
{
    public class XcGameGetCoinSuccessDataModel
    {
        public XcGameGetCoinSuccessDataModel()
        { 
            
        }

        public XcGameGetCoinSuccessDataModel(int coins, string operationDate)
        {
            this.Coins = coins;
            this.OperationDate = operationDate;
        }


        public int Coins { set; get; }

        public string OperationDate { set; get; }
    }
}
