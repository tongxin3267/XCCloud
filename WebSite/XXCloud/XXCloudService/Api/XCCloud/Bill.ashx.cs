using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.BLL.XCCloud;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;
using System.Transactions;
using System.Data.SqlClient;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.CacheService;
using XCCloudService.Business;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace XXCloudService.Api.XCCloud
{
    [Authorize(Merches = "Normal,Heavy")]
    /// <summary>
    /// Bill 的摘要说明
    /// </summary>
    public class Bill : ApiBase
    {
        //查询方法  
        private List<Data_BillInfo> Search(string title = null, string publishDate = null)
        {
            //为了模拟EF查询，转换为IEnumerable,在EF中此处为数据库上下文的表对象  
            IData_BillInfoService data_BillInfoService = BLLContainer.Resolve<IData_BillInfoService>();
            var result = data_BillInfoService.GetModels();

            /*下列代码不会立即执行查询，而是生成查询计划 
             * 若参数不存在则不添加查询条件，从而可以无限制的添加查询条件 
             */
            if (!string.IsNullOrEmpty(title))
            {
                result = result.Where(p => p.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(publishDate))
            {
                var date = Convert.ToDateTime(publishDate);
                result = result.Where(p => System.Data.Entity.DbFunctions.DiffDays(p.ReleaseTime, date) == 0);
            }            

            //此时执行查询  
            var final = result.ToList();
            return final;
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object UploadPicture(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;

                #region 验证参数
                
                var file = HttpContext.Current.Request.Files[0];                
                if (file == null)
                {
                    errMsg = "未找到图片";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                if (file.ContentLength > int.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxImageSize"].ToString()))
                {
                    errMsg = "超过图片的最大限制为1M";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                #endregion
                
                string picturePath = System.Configuration.ConfigurationManager.AppSettings["UploadImageUrl"].ToString() + "/XCCloud/Bill/";
                string path = System.Web.HttpContext.Current.Server.MapPath(picturePath);
                //如果不存在就创建file文件夹
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + Utils.ConvertDateTimeToLong(DateTime.Now) + Path.GetExtension(file.FileName);
                if (File.Exists(path + fileName))
                {
                    errMsg = "图片名称已存在，请重命名后上传";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                
                file.SaveAs(path + fileName);

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, new { PicturePath = (picturePath + fileName) });
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object DeletePicture(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string fileName = dicParas.ContainsKey("fileName") ? dicParas["fileName"].ToString() : string.Empty;

                if (fileName == null)
                {
                    errMsg = "图片名称不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                string picturePath = System.Configuration.ConfigurationManager.AppSettings["UploadImageUrl"].ToString() + "/XCCloud/";
                string path = System.Web.HttpContext.Current.Server.MapPath(picturePath);                
                
                IData_BillInfoService data_BillInfoService = BLLContainer.Resolve<IData_BillInfoService>();
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@PicturePath", picturePath + fileName);
                data_BillInfoService.ExecuteSqlCommand("update Data_BillInfo set PicturePath='' where PicturePath=@PicturePath", parameters);

                if (File.Exists(path + fileName))
                {
                    File.Delete(path + fileName);
                }
                
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object PublishBill(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? dicParas["id"].ToString() : string.Empty;
                string title = dicParas.ContainsKey("title") ? dicParas["title"].ToString() : string.Empty;
                string publishType = dicParas.ContainsKey("publishType") ? dicParas["publishType"].ToString() : string.Empty;
                string promotionType = dicParas.ContainsKey("promotionType") ? dicParas["promotionType"].ToString() : string.Empty;
                string picturePath = dicParas.ContainsKey("picturePath") ? dicParas["picturePath"].ToString() : string.Empty;
                string pagePath = dicParas.ContainsKey("pagePath") ? dicParas["pagePath"].ToString() : string.Empty;

                #region 验证参数
                if (!string.IsNullOrEmpty(id) && !Utils.isNumber(id))
                {
                    errMsg = "海报ID格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(title))
                {
                    errMsg = "标题不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(publishType))
                {
                    errMsg = "展示方式不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(promotionType))
                {
                    errMsg = "活动类别不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(pagePath))
                {
                    errMsg = "活动内容不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (title.Length > 50)
                {
                    errMsg = "标题不能超过50字";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(publishType))
                {
                    errMsg = "展示方式须为整形";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(promotionType))
                {
                    errMsg = "活动类别须为整形";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }                
                #endregion

                Data_BillInfo data_BillInfoModel = new Data_BillInfo();
                IData_BillInfoService data_BillInfoService = BLLContainer.Resolve<IData_BillInfoService>();
                if (!data_BillInfoService.Any(p => p.ID.ToString().Equals(id, StringComparison.OrdinalIgnoreCase)))
                {
                    data_BillInfoModel.Title = title;
                    data_BillInfoModel.PublishType = Convert.ToInt32(publishType);
                    data_BillInfoModel.PromotionType = Convert.ToInt32(promotionType);
                    data_BillInfoModel.PicturePath = picturePath;
                    data_BillInfoModel.PagePath = pagePath;
                    data_BillInfoModel.State = 1;
                    data_BillInfoModel.Time = DateTime.Now;
                    data_BillInfoModel.ReleaseTime = DateTime.Now;

                    if (!data_BillInfoService.Add(data_BillInfoModel))
                    {
                        errMsg = "发布海报失败";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }
                else
                {
                    data_BillInfoModel = data_BillInfoService.GetModels(p => p.ID.ToString().Equals(id, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Data_BillInfo>();
                    data_BillInfoModel.Title = title;
                    data_BillInfoModel.PublishType = Convert.ToInt32(publishType);
                    data_BillInfoModel.PromotionType = Convert.ToInt32(promotionType);
                    data_BillInfoModel.PicturePath = picturePath;
                    data_BillInfoModel.PagePath = pagePath;
                    data_BillInfoModel.State = 1;
                    data_BillInfoModel.ReleaseTime = DateTime.Now;

                    if (!data_BillInfoService.Update(data_BillInfoModel))
                    {
                        errMsg = "发布海报失败";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }
                                                                
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object DeleteBill(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? dicParas["id"].ToString() : string.Empty;                

                #region 验证参数
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(id))
                {
                    errMsg = "ID须为整形";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                #endregion

                IData_BillInfoService data_BillInfoService = BLLContainer.Resolve<IData_BillInfoService>();
                int iId = Convert.ToInt32(id);
                if (!data_BillInfoService.Any(p => p.ID.Equals(iId)))
                {
                    errMsg = "该海报Id不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var data_BillInfoModel = data_BillInfoService.GetModels(p => p.ID.Equals(iId)).FirstOrDefault<Data_BillInfo>();
                if (!data_BillInfoService.Delete(data_BillInfoModel))
                {
                    errMsg = "删除海报失败";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (File.Exists(data_BillInfoModel.PicturePath))
                {
                    File.Delete(data_BillInfoModel.PicturePath);
                }

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetBills(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string title = dicParas.ContainsKey("title") ? dicParas["title"].ToString() : string.Empty;
                string publishDate = dicParas.ContainsKey("publishDate") ? dicParas["publishDate"].ToString() : string.Empty;

                #region 验证参数
                if (!string.IsNullOrEmpty(publishDate))
                {
                    try
                    {
                        Convert.ToDateTime(publishDate);
                    }
                    catch (Exception ex)
                    {
                        return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, ex.Message);
                    }
                }
                #endregion

                //IData_BillInfoService data_BillInfoService = BLLContainer.Resolve<IData_BillInfoService>();    
                //string sql = "select * from Data_BillInfo where 1=1";
                //if(!string.IsNullOrEmpty(title))
                //{
                //    sql = sql + " and title=@title";
                //}
                //if(!string.IsNullOrEmpty(publishDate))
                //{
                //    sql = sql + " and DATEDIFF(day,releasetime,@publishDate)=0";
                //}
                //SqlParameter[] parameters = new SqlParameter[2];
                //parameters[0] = new SqlParameter("@title", title);
                //parameters[1] = new SqlParameter("@publishDate", string.IsNullOrEmpty(publishDate) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(publishDate));
                //var data_BillInfo = data_BillInfoService.SqlQuery(sql, parameters).ToList<Data_BillInfo>();

                var data_BillInfo = Search(title, publishDate);

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_BillInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetPictures(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;

                IData_BillInfoService data_BillInfoService = BLLContainer.Resolve<IData_BillInfoService>();
                string sql = "select * from Data_BillInfo";
                var data_BillInfo = data_BillInfoService.SqlQuery(sql).ToList().GroupBy(p => p.PicturePath).Select(g => g.Key).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_BillInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        private object ToAnonymousObject(int code = 1, string msg = "", string src = "", string title = null)
        {
            return new
            {
                code = code, //0表示成功，其它失败
                msg = msg,   //提示信息 //一般上传失败后返回
                data = new
                {
                    src = src,    //图片路径
                    title = title //可选
                }
            };
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken, SysIdAndVersionNo = false)]
        public object UploadPictureInRich(Dictionary<string, object> dicParas)
        {            
            try
            {
                string errMsg = string.Empty;
                                
                #region 验证参数
                
                var file = HttpContext.Current.Request.Files[0];
                if (file == null)
                {
                    errMsg = "未找到图片";
                    return ToAnonymousObject(msg: errMsg);
                }
                if (file.ContentLength > int.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxImageSize"].ToString()))
                {
                    errMsg = "超过图片的最大限制为1M";
                    return ToAnonymousObject(msg: errMsg);
                }
                
                #endregion

                string picturePath = System.Configuration.ConfigurationManager.AppSettings["UploadImageUrl"].ToString() + "/XCCloud/Bill/";
                string path = System.Web.HttpContext.Current.Server.MapPath(picturePath);
                //如果不存在就创建file文件夹
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                
                //string fileName = Path.GetFileName(file.FileName);
                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + Utils.ConvertDateTimeToLong(DateTime.Now) + Path.GetExtension(file.FileName);
                if (File.Exists(path + fileName))
                {
                    errMsg = "图片名称已存在，请重命名后上传";
                    return ToAnonymousObject(msg: errMsg);
                }
                
                file.SaveAs(path + fileName);

                return ToAnonymousObject(code: 0, src: (picturePath + fileName));
            }
            catch (Exception e)
            {
                return ToAnonymousObject(msg: e.Message);
            }
        }
    }
}