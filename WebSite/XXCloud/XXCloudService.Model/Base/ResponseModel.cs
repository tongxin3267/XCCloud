using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace XCCloudService.Base
{
    /// <summary>
    /// 接口响应输出实体
    /// </summary>
    [DataContract]
    public class ResponseModel
    {
        public ResponseModel(string return_code, string return_msg)
        {
            this.Return_Code = return_code;
            this.Return_Msg = return_msg;
        }

        public ResponseModel(string return_code,string return_msg,string result_code,string result_msg)
        {
            this.Return_Code = return_code;
            this.Return_Msg = return_msg;
            this.Result_Code = result_code;
            this.Result_Msg = result_msg;
        }

        public ResponseModel()
        {
            this.Return_Code = "0";
            this.Return_Msg = "";
            this.Result_Code = "0";
            this.Result_Msg = "";
            this.SignKey = "";
        }

        /// <summary>
        /// 返回代码（T/F;T-授权验证成功,F-授权验证失败）
        /// </summary>
        [DataMember(Name = "return_code",Order=1)]
        public string Return_Code { set; get; }

        /// <summary>
        /// 返回代码（授权验证失败，返回错误消息）
        /// </summary>
        [DataMember(Name = "return_msg",Order = 2)]
        public string Return_Msg { set; get; }

        /// <summary>
        /// 结果代码（T/F;T-接口业务逻辑执行成功，F-接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name = "result_code",Order = 3)]
        public string Result_Code { set; get; }

        /// <summary>
        /// 结果消息（接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name = "result_msg",Order = 4)]
        public string Result_Msg { set; get; }

        /// <summary>
        /// 返回结果签名（对return_code、return_msg、result_code、result_msg、加密狗号做字符串连接的MD5）
        /// </summary>
        [DataMember(Name = "signkey", Order = 5)]
        public string SignKey { set; get; }
    }


    /// <summary>
    /// 接口响应输出实体
    /// </summary>
    [DataContract]
    public class ReturnResponseModel
    {
        public ReturnResponseModel(string return_code, string return_msg)
        {
            this.Return_Code = return_code;
            this.Return_Msg = return_msg;
            this.SignKey = "";
        }


        public ReturnResponseModel()
        {
            this.Return_Code = "0";
            this.Return_Msg = "";
            this.SignKey = "";
        }

        /// <summary>
        /// 返回代码（T/F;T-授权验证成功,F-授权验证失败）
        /// </summary>
        [DataMember(Name = "return_code", Order = 1)]
        public string Return_Code { set; get; }

        /// <summary>
        /// 返回代码（授权验证失败，返回错误消息）
        /// </summary>
        [DataMember(Name = "return_msg", Order = 2)]
        public string Return_Msg { set; get; }

        /// <summary>
        /// 返回结果签名（对return_code、return_msg、result_code、result_msg、加密狗号做字符串连接的MD5）
        /// </summary>
        [DataMember(Name = "signkey", Order = 5)]
        public string SignKey { set; get; }
    }


    /// <summary>
    /// 接口响应输出实体
    /// </summary>
    [DataContract]
    public class NoSignKeyResponseModel
    {
        public NoSignKeyResponseModel(string return_code, string return_msg)
        {
            this.Return_Code = return_code;
            this.Return_Msg = return_msg;
        }

        public NoSignKeyResponseModel(string return_code, string return_msg, string result_code, string result_msg)
        {
            this.Return_Code = return_code;
            this.Return_Msg = return_msg;
            this.Result_Code = result_code;
            this.Result_Msg = result_msg;
        }

        public NoSignKeyResponseModel()
        {
            this.Return_Code = "0";
            this.Return_Msg = "";
            this.Result_Code = "0";
            this.Result_Msg = "";
        }

        /// <summary>
        /// 返回代码（T/F;T-授权验证成功,F-授权验证失败）
        /// </summary>
        [DataMember(Name = "return_code", Order = 1)]
        public string Return_Code { set; get; }

        /// <summary>
        /// 返回代码（授权验证失败，返回错误消息）
        /// </summary>
        [DataMember(Name = "return_msg", Order = 2)]
        public string Return_Msg { set; get; }

        /// <summary>
        /// 结果代码（T/F;T-接口业务逻辑执行成功，F-接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name = "result_code", Order = 3)]
        public string Result_Code { set; get; }

        /// <summary>
        /// 结果消息（接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name = "result_msg", Order = 4)]
        public string Result_Msg { set; get; }
    }


    /// <summary>
    /// 接口响应输出实体
    /// </summary>
    [DataContract]
    public class NoSignKeyReturnResponseModel
    {
        public NoSignKeyReturnResponseModel(string return_code, string return_msg)
        {
            this.Return_Code = return_code;
            this.Return_Msg = return_msg;
        }


        public NoSignKeyReturnResponseModel()
        {
            this.Return_Code = "0";
            this.Return_Msg = "";

        }

        /// <summary>
        /// 返回代码（T/F;T-授权验证成功,F-授权验证失败）
        /// </summary>
        [DataMember(Name = "return_code", Order = 1)]
        public string Return_Code { set; get; }

        /// <summary>
        /// 返回代码（授权验证失败，返回错误消息）
        /// </summary>
        [DataMember(Name = "return_msg", Order = 2)]
        public string Return_Msg { set; get; }
    }

    /// <summary>
    /// 接口响应输出实体
    /// </summary>
    [DataContract]
    public class ResponseModel<T>
    {
        public ResponseModel()
        {
            this.Return_Code = "1";
            this.Return_Msg = string.Empty;
            this.Result_Code = "1";
            this.Result_Msg = string.Empty;
        }

        public ResponseModel(T dataObj)
        {
            this.Return_Code = "1";
            this.Return_Msg = string.Empty;
            this.Result_Code = "1";
            this.Result_Msg = string.Empty;
            this.Result_Data = dataObj;
        }

        /// <summary>
        /// 返回代码（T/F;T-授权验证成功,F-授权验证失败）
        /// </summary>
        [DataMember(Name="return_code",Order=1)]
        public string Return_Code { set; get; }

        /// <summary>
        /// 返回代码（授权验证失败，返回错误消息）
        /// </summary>
        [DataMember(Name="return_msg",Order=2)]
        public string Return_Msg { set; get; }

        /// <summary>
        /// 结果代码（T/F;T-接口业务逻辑执行成功，F-接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name="result_code",Order=3)]
        public string Result_Code { set; get; }

        /// <summary>
        /// 结果消息（接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name="result_msg",Order=4)]
        public string Result_Msg { set; get; }

        /// <summary>
        /// 结果数据
        /// </summary>
        [DataMember(Name = "result_data", Order = 5)]
        public T Result_Data { set; get; }

        /// <summary>
        /// 返回结果签名（对return_code、return_msg、result_code、result_msg、加密狗号做字符串连接的MD5）
        /// </summary>
        [DataMember(Name = "signkey", Order = 6)]
        public string SignKey { set; get; }
    }


    /// <summary>
    /// 接口响应输出实体
    /// </summary>
    [DataContract]
    public class NoSignKeyResponseModel<T>
    {
        public NoSignKeyResponseModel()
        {
            this.Return_Code = "1";
            this.Return_Msg = string.Empty;
            this.Result_Code = "1";
            this.Result_Msg = string.Empty;
        }

        public NoSignKeyResponseModel(T dataObj)
        {
            this.Return_Code = "1";
            this.Return_Msg = string.Empty;
            this.Result_Code = "1";
            this.Result_Msg = string.Empty;
            this.Result_Data = dataObj;
        }

        /// <summary>
        /// 返回代码（T/F;T-授权验证成功,F-授权验证失败）
        /// </summary>
        [DataMember(Name = "return_code", Order = 1)]
        public string Return_Code { set; get; }

        /// <summary>
        /// 返回代码（授权验证失败，返回错误消息）
        /// </summary>
        [DataMember(Name = "return_msg", Order = 2)]
        public string Return_Msg { set; get; }

        /// <summary>
        /// 结果代码（T/F;T-接口业务逻辑执行成功，F-接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name = "result_code", Order = 3)]
        public string Result_Code { set; get; }

        /// <summary>
        /// 结果消息（接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name = "result_msg", Order = 4)]
        public string Result_Msg { set; get; }

        /// <summary>
        /// 结果数据
        /// </summary>
        [DataMember(Name = "result_data", Order = 5)]
        public T Result_Data { set; get; }
    }

    /// <summary>
    /// 分页信息对象
    /// </summary>
    [DataContract]
    public class Page
    {
        public Page(int pageIndex, int pageSize, int recordCount)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.RecordCount = recordCount;
        }
        /// <summary>
        /// 页索引
        /// </summary>
        [DataMember(Name = "pageindex", Order = 1)]
        public int PageIndex {set;get;}

        /// <summary>
        /// 页尺寸
        /// </summary>
        [DataMember(Name = "pagesize", Order = 2)]
        public int PageSize {set;get;}

        /// <summary>
        /// 记录数
        /// </summary>
        [DataMember(Name = "recordcount", Order = 3)]
        public int RecordCount {set;get;}

        /// <summary>
        /// 页代码
        /// </summary>
        [DataMember(Name = "pagecount", Order = 4)]
        public int PageCount
        {
            set {}
            get
            {
                int m = RecordCount % PageSize;
                return m > 0 ? (RecordCount / PageSize) + 1 : (RecordCount / PageSize);
            }
        }
    }



    //返回代码
    public class Return_Code
    {
        public static string T { get { return "1"; } }
        public static string F { get { return "0"; } }
    }


    //结果代码
    public class Result_Code
    {
        public static string T { get { return "1"; } }
        public static string F { get { return "0"; } }
    }
}