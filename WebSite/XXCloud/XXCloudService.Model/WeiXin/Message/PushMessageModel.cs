using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.WeiXin.Message
{
    [DataContract]
    public class MiniProgram
    {
        public MiniProgram(string appId,string pagePath)
        {
            this.SAppId = appId;
            this.PagePath = pagePath;
        }

        [DataMember(Name = "appid", Order = 1)]
        public string SAppId {set;get;}

        [DataMember(Name = "pagepath", Order = 2)]
        public string PagePath {set;get;}
    }

    [DataContract]
    public class PushMessageMiniModel<T>
    {
        public PushMessageMiniModel(string toUser,string tmpId,string detailsUrl,MiniProgram miniProgram,T data)
        {
            this.Data = data;
        }

        [DataMember(Name = "touser", Order = 1)]
        public string ToUser { set; get; }

        [DataMember(Name = "template_id", Order = 2)]
        public string Template_Id { set; get; }

        [DataMember(Name = "url", Order = 3)]
        public string Url { set; get; }

        [DataMember(Name = "miniprogram", Order = 4)]
        public MiniProgram MiniProgramModel { set; get; }

        [DataMember(Name = "data", Order = 5)]
        public T Data { set; get; } 
    }


    [DataContract]
    public class PushMessageModel<T>
    {
        public PushMessageModel(T data)
        {
            this.Data = data;
        }

        [DataMember(Name = "touser", Order = 1)]
        public string ToUser { set; get; }

        [DataMember(Name = "template_id", Order = 2)]
        public string Template_Id { set; get; }

        [DataMember(Name = "url", Order = 3)]
        public string Url { set; get; }
        
        [DataMember(Name = "data", Order = 4)]
        public T Data { set; get; }
    }
}
