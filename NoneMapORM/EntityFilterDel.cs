using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoneMapORM
{
    /// <summary>
    /// 实体过滤委托
    /// </summary>
    /// <param name="input">需判断实体</param>
    /// <returns>是否有效</returns>
    public delegate bool EntityFilterDel<T>(T input)
        where T:BaseEntity;
}
