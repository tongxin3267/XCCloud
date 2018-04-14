using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.CacheService;

namespace XCCloudService.Business.Common
{
    public class UnionIdTokenBusiness
    {
        public static string SetUnionIdToken(string unionId)
        {
            string token = System.Guid.NewGuid().ToString("N");
            if (UnionIdTokenCache.ExistTokenByKey(unionId))
            {
                UnionIdTokenCache.UpdateTokenByKey(unionId, token);
            }
            else
            {
                UnionIdTokenCache.AddToken(unionId, token);
            }
            return token;
        }

        public static bool ExistToken(string token, out string unionId)
        {
            unionId = string.Empty;
            if (UnionIdTokenCache.ExistTokenByValue(token))
            {
                unionId = UnionIdTokenCache.GetKeyByValue(token);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
