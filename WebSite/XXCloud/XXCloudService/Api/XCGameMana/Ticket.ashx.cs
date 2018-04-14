using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.XCGameManager;


namespace XXCloudService.Api.XCGameMana
{
    /// <summary>
    /// Ticket 的摘要说明
    /// </summary>
    public class Ticket : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object addTicket(Dictionary<string, object> dicParas)
        {
            StoreCacheModel storeCacheModel = null;
            string errMsg = string.Empty;
            string tickets = dicParas.ContainsKey("tickets") ? dicParas["tickets"].ToString() : string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;

            XCCloudService.Business.XCGameMana.StoreBusiness storeBusiness = new XCCloudService.Business.XCGameMana.StoreBusiness();
            XCCloudService.BLL.IBLL.XCGameManager.ITicketService ticketService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGameManager.ITicketService>();
            if (storeBusiness.IsEffectiveStore(storeId, ref storeCacheModel, out errMsg))
            {
                string[] ticketArr = tickets.Split(',');
                for(int i = 0;i< ticketArr.Length ;i++)
                {
                    string curTicketNo = ticketArr[i];
                    if (ticketService.GetModels(p => p.StoreId.Equals(storeId) && p.TicketNo.Equals(curTicketNo)).Count<t_ticket>() == 0)
                    {
                        t_ticket ticket = new t_ticket();
                        ticket.TicketNo = ticketArr[i];
                        ticket.StoreId = storeId;
                        ticket.CreateTime = System.DateTime.Now;
                        ticketService.Add(ticket);                        
                    }
                }
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, "");
            }
            else
            {
                return ResponseModelFactory.CreateFailModel(isSignKeyReturn,"");
            }
        }
    }
}