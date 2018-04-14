using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.CacheService
{
    public class WorkFlowCache<T>
    {
        private static List<T> _workFlowList = new List<T>();

        public static void Add(T t)
        {
            _workFlowList.Add(t);
        }

        public static bool Exist(Predicate<T> match)
        {
            return _workFlowList.Exists(match);
        }

        public static bool Remove(T t)
        {
            return _workFlowList.Remove(t);
        }

        public static int RemoveAll(Predicate<T> match)
        {
            return _workFlowList.RemoveAll(match);
        }

        public static T Find(Predicate<T> match)
        {
            return _workFlowList.Find(match);
        }

        public static void Clear()
        {
            _workFlowList.Clear();
        }
    }
}
