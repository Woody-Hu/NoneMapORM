using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoneMapORM
{
    /// <summary>
    /// 属性特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false,Inherited = false)]
    public class PropertyAttribute:Attribute
    {

        /// <summary>
        /// 是否是多对多模式
        /// </summary>
        private bool m_ifIsManyToMany = false;

        /// <summary>
        /// 是否是多对多模式的主控方
        /// </summary>
        private bool m_ifIsManyToManyControl = false;


        /// <summary>
        /// 是否是多对多模式
        /// </summary>
        public bool IfIsManyToMany
        {
            get
            {
                return m_ifIsManyToMany;
            }

            set
            {
                m_ifIsManyToMany = value;
            }
        }

        /// <summary>
        /// 是否是多对多模式的主控方
        /// </summary>
        public bool IfIsManyToManyControl
        {
            get
            {
                return m_ifIsManyToManyControl;
            }

            set
            {
                m_ifIsManyToManyControl = value;
            }
        }
    }
}
