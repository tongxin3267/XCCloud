using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    public class Data_Push_RuleList
    {
        public int ID { get; set; }
        private string getWeekName(Nullable<int> week)
        {
            return week.HasValue ? (week + "").Replace("1", "周一").Replace("2", "周二").Replace("3", "周三").Replace("4", "周四").Replace("5", "周五").Replace("6", "周六").Replace("7", "周日") : string.Empty;
        }

        public string GameID { get; set; }
        public string GameName { get; set; }
        public string MemberLevelName { get; set; }

        private string _weekName;
        public string WeekName
        {
            get
            {
                return getWeekName(Week);
            }
            set
            {
                _weekName = value;
            }
        }
        private Nullable<int> Week { get; set; }
        public Nullable<int> Allow_Out { get; set; }
        public Nullable<int> Allow_In { get; set; }
        public Nullable<int> Coin { get; set; }
        public Nullable<int> Level { get; set; }
        private Nullable<TimeSpan> ST { get; set; }
        private Nullable<TimeSpan> ET { get; set; }

        private string _startTime;
        public string StartTime
        {
            get
            {
                return ST != null ? ST.Value.ToString(@"hh\:mm\:ss") : string.Empty;
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
                return ET != null ? ET.Value.ToString(@"hh\:mm\:ss") : string.Empty;
            }
            set
            {
                _endTime = value;
            }
        }
    }
}
