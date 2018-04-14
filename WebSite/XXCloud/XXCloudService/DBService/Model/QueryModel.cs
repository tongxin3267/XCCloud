using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace XCCloudService.DBService.Model
{
    /// <summary>
    /// 模板查询
    /// </summary>
    [DataContract] 
    public class InitModel
    {
        /// <summary>
        /// 顺序号从0开始
        /// </summary>
        [DataMember(Name="id",Order=1)]
        public int ID {set;get;}

        /// <summary>
        /// 标题
        /// </summary>
        [DataMember(Name = "title", Order = 2)]
        public string Title {set;get;}
        
        /// <summary>
        /// 字段名
        /// </summary>
        [DataMember(Name = "field", Order = 3)]
        public string FieldName { set; get; }

        /// <summary>
        /// 数据类型
        /// </summary>
        [DataMember(Name = "type", Order = 4)]
        public string DataType { set; get; }

        /// <summary>
        /// 是否显示列
        /// </summary>
        [DataMember(Name = "iscolume", Order = 5)]
        public int ShowColume { set; get; }
 
        /// <summary>
        /// 是否显示默认查询
        /// </summary>
        [DataMember(Name = "issearch", Order = 6)]
        public int ShowSearch { set; get; }

        /// <summary>
        /// 宽度
        /// </summary>
        [DataMember(Name = "width", Order = 6)]
        public int Width { set; get; }

        /// <summary>
        /// 默认数据
        /// </summary>
        [DataMember(Name = "list", Order = 7)]
        public List<string> List {set;get;}

        /// <summary>
        /// 支付信息字典ID 
        /// </summary>
        public int DictID { set; get; }
    }
}