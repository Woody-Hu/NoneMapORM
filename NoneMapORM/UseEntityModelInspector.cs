using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace NoneMapORM
{
    /// <summary>
    /// 使用的扩展监听器
    /// </summary>
    internal class UseEntityModelInspector: ExplicitlyDeclaredModel
    {
        /// <summary>
        /// 判断是否是 one Many
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public override bool IsOneToMany(MemberInfo member)
        {
            //获取附加属性
            PropertyAttribute useAttribute = member.GetCustomAttribute(typeof(PropertyAttribute)) as PropertyAttribute;

            if (null == useAttribute || false == useAttribute.IfIsManyToMany)
            {
                if (IsBag(member))
                {
                    return true;
                }
            }

            return base.IsOneToMany(member);
        }

        /// <summary>
        /// 判断是否是Many Many
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public override bool IsManyToManyItem(MemberInfo member)
        {
            //获取附加属性
            PropertyAttribute useAttribute = member.GetCustomAttribute(typeof(PropertyAttribute)) as PropertyAttribute;

            if (null != useAttribute && useAttribute.IfIsManyToMany)
            {
                if (IsBag(member))
                {
                    return true;
                }
            }

            return base.IsManyToManyItem(member);
        }

    }
}
