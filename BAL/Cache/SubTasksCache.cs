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
    public class SubTasksCache
    {
        private static ObjectCache cache = MemoryCache.Default;
        private const string SubTasksCacheKey = "SubTasks";
        private static readonly object cacheLock = new object();

        public static DataTable GetSubTasks(int teamId)
        {
            var subTasks = (DataTable)cache.Get(SubTasksCacheKey);

            if (subTasks != null)
            {
                return subTasks;
            }

            // Use a lock to ensure that we do not try to create the cache item more than once.
            lock (cacheLock)
            {
                subTasks = (DataTable)cache.Get(SubTasksCacheKey);

                if (subTasks != null)
                {
                    return subTasks;
                }

                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration
                };

                SqlDBHelper objDB = new SqlDBHelper();

                SqlParameter[] param = new SqlParameter[] {
                   new SqlParameter("@teamId", teamId)
                };

                string sQuery = "select STL_Task_Id, STL_ID, STL_SubTask from RTM_SubTask_List WITH (NOLOCK) left join RTM_Task_List WITH (NOLOCK) ON STL_Task_Id = TL_ID WHERE TL_TeamId = @teamId and STL_ViewStatus = 1 order by STL_SubTask";

                subTasks = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);

                cache.Set(SubTasksCacheKey, subTasks, cacheItemPolicy);

                return subTasks;
            }
        }

        public static void RemoveSubTasksCachedItem()
        {
            // Check and remove cache if exists
            if (cache.Contains(SubTasksCacheKey))
            {
                cache.Remove(SubTasksCacheKey);
            }
        }
    }
}
