using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XCCloudService.Common
{
    public class HolidayHelper
    {
        #region 字段属性
        private static object _syncObj = new object();
        private static HolidayHelper _instance { get; set; }
        private static List<DateModel> cacheDateList { get; set; }
        private static string path { get; set; }
        private static DateModel dateModel { get; set; }
        private HolidayHelper() { }
        /// <summary>
        /// 获得单例对象,使用懒汉式（双重锁定）
        /// </summary>
        /// <returns></returns>
        public static HolidayHelper GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncObj)
                {
                    if (_instance == null)
                    {
                        _instance = new HolidayHelper();
                    }
                }
            }
            return _instance;
        }
        #endregion
 
        #region 私有方法
        
        /// <summary>
        /// 获取配置的Json文件
        /// </summary>
        /// <returns>经过反序列化之后的对象集合</returns>
        private List<DateModel> GetConfigList()
        {
            path = string.Format("{0}/HolidayConfig.json", System.AppDomain.CurrentDomain.BaseDirectory);
            string fileContent = GetFileContent(path);
            if (!string.IsNullOrWhiteSpace(fileContent))
            {
                cacheDateList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateModel>>(fileContent);
            }
            return cacheDateList;
        }

        private enum DayType
        {
            Work = 0,
            DayOff = 1,
            Holiday = 2
        }

        private async void GetDaysAsync(int year)
        {
            try
            {
                var dateModel = new DateModel();
                dateModel.Work = new List<string>();
                dateModel.DayOff = new List<string>();
                dateModel.Holiday = new List<string>();
                dateModel.Year = year;
                cacheDateList = cacheDateList ?? new List<DateModel>();
                cacheDateList.Add(dateModel);

                var taskList = new List<Task>();
                var uri = @"https://api.goseek.cn/Tools/holiday";
                var httpClient = new HttpClient(uri);
                var start = new DateTime(year, 1, 1);
                for (var dt = start; dt < start.AddYears(1); dt = dt.AddDays(1))
                {
                    taskList.Add(GetDayTypeByDateAsync(httpClient, uri, dt));
                    Thread.Sleep(100);
                }
                                
                await Task.WhenAll(taskList);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(cacheDateList);
                System.IO.File.WriteAllText(path, json);
            }
            catch (Exception)
            {
                System.GC.Collect();
                throw;
            }
        }

        private async Task GetDayTypeByDateAsync(HttpClient httpClient, string uri, DateTime dt)
        {
            var content = new Dictionary<string, string>()
                       {
                           {"date", dt.ToString("yyyyMMdd")}
                       };

            var sRemoteInfo = await httpClient.GetResponse(uri, "post", content, "utf-8");            

            DayType dayType = DayType.Work;
            string result = JObject.Parse(sRemoteInfo)["data"].ToString();
            dayType = (DayType)Convert.ToInt32(result);
            dateModel = dateModel ?? GetConfigDataByYear(dt.Year);
            if (dateModel != null)
            {
                switch (dayType)
                {
                    case DayType.Work:
                        {
                            dateModel.Work.Add(dt.ToString("MMdd"));
                            break;
                        }
                    case DayType.DayOff:
                        {
                            dateModel.DayOff.Add(dt.ToString("MMdd"));
                            break;
                        }
                    case DayType.Holiday:
                        {
                            dateModel.Holiday.Add(dt.ToString("MMdd"));
                            break;
                        }
                }
            }                       
        }

        /// <summary>
        /// 获取指定年份的数据
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private DateModel GetConfigDataByYear(int year)
        {
            //从本地文件取配置数据
            if (cacheDateList == null)
            {
                GetConfigList();
            }

            dateModel = cacheDateList != null ? cacheDateList.FirstOrDefault(m => m.Year == year) : null;

            //从远程取配置数据
            if (dateModel == null)
            {
                GetDaysAsync(year);
            }

            return dateModel;
        }

        /// <summary>
        /// 判断是否为工作日
        /// </summary>
        /// <param name="currDate">要判断的时间</param>
        /// <param name="thisYearData">当前的数据</param>
        /// <returns></returns>
        private bool IsWorkDay(DateTime currDate, DateModel thisYearData)
        {
            if (currDate.Year != thisYearData.Year)//跨年重新读取数据
            {
                thisYearData = GetConfigDataByYear(currDate.Year);
            }
            if (thisYearData != null)
            {
                string date = currDate.ToString("MMdd");
                //int week = (int)currDate.DayOfWeek;
 
                if (thisYearData.Work.IndexOf(date) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否为休息日
        /// </summary>
        /// <param name="currDate">要判断的时间</param>
        /// <param name="thisYearData">当前的数据</param>
        /// <returns></returns>
        private bool IsDayOff(DateTime currDate, DateModel thisYearData)
        {
            if (currDate.Year != thisYearData.Year)//跨年重新读取数据
            {
                thisYearData = GetConfigDataByYear(currDate.Year);
            }
            if (thisYearData != null)
            {
                string date = currDate.ToString("MMdd");

                if (thisYearData.DayOff.IndexOf(date) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否为节假日
        /// </summary>
        /// <param name="currDate">要判断的时间</param>
        /// <param name="thisYearData">当前的数据</param>
        /// <returns></returns>
        private bool IsHoliday(DateTime currDate, DateModel thisYearData)
        {
            if (currDate.Year != thisYearData.Year)//跨年重新读取数据
            {
                thisYearData = GetConfigDataByYear(currDate.Year);
            }
            if (thisYearData != null)
            {
                string date = currDate.ToString("MMdd");

                if (thisYearData.Holiday.IndexOf(date) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetFileContent(string filePath)
        {
            string result = "";
            if (File.Exists(filePath))
            {
                result = File.ReadAllText(filePath);
            }
            return result;
        }

        #endregion
 
        #region 公共方法
        public void CleraCacheData()
        {
            if (cacheDateList != null)
            {
                cacheDateList.Clear();
            }
        }
        /// <summary>
        /// 根据传入的工作日天数，获得计算后的日期,可传负数
        /// </summary>
        /// <param name="day">天数</param>
        /// <param name="isContainToday">当天是否算工作日（默认：true）</param>
        /// <returns></returns>
        public DateTime GetReckonDate(int day, bool isContainToday = true)
        {
            DateTime currDate = DateTime.Now;
            int addDay = day >= 0 ? 1 : -1;
 
            if (isContainToday)
                currDate = currDate.AddDays(-addDay);
 
            DateModel thisYearData = GetConfigDataByYear(currDate.Year);
            if (thisYearData != null)
            {
                int sumDay = Math.Abs(day);
                int workDayNum = 0;
                while (workDayNum < sumDay)
                {
                    currDate = currDate.AddDays(addDay);
                    if (IsWorkDay(currDate, thisYearData))
                        workDayNum++;
                }
            }
            return currDate;
        }

        /// <summary>
        /// 根据传入的时间，计算工作日天数
        /// </summary>
        /// <param name="date">带计算的时间</param>
        /// <param name="isContainToday">当天是否算工作日（默认：true）</param>
        /// <returns></returns>
        public int GetWorkDayNum(DateTime date, bool isContainToday = true)
        {
            var currDate = DateTime.Now;
 
            int workDayNum = 0;
            int addDay = date.Date > currDate.Date ? 1 : -1;
 
            if (isContainToday)
            {
                currDate = currDate.AddDays(-addDay);
            }
 
            DateModel thisYearData = GetConfigDataByYear(currDate.Year);
            if (thisYearData != null)
            {
                bool isEnd = false;
                do
                {
                    currDate = currDate.AddDays(addDay);
                    if (IsWorkDay(currDate, thisYearData))
                        workDayNum += addDay;
                    isEnd = addDay > 0 ? (date.Date > currDate.Date) : (date.Date < currDate.Date);
                } while (isEnd);
            }
            return workDayNum;
        }

        public bool IsWorkDay(DateTime date)
        {
            DateModel thisYearData = GetConfigDataByYear(date.Year);
            if (thisYearData != null)
            {
                return IsWorkDay(date, thisYearData);
            }

            return true;
        }

        public bool IsDayOff(DateTime date)
        {
            DateModel thisYearData = GetConfigDataByYear(date.Year);
            if (thisYearData != null)
            {
                return IsDayOff(date, thisYearData);
            }

            return false;
        }

        public bool IsHoliday(DateTime date)
        {
            DateModel thisYearData = GetConfigDataByYear(date.Year);
            if (thisYearData != null)
            {
                return IsHoliday(date, thisYearData);
            }

            return false;
        }
        #endregion
    }
 
    public class DateModel
    {
        public int Year { get; set; }
 
        public List<string> Work { get; set; }

        public List<string> DayOff { get; set; }
 
        public List<string> Holiday { get; set; }
    }
    
}
