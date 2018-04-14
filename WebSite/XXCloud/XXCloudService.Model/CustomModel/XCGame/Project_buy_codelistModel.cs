using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGame
{
    [DataContract]
    public class Project_buy_codelistModel
    {
        [DataMember(Name = "id", Order = 1)]
        public long ID { set; get; }

        [DataMember(Name = "projectname", Order = 2)]
        public string ProjectName { set; get; }

        [DataMember(Name = "state", Order = 3)]
        public string State { set; get; }

        [DataMember(Name = "projecttype", Order = 4)]
        public string ProjectType { set; get; }

        [DataMember(Name = "remaincount", Order = 5)]
        public int RemainCount { set; get; }

        [DataMember(Name = "endtime", Order = 6)]
        public string EndTimes { set; get; }
    }
}
