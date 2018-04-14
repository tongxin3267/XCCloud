using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCCloudService.DBService.Model
{
    public class Dict_SystemModel
    {
        public int ID { set; get; }
        public int PID { set; get; }
        public string DictKey { set; get; }
        public string DictValue { set; get; }
        public string Commnet { set; get; }
        public int Enabled { set; get; }
        public string StoreID { set; get; }
    }
}