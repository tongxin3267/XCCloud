using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.WorkFlow;
using XCCloudService.CacheService;
using XCCloudService.Business.Common;
using XCCloudService.Common;
using System.Web.Services;

namespace XXCloudService.Test.WorkFlowTest
{
    public partial class WorkFlowTest : System.Web.UI.Page
    {        
        static Thread thread = new Thread(new ThreadStart(StartWorkFlow));
        static StockWorkFlow stockWorkFlow;

        protected void Page_Load(object sender, EventArgs e)
        {
            ViewState["warningservertoken"] = MobileTokenBusiness.SetMobileToken("warningserver");
            WorkFlowCache<StockWorkFlow>.RemoveAll(p => p.Token.Equals(Convert.ToString(Session["workflowtoken"])));
            stockWorkFlow = new StockWorkFlow();
            WorkFlowCache<StockWorkFlow>.Add(stockWorkFlow);
            Session["workflowtoken"] = stockWorkFlow.Token;
        }

        [WebMethod]
        public static string Start()
        {
            stockWorkFlow.State = StockState.Normal;
            if (!thread.IsAlive)
            {
                thread.Start();
            }
            return "Start stock warning!";
        }

        static void StartWorkFlow()
        {                      
            while (true)
            {
                if (stockWorkFlow.CanFire(StockTrigger.Warn))
                {
                    stockWorkFlow.Warn("warningserver", "lijunjie");
                }
                    
                Thread.Sleep(2000);
            }            
        }
    }
}