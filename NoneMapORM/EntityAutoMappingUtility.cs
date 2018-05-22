using NHibernate.Mapping.ByCode;
using NoneMapORM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NoneMapORM
{
    /// <summary>
    /// 自动映射处理器
    /// </summary>
    internal static class EntityAutoMappingUtility
    {
        /// <summary>
        /// id属性名
        /// </summary>
        private const string m_strUseIdName = "Id";

        /// <summary>
        /// 获取Property方法名
        /// </summary>
        private const string m_strUsePropertyMethodName = "Property";

        /// <summary>
        /// 获取Bag方法名
        /// </summary>
        private const string m_strUseBagMethodName = "Bag";

        /// <summary>
        /// 获取ManyToOne方法名
        /// </summary>
        private const string m_strUseManyToOneMethodName = "ManyToOne";

        /// <summary>
        /// Bag多对多方法
        /// </summary>
        private const string m_strUseBagManyMethodName = "BagManyToMany";

        /// <summary>
        /// Entity基类类型
        /// </summary>
        private static Type m_baseEntityType = typeof(BaseEntity);

        /// <summary>
        /// IEnumerable 接口基础类
        /// </summary>
        private static Type m_baseIEnumerable = typeof(IEnumerable<>);

        /// <summary>
        /// 简单映射方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputMapper"></param>
        internal static void EntityMappingMethod<X>(IClassMapper<X> inputValue)
            where X : BaseEntity
        {
            //开启二级缓存
            inputValue.Cache(k => { k.Usage(CacheUsage.ReadWrite); } );

            //优先关闭延迟加载
            inputValue.Lazy(false);

            //绑定Id
            //设定为自增主键
            inputValue.Id(k => k.Id,map=>map.Generator(Generators.Identity));
            inputValue.EntityName(typeof(X).Name);

            Type useType = typeof(X);

            //获取属性设置器类型
            Type useMapperMehtodType = typeof(EntityAutoMappingUtility);

            //设置用方法
            MethodInfo useBasePropertyInfo = null;
            MethodInfo useBaseBagInfo = null;
            MethodInfo useBaseManyToOneInfo = null;
            MethodInfo useBaseBagManyInfo = null;

            useBasePropertyInfo = useMapperMehtodType.GetMethod(m_strUsePropertyMethodName, BindingFlags.Static | BindingFlags.NonPublic);
            useBaseBagInfo = useMapperMehtodType.GetMethod(m_strUseBagMethodName, BindingFlags.Static | BindingFlags.NonPublic);
            useBaseManyToOneInfo = useMapperMehtodType.GetMethod(m_strUseManyToOneMethodName, BindingFlags.Static | BindingFlags.NonPublic);
            useBaseBagManyInfo = useMapperMehtodType.GetMethod(m_strUseBagManyMethodName, BindingFlags.Static | BindingFlags.NonPublic);

            var allProperties = useType.GetProperties();

            //表达式变量
            ParameterExpression target = null;
            Expression getPropertyValue = null;
            MethodInfo realUsePopertyMethod = null;

            //遍历属性
            foreach (var onePropertie in allProperties)
            {
                //跳过Id
                if (onePropertie.Name.Equals(m_strUseIdName))
                {
                    continue;
                }

                //选取合适的特性
                if (onePropertie.CanRead && onePropertie.CanWrite && onePropertie.GetMethod.IsVirtual && onePropertie.SetMethod.IsVirtual)
                {
                    //获取附加属性
                    PropertyAttribute useAttribute = onePropertie.GetCustomAttribute(typeof(PropertyAttribute)) as PropertyAttribute;

                    //制作表达式
                    target = Expression.Parameter(useType);
                    getPropertyValue = Expression.Property(target, onePropertie);


                    //one - Many 行为
                    if (onePropertie.PropertyType.IsGenericCollection() && onePropertie.PropertyType.IsInterface &&
                        onePropertie.PropertyType.DetermineCollectionElementType().IsSubclassOf(m_baseEntityType))
                    {
                        //bag
                        if (onePropertie.PropertyType.GetGenericTypeDefinition() == m_baseIEnumerable)
                        {
                            //区分ManyToMany
                            if (null != useAttribute && useAttribute.IfIsManyToMany)
                            {
                                realUsePopertyMethod = useBaseBagManyInfo.MakeGenericMethod(new Type[] { useType, onePropertie.PropertyType.DetermineCollectionElementType() });

                                //不同的执行分支
                                realUsePopertyMethod.Invoke(null, new object[] { inputValue, Expression.Lambda(getPropertyValue, target), !useAttribute.IfIsManyToManyControl });

                                continue;
                            }
                            else
                            {
                                realUsePopertyMethod = useBaseBagInfo.MakeGenericMethod(new Type[] { useType, onePropertie.PropertyType.DetermineCollectionElementType() });
                            }
                        }
                        else
                        {
                            continue;
                        }
                        
                     
                    }
                    //Many - one 行为
                    else if (onePropertie.PropertyType.IsSubclassOf(m_baseEntityType))
                    {
                        realUsePopertyMethod = useBaseManyToOneInfo.MakeGenericMethod(new Type[] { useType, onePropertie.PropertyType });
             
                    }
                    //标准行为
                    else if (!((onePropertie.PropertyType.IsGenericCollection() && onePropertie.PropertyType.IsInterface)))
                    {
                        realUsePopertyMethod = useBasePropertyInfo.MakeGenericMethod(new Type[] { useType, onePropertie.PropertyType });
                    }
                    //不处理行为
                    else
                    {
                        continue;
                    }

                    realUsePopertyMethod.Invoke(null, new object[] {inputValue, Expression.Lambda(getPropertyValue, target) });

                }
                else
                {
                    continue;
                }
            }


        }

        /// <summary>
        /// 代理生成Action
        /// </summary>
        /// <typeparam name="X">Entity类型泛型</typeparam>
        /// <returns>生成的Action</returns>
        internal static Action<IClassMapper<X>> EntityMapping<X>()
            where X: BaseEntity
        {
            return new Action<IClassMapper<X>>(EntityMappingMethod);
        }

        /// <summary>
        /// 代理执行Propert
        /// </summary>
        /// <typeparam name="X">Entity类型泛型</typeparam>
        /// <typeparam name="T">参数类型泛型</typeparam>
        /// <param name="inputValue">输入的Mapper</param>
        /// <param name="inputProperty">输入的表达式参数</param>
        private static void Property<X,T>(IClassMapper<X> inputValue,Expression<Func<X,T>> inputProperty)
            where X : BaseEntity
        {
            inputValue.Property<T>(inputProperty);
        }

        /// <summary>
        /// 代理执行Bag方法 （one - Many 对照）
        /// </summary>
        /// <typeparam name="X">Entity类型泛型</typeparam>
        /// <typeparam name="T">参数类型泛型</typeparam>
        /// <param name="inputValue">输入的Mapper</param>
        /// <param name="inputBag">输入的表达式参数</param>
        private static void Bag<X,T>(IClassMapper<X> inputValue,Expression<Func<X,IEnumerable<T>>> inputBag)
             where X : BaseEntity
        {
            inputValue.Bag<T>(inputBag, bag => {  bag.Cascade(Cascade.All); bag.Inverse(true); bag.Cache(k => k.Usage(CacheUsage.ReadWrite)); });
        }

        /// <summary>
        /// ManyToMany设置
        /// </summary>
        /// <typeparam name="X">Entity类型泛型</typeparam>
        /// <typeparam name="T">参数类型泛型</typeparam>
        /// <param name="inputValue">输入的Mapper</param>
        /// <param name="inputBag">输入的表达式参数</param>
        private static void BagManyToMany<X, T>(IClassMapper<X> inputValue, Expression<Func<X, IEnumerable<T>>> inputBag,bool ifInVerse)
            where X : BaseEntity
        {
            var useThisType = typeof(X);
            var useAnotherType = typeof(T);
            bool useifInVerse = ifInVerse;

            //利用Hash值进行计算
            if (useThisType.GetHashCode() < useAnotherType.GetHashCode())
            {
                useifInVerse = false;
            }
            else if (useThisType.GetHashCode() > useAnotherType.GetHashCode())
            {
                useifInVerse = true;
            }


            inputValue.Bag<T>(inputBag, bag => 
            { bag.Key(c => c.Column(useThisType.Name)); bag.Cascade(Cascade.All);
                bag.Inverse(useifInVerse); bag.Cache(k => k.Usage(CacheUsage.ReadWrite));
            }, mapper => mapper.ManyToMany(c => c.Column(useAnotherType.Name)));
        }

        /// <summary>
        /// 代理执行Many - one方法
        /// </summary>
        /// <typeparam name="X">Entity类型泛型</typeparam>
        /// <typeparam name="T">参数类型泛型</typeparam>
        /// <param name="inputValue">输入的Mapper</param>
        /// <param name="inputProperty">输入的表达式参数</param>
        private static void ManyToOne<X, T>(IClassMapper<X> inputValue, Expression<Func<X, T>> inputProperty)
            where X : BaseEntity
            where T : BaseEntity
        {
            inputValue.ManyToOne<T>(inputProperty );
        }

    }
}
