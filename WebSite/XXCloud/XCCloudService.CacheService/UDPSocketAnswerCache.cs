using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.CacheService
{
    public class UDPSocketAnswerCache
    {
        private static Hashtable _UDPSocketAnswer = new Hashtable();

        public static Hashtable UDPSocketAnswer
        {
            set { _UDPSocketAnswer = value; }
            get { return _UDPSocketAnswer; }
        }

        public static void AddAnswer(string key, object obj)
        {
            _UDPSocketAnswer[key] = obj;
        }


        public static bool Exist(string key)
        {
            return _UDPSocketAnswer.ContainsKey(key);
        }



        public static object GetAnswer(string key)
        {
            return _UDPSocketAnswer[key];
        }

        public static void Remove(string key)
        {
            _UDPSocketAnswer.Remove(key);
        }
    }
}
