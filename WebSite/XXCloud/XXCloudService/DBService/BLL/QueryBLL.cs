using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using XCCloudService.Common;
using XCCloudService.DBService.DAL;
using XCCloudService.DBService.Model;

namespace XCCloudService.DBService.BLL
{
    public class QueryBLL
    {
        public static void GetInit(string pageName, string processName, int userId, ref List<InitModel> listInitModel, ref List<Dict_SystemModel> listDict_SystemModel)
        {
            QueryDAL.GetInit(pageName, processName, userId, ref listInitModel, ref listDict_SystemModel);
            foreach(InitModel init in listInitModel)
            {
                List<Dict_SystemModel> tmpList = listDict_SystemModel.Where<Dict_SystemModel>(p => p.PID == init.DictID).ToList<Dict_SystemModel>();
                init.List = new List<string>();
                foreach(Dict_SystemModel dict in tmpList)
                {
                    init.List.Add(dict.DictKey);
                }
            }
        }

        public static bool GenDynamicSql(object[] conditions, string prefix, ref string sql, ref SqlParameter[] parameters, out string errMsg)
        {
            errMsg = string.Empty;
            if (conditions == null || conditions.Length == 0)
            {
                errMsg = "查询条件为空";
                return false;
            }

            var sb = new StringBuilder();
            var initModel = new InitModel();  
            var listDict_SystemModel = new List<Dict_SystemModel>();
            var alias = !string.IsNullOrEmpty(prefix) ? (prefix.Substring(prefix.Length - 1, 1) == "." ? prefix : (prefix + ".")) : string.Empty;
            foreach (IDictionary<string, object> el in conditions)
            {
                if (el != null)
                {
                    var dicPara = new Dictionary<string, object>(el, StringComparer.OrdinalIgnoreCase);

                    int id = (dicPara.ContainsKey("id") && Utils.isNumber(dicPara["id"])) ? Convert.ToInt32(dicPara["id"]) : 0;
                    string field = (dicPara.ContainsKey("field") && dicPara["field"] != null) ? dicPara["field"].ToString() : string.Empty;
                    if (string.IsNullOrEmpty(field))
                    {
                        errMsg = "查询条件字段不明确";
                        return false;
                    }

                    QueryDAL.GetInitModel(id, field, ref initModel, ref listDict_SystemModel);
                    if (string.IsNullOrEmpty(initModel.DataType))
                    {
                        errMsg = "查询条件类型不明确";
                        return false;
                    }

                    if (initModel.DataType.Equals("string"))
                    {
                        var str = (dicPara.ContainsKey("values") && dicPara["values"] != null) ? dicPara["values"].ToString() : string.Empty;
                        sb.Append(string.Format(" and {1}{0} like '%' + @{0} + '%' ", field, alias));
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = new SqlParameter("@" + field, str);
                    }
                    else if (initModel.DataType.Equals("number") || initModel.DataType.Equals("bit"))
                    {
                        var number = (dicPara.ContainsKey("values") && dicPara["values"] != null) ? dicPara["values"].ToString() : string.Empty;
                        sb.Append(string.Format(" and {1}{0} = @{0} ", field, alias));
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = new SqlParameter("@" + field, number);
                    }
                    else if (initModel.DataType.Equals("numbers"))
                    {
                        var numbers = dicPara.ContainsKey("values") ? (object[])dicPara["values"] : null;
                        if (numbers != null && numbers.Length > 0)
                        {
                            if (numbers.Length >= 1)
                            {
                                var n0 = numbers[0];
                                if (!string.IsNullOrEmpty(n0 + ""))
                                {
                                    sb.Append(string.Format(" and {1}{0} >= @{0}lower ", field, alias));
                                    Array.Resize(ref parameters, parameters.Length + 1);
                                    parameters[parameters.Length - 1] = new SqlParameter("@" + field + "lower", n0);
                                }
                            }

                            if (numbers.Length >= 2)
                            {
                                var n1 = numbers[1];
                                if (!string.IsNullOrEmpty(n1 + ""))
                                {
                                    sb.Append(string.Format(" and {1}{0} <= @{0}upper ", field, alias));
                                    Array.Resize(ref parameters, parameters.Length + 1);
                                    parameters[parameters.Length - 1] = new SqlParameter("@" + field + "upper", n1);
                                }
                            }                            
                        }
                    }
                    else if (initModel.DataType.Equals("literals"))
                    {
                        var literals = (dicPara.ContainsKey("values") && dicPara["values"] != null) ? dicPara["values"].ToString() : string.Empty;
                        var dict_SystemModel = listDict_SystemModel.Where(w => w.DictKey.Equals(literals, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        literals = (dict_SystemModel != null) ? dict_SystemModel.DictValue : string.Empty;
                        sb.Append(string.Format(" and {1}{0} = @{0} ", field, alias));
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = new SqlParameter("@" + field, literals);
                    }
                    else if (initModel.DataType.Equals("date") || initModel.DataType.Equals("datetime"))
                    {
                        var date = dicPara.ContainsKey("values") ? dicPara["values"] : null;
                        try
                        {
                            DateTime dtDate = Convert.ToDateTime(date);
                            if (initModel.DataType.Equals("date"))
                            {
                                //同一天
                                sb.Append(string.Format(" and convert(varchar,{1}{0},102) = convert(varchar,@{0},102) ", field, alias));
                            }
                            else
                            {
                                //同一分钟
                                sb.Append(string.Format(" and convert(varchar,{1}{0},100) = convert(varchar,@{0},100) ", field, alias));
                            }
                            Array.Resize(ref parameters, parameters.Length + 1);
                            parameters[parameters.Length - 1] = new SqlParameter("@" + field, dtDate);
                        }
                        catch (Exception ex)
                        {
                            errMsg = ex.Message;
                            return false;
                        }
                    }
                    else if (initModel.DataType.Equals("dates") || initModel.DataType.Equals("datetimes"))
                    {
                        var dates = dicPara.ContainsKey("values") ? (object[])dicPara["values"] : null;
                        if (dates != null && dates.Length > 0)
                        {
                            try
                            {
                                if (dates.Length >= 1)
                                {
                                    var d0 = dates[0];
                                    if (!string.IsNullOrEmpty(d0 + ""))
                                    {
                                        DateTime dtDate = Convert.ToDateTime(d0);
                                        sb.Append(string.Format(" and {1}{0} >= @{0}start ", field, alias));
                                        Array.Resize(ref parameters, parameters.Length + 1);
                                        parameters[parameters.Length - 1] = new SqlParameter("@" + field + "start", dtDate);
                                    }
                                }

                                if (dates.Length >= 2)
                                {
                                    var d1 = dates[1];
                                    if (!string.IsNullOrEmpty(d1 + ""))
                                    {
                                        DateTime dtDate = Convert.ToDateTime(d1);
                                        sb.Append(string.Format(" and {1}{0} <= @{0}end ", field, alias));
                                        Array.Resize(ref parameters, parameters.Length + 1);
                                        parameters[parameters.Length - 1] = new SqlParameter("@" + field + "end", dtDate);
                                    }
                                }                                
                            }
                            catch (Exception ex)
                            {
                                errMsg = ex.Message;
                                return false;
                            }
                        }
                    }                    
                    else
                    {
                        errMsg = "查询条件类型不支持";
                        return false;
                    }                    
                }
                else
                {
                    errMsg = "提交数据包含空对象";
                    return false;
                }
            }

            sql = sql + sb.ToString();

            return true;
        }
    }
}