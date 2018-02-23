using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoneMapORM
{
    /// <summary>
    /// 自动映射处理器
    /// </summary>
    internal class EntityAutoMappingHandler
    {
        #region 私有字段
        /// <summary>
        /// 反射的方法基础名
        /// </summary>
        private const string m_strUseMethodName = "Class";

        /// <summary>
        /// 反射的自动映射方法名
        /// </summary>
        private const string m_strUseSimpleMethodName = "EntityMapping";

        /// <summary>
        /// 使用的Mapper
        /// </summary>
        private ModelMapper m_useModelMapper = new ModelMapper(new UseEntityModelInspector());

        /// <summary>
        /// Mapper的类型
        /// </summary>
        private Type useType = typeof(ModelMapper);

        /// <summary>
        /// Mape-by-code对应方法
        /// </summary>
        private MethodInfo m_useMethod;

        /// <summary>
        /// 自动映射方法
        /// </summary>
        private MethodInfo m_useSimpleUtiltiyMehtod = null; 
        #endregion

        /// <summary>
        /// 构造处理器
        /// </summary>
        internal EntityAutoMappingHandler()
        {
            PrepareMethod();
        }

        /// <summary>
        /// 准备反射方法
        /// </summary>
        private void PrepareMethod()
        {
            m_useMethod = useType.GetMethod(m_strUseMethodName);

            m_useSimpleUtiltiyMehtod = typeof(EntityAutoMappingUtility).GetMethod(m_strUseSimpleMethodName,BindingFlags.Static| BindingFlags.NonPublic);
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="input">需注册的类型</param>
        internal void RegiesType(Type input)
        {
            //获得实际使用泛型方法
            var realUseMethod = m_useMethod.MakeGenericMethod(input);

            //获得使用的底层方法
            var useUtilityMethod = m_useSimpleUtiltiyMehtod.MakeGenericMethod(input);

            //获取Action对象
            var useAction = useUtilityMethod.Invoke(null, null);

            //注册
            realUseMethod.Invoke(m_useModelMapper, new object[] { useAction });

        }

        /// <summary>
        /// 获得的Hbm对象
        /// </summary>
        /// <returns></returns>
        internal HbmMapping GetHbmMapping()
        {
            var useHbm = m_useModelMapper.CompileMappingForAllExplicitlyAddedEntities();
            return useHbm;
        }
    }
}
