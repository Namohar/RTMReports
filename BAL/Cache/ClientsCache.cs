using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using DAL;
using System.Data;
using System.Data.SqlClient;

namespace BAL.Cache
{
    public class ClientsCache
    {
        private static ObjectCache cache = MemoryCache.Default;
        private const string ClientsCacheKey = "Clients";
        private static readonly object cacheLock = new object();

        public static DataTable GetClients(int teamId)
        {
            var clients = (DataTable)cache.Get(ClientsCacheKey);

            if (clients != null)
            {
                return clients;
            }

            // Use a lock to ensure that we do not try to create the cache item more than once.
            lock (cacheLock)
            {
                clients = (DataTable)cache.Get(ClientsCacheKey);

                if (clients != null)
                {
                    return clients;
                }

                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration
                };

                SqlDBHelper objDB = new SqlDBHelper();

                SqlParameter[] param = new SqlParameter[] {
                   new SqlParameter("@teamId", teamId)
                };

                string sQuery = "select CL_ID, CL_ClientName from RTM_Client_List WITH (NOLOCK) where CL_TeamId = 4 and CL_Status = 1 order by CL_ClientName";

                clients = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);

                cache.Set(ClientsCacheKey, clients, cacheItemPolicy);

                return clients;
            }
        }

        public static void RemoveClientsCachedItem()
        {
            // Check and remove cache if exists
            if (cache.Contains(ClientsCacheKey))
            {
                cache.Remove(ClientsCacheKey);
            }
        }
    }
}
