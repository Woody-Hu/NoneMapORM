using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoneMapORM
{
    /// <summary>
    /// 实体用特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = false)]
    public class EntityAttribute:Attribute
    {
        /// <summary>
        /// 映射的表格名称
        /// </summary>
        private string m_strUseTableName;

        /// <summary>
        /// 映射的表格名称
        /// </summary>
        public string UseTableName
        {
            get
            {
                return m_strUseTableName;
            }

            set
            {
                m_strUseTableName = value;
            }
        }
    }
}
