using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.Business.Common;
using XCCloudService.CacheService;
using XCCloudService.Common;

namespace XXCloudService.Api.XCGameMana
{
    /// <summary>
    /// PhotoUpload 的摘要说明
    /// </summary>
    public class PhotoUpload : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        #region "会员图片上传"
        public object getphotoupload(Dictionary<string, object> dicParas)
        {
            
            string moblietoken = dicParas.ContainsKey("moblietoken") ? dicParas["moblietoken"].ToString() : string.Empty;
            string imageUrl = dicParas.ContainsKey("imageurl") ? dicParas["imageurl"].ToString() : string.Empty;
            string mobile = string.Empty;         
            var file = HttpContext.Current.Request.Files[0];
            LogHelper.SaveLog("file=" + file);           
            if (file == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未找到图片");
            }
            if (imageUrl != "")
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未找到上传路径");
            }
            string path = imageUrl + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss").ToString() + file.FileName;
            file.SaveAs(path);
            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            //return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, path, Result_Code.T, "");

        }
        #endregion
        
    }
}