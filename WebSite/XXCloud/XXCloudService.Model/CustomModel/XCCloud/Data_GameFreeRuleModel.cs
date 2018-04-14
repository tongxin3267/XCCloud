using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    public class Data_GameFreeRuleList
    {
        public int ID { get; set; }
        public string GameID { get; set; }
        public string GameName { get; set; }
        public string MemberLevelName { get; set; }
        public string RuleTypeStr { get; set; }
        public Nullable<int> NeedCoin { get; set; }
        public Nullable<int> FreeCoin { get; set; }
        public Nullable<int> ExitCoin { get; set; }
        public Nullable<int> State { get; set; }
        private Nullable<DateTime> ST { get; set; }
        private Nullable<DateTime> ET { get; set; }

        private string _startTime;
        public string StartTime
        {
            get
            {
                return ST != null ? ST.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            }
            set
            {
                _startTime = value;
            }
        }

        private string _endTime;
        public string EndTime
        {
            get
            {
                return ET != null ? ET.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            }
            set
            {
                _endTime = value;
            }
        }
    }
}
