<%@ WebHandler Language="C#" Class="XtreeData" %>

using System.Web;
using System.Text;

public class XtreeData : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        System.Threading.Thread.Sleep(2000);

        StringBuilder str = new StringBuilder();

        str.Append("[");
        str.Append("{title:\"节点1\",value:\"jd1\",checked:false,disabled:false,data:[");
        str.Append("{title:\"节点1.1\",value:\"jd1.1\",checked:false,disabled:false,data:[]}");
        str.Append(",{title:\"节点1.2\",value:\"jd1.2\",checked:false,disabled:false,data:[]}");
        str.Append(",{title:\"节点1.3\",value:\"jd1.3\",checked:false,disabled:false,data:[]}");
        str.Append(",{title:\"节点1.4\",value:\"jd1.4\",checked:false,disabled:false,data:[]}");
        str.Append("]}");
        str.Append(",{title:\"节点2\",value:\"jd2\",checked:false,disabled:false,data:[");
        str.Append("{title:\"节点2.1\",value:\"jd2.1\",checked:false,disabled:false,data:[]}");
        str.Append(",{title:\"节点2.2\",value:\"jd2.2\",checked:false,disabled:true,data:[");
        str.Append("{title:\"节点2.2.1\",value:\"jd2.2.1\",checked:true,disabled:false,data:[]}");
        str.Append(",{title:\"节点2.2.2\",value:\"jd2.2.2\",checked:false,disabled:false,data:[]}");
        str.Append(",{title:\"节点2.2.3\",value:\"jd2.2.3\",checked:false,disabled:true,data:[]}");
        str.Append(",{title:\"节点2.2.4\",value:\"jd2.2.4\",checked:true,disabled:true,data:[]}]}");
        str.Append(",{title:\"节点2.3\",value:\"jd2.3\",checked:false,disabled:false,data:[]}");
        str.Append(",{title:\"节点2.4\",value:\"jd2.4\",checked:false,disabled:false,data:[]}");
        str.Append("]}");
        str.Append(",{title:\"节点3\",value:\"jd3\",checked:false,disabled:false,data:[]}");
        str.Append(",{title:\"节点4\",value:\"jd4\",checked:false,disabled:false,data:[");
        str.Append("{title:\"节点4.1\",value:\"jd4.1\",checked:false,disabled:false,data:[");
        str.Append("{title:\"节点4.1.1\",value:\"jd4.1.1\",checked:false,disabled:false,data:[");
        str.Append("{title:\"节点4.1.1.1\",value:\"jd4.1.1.1\",checked:false,disabled:false,data:[]}");
        str.Append(",{title:\"节点4.1.1.2\",value:\"jd4.1.1.2\",checked:false,disabled:false,data:[");
        str.Append("{title:\"节点4.1.1.2.1\",value:\"jd4.1.1.2.1\",checked:false,disabled:false,data:[]}");
        str.Append("]}");
        str.Append("]}");
        str.Append("]}");
        str.Append(",{title:\"节点4.2\",value:\"jd4.2\",checked:false,disabled:true,data:[]}");
        str.Append(",{title:\"节点4.3\",value:\"jd4.3\",checked:false,disabled:true,data:[]}");
        str.Append(",{title:\"节点4.4\",value:\"jd4.4\",checked:false,disabled:false,data:[]}");
        str.Append(",{title:\"节点4.5\",value:\"jd4.5\",checked:false,disabled:false,data:[]}");
        str.Append(",{title:\"节点4.6\",value:\"jd4.6\",checked:false,disabled:false,data:[]}");
        str.Append("]}");
        str.Append("]");

        context.Response.Write(str);
        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
