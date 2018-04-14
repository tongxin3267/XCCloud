using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Model.XCGame;
using XCCloudService.Model.XCGameManager;

namespace XXCloudService.Api.XCGame
{
    /// <summary>
    /// Project 的摘要说明
    /// </summary>
    public class Project : ApiBase
    {
        /// <summary>
        /// 获取门票信息
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getProject(Dictionary<string, object> dicParas)
        {
            try
            {
                string Barcode = dicParas.ContainsKey("barcode") ? dicParas["barcode"].ToString() : string.Empty;
             
                if (string.IsNullOrEmpty(Barcode))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票条码无效");
                }
                ITicketService ticketService = BLLContainer.Resolve<ITicketService>();
                var ticketlist = ticketService.GetModels(x => x.TicketNo == Barcode).FirstOrDefault<t_ticket>();
                if (ticketlist == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到门店号");
                }
                int storeids = int.Parse(ticketlist.StoreId);
                IStoreService storeService = BLLContainer.Resolve<IStoreService>();
                var menlist = storeService.GetModels(x => x.id == storeids).FirstOrDefault<t_store>();
                if (menlist == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.F, "", Result_Code.T, "门店号无效");
                }
                string dbname = menlist.store_dbname;
                IProject_buy_codelistService project_Buy_CodelistService = BLLContainer.Resolve<IProject_buy_codelistService>(dbname);
                var projectlist = project_Buy_CodelistService.GetModels(x => x.Barcode == Barcode).FirstOrDefault<flw_project_buy_codelist>();
                if (projectlist == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.F, "", Result_Code.T, "未查询到门票信息");
                }
                Project_buy_codelistModel project_Buy_CodelistModel = new Project_buy_codelistModel();
                project_Buy_CodelistModel.ID = projectlist.ID;             
                if (projectlist.EndTime != null)
                {
                    project_Buy_CodelistModel.EndTimes = Convert.ToDateTime(projectlist.EndTime).ToString("yyyy-MM-dd");
                }              
                IProjectService projectService = BLLContainer.Resolve<IProjectService>(dbname);
                var projectlist1 = projectService.GetModels(x => x.id == projectlist.ProjectID).FirstOrDefault<t_project>();
                if (projectlist1 != null)
                {
                    project_Buy_CodelistModel.ProjectName = projectlist1.ProjectName;
                }
                if (projectlist.State == 0)
                {
                    project_Buy_CodelistModel.State = "未使用";
                }
                if (projectlist.State == 1)
                {
                    project_Buy_CodelistModel.State = "已使用";
                }
                if (projectlist.State == 2)
                {
                    project_Buy_CodelistModel.State = "被锁定";
                }
                if (projectlist.ProjectType == 0)
                {
                    project_Buy_CodelistModel.ProjectType = "次数";
                }
                if (projectlist.ProjectType == 1)
                {
                    project_Buy_CodelistModel.ProjectType = "有效期";
                }
                project_Buy_CodelistModel.RemainCount = Convert.ToInt32(projectlist.RemainCount);
                return ResponseModelFactory<Project_buy_codelistModel>.CreateModel(isSignKeyReturn, project_Buy_CodelistModel);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}