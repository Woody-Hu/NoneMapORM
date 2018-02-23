using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoneMapORM
{
    /// <summary>
    /// 使用的缓存工厂
    /// </summary>
    internal class UseCacheFactory : NHibernate.Cache.IQueryCacheFactory
    {
        public UseCacheFactory()
        {
            ;
        }

        public NHibernate.Cache.IQueryCache GetQueryCache
            (string regionName, NHibernate.Cache.UpdateTimestampsCache updateTimestampsCache, Settings settings, IDictionary<string, string> props)
        {
            return new UseStandardQueryCache(settings, props, updateTimestampsCache, regionName);
        }


    }
}
