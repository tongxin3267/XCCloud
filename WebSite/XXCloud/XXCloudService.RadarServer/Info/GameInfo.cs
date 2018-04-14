using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXCloudService.RadarServer.Info
{
    public class GameInfo
    {
        public class 游戏机常规属性
        {
            public string 游戏机编号 = "";
            public string 游戏机名称 = "";
            public List<int> 退币超时时间 = new List<int>();
            public int 退币超时计数 = 0;
            public int 机头数 = 0;
            public bool 退币保护标志 = false;
            public DateTime? 上次检测时间 = null;
        }

        public class 游戏机信息
        {
            public 游戏机常规属性 常规 = new 游戏机常规属性();
        }

        static List<游戏机信息> 游戏机列表 = new List<游戏机信息>();

        //检查当前分组游戏机是否触发了退币保护盾功能
        public static bool TBProtect(string gameID)
        {
            foreach (游戏机信息 游戏机 in 游戏机列表)
            {
                if (游戏机.常规.游戏机编号 == gameID)
                {
                    游戏机.常规.退币超时计数++;
                    if (游戏机.常规.退币超时计数 >= 3)
                    {
                        //连续3次报警，触发退币保护功能
                        游戏机.常规.退币超时计数 = 0;  //重新清零计数                        
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        public static bool ClearTimeoutCount(string gameID, bool protectValue)
        {
            foreach (游戏机信息 游戏机 in 游戏机列表)
            {
                if (游戏机.常规.游戏机编号 == gameID)
                {
                    游戏机.常规.退币超时时间 = new List<int>();
                    游戏机.常规.退币超时计数 = 0;
                    游戏机.常规.退币保护标志 = protectValue;
                    break;
                }
            }
            return true;
        }
    }
}
