using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoneMapORM
{
    /// <summary>
    /// 使用的标准查询缓存
    /// </summary>
    internal class UseStandardQueryCache : NHibernate.Cache.StandardQueryCache
    {
        public UseStandardQueryCache(Settings settings, IDictionary<string, string> props, NHibernate.Cache.UpdateTimestampsCache updateTimestampsCache, string regionName)
            : base(settings, props, updateTimestampsCache, regionName)
        {

            ;
        }

        protected override bool IsUpToDate(ISet<string> spaces, long timestamp)
        {
            var retrunValue = base.IsUpToDate(spaces, timestamp);
            return retrunValue;
        }

    }
}
