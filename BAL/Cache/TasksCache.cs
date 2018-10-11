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
    public class TasksCache
    {
        private static ObjectCache cache = MemoryCache.Default;
        private const string TasksCacheKey = "Tasks";
        private static readonly object cacheLock = new object();
        
        public static DataTable GetTasks(int teamId)
        {
            var tasks = (DataTable)cache.Get(TasksCacheKey);

            if (tasks != null)
            {
                return tasks;
            }

            // Use a lock to ensure that we do not try to create the cache item more than once.
            lock (cacheLock)
            {
                tasks = (DataTable)cache.Get(TasksCacheKey);

                if (tasks != null)
                {
                    return tasks;
                }

                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration
                };

                SqlDBHelper objDB = new SqlDBHelper();

                SqlParameter[] param = new SqlParameter[] {
                   new SqlParameter("@teamId", teamId)
                };

                string sQuery = "select TL_ID, TL_Task from RTM_Task_List WITH (NOLOCK) where TL_TeamId =@teamId and TL_Status = 1 order By TL_Task";                

                tasks = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);

                cache.Set(TasksCacheKey, tasks, cacheItemPolicy);

                return tasks;
            }
        }

        public static void RemoveTasksCachedItem()
        {
            // Check and remove cache if exists
            if (cache.Contains(TasksCacheKey))
            {
                cache.Remove(TasksCacheKey);
            }
        }
    }
}
