using NHibernate;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Impl;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoneMapORM
{
    /// <summary>
    /// ORM关联器
    /// </summary>
    public class ORMManger
    {
        #region 私有字段
        /// <summary>
        /// 使用的整体配置
        /// </summary>
        private ConfigurationPacker m_useConfiguartionPacker = null;

        /// <summary>
        /// 使用的自动映射处理器
        /// </summary>
        private EntityAutoMappingHandler m_useAutoMappingHandler = null;

        /// <summary>
        /// 使用的Hibernate类
        /// </summary>
        private Configuration m_hibernateConfig = null;

        /// <summary>
        /// 会话工厂
        /// </summary>
        private ISessionFactory m_useSessionFactory = null;

        /// <summary>
        /// 使用的会话
        /// </summary>
        private ISession m_currentSession = null;

        /// <summary>
        /// 当前使用的事务对象
        /// </summary>
        private ITransaction m_useTransaction = null;
        #endregion

        #region 构造与析构
        /// <summary>
        /// 构造管理器
        /// </summary>
        /// <param name="inputConfiguartionPacker">输入的数据库设置</param>
        /// <param name="lstTypeNeedMap">需注册的类型</param>
        public ORMManger(ConfigurationPacker inputConfiguartionPacker, List<Type> lstTypeNeedMap)
        {
            PrepareData(inputConfiguartionPacker, lstTypeNeedMap);
        }

        /// <summary>
        /// 构造管理器
        /// </summary>
        /// <param name="inputConfiguartionPacker">输入的数据库设置</param>
        public ORMManger(ConfigurationPacker inputConfiguartionPacker)
        {
            Assembly useAssembly = Assembly.GetCallingAssembly();

            Type useBaseType = typeof(BaseEntity);

            var useTypes = from n in useAssembly.GetTypes() where useBaseType.IsAssignableFrom(n) select n;

            PrepareData(inputConfiguartionPacker, useTypes.ToList());
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="inputConfiguartionPacker"></param>
        /// <param name="lstTypeNeedMap"></param>
        private void PrepareData(ConfigurationPacker inputConfiguartionPacker, List<Type> lstTypeNeedMap)
        {
            m_hibernateConfig = new Configuration();
            m_useConfiguartionPacker = inputConfiguartionPacker;
            m_useAutoMappingHandler = new EntityAutoMappingHandler();

            Type baseType = typeof(BaseEntity);

            //过滤基类
            var useFullType = from n in lstTypeNeedMap where baseType.IsAssignableFrom(n) select n;

            foreach (var oneType in useFullType)
            {
                m_useAutoMappingHandler.RegiesType(oneType);
            }

            //获得映射
            var useHbm = m_useAutoMappingHandler.GetHbmMapping();

            //设置配置
            m_useConfiguartionPacker.AddToNHibernateConfiguration(m_hibernateConfig);

            //注册映射
            m_hibernateConfig.AddDeserializedMapping(useHbm, Guid.NewGuid().ToString());

            //创建会话工厂
            m_useSessionFactory = m_hibernateConfig.BuildSessionFactory();
        }


        /// <summary>
        /// 析构函数
        /// </summary>
        ~ORMManger()
        {
            if (null != m_currentSession)
            {
                CloseSession();
            }

            try
            {
                m_useSessionFactory.Close();
                m_useSessionFactory.Dispose();
            }
            catch (Exception)
            {
                m_useSessionFactory = null;
            }

        }
        #endregion

        #region 会话方法
        /// <summary>
        /// 打开会话
        /// </summary>
        public void OpenSession()
        {
            if (null == m_currentSession)
            {
                //关闭事务
                FinishTransaction(false);
                m_currentSession = m_useSessionFactory.OpenSession();
                //打开事务
                m_useTransaction = m_currentSession.BeginTransaction();
            }
        }

        /// <summary>
        /// 关闭会话
        /// </summary>
        /// <param name="ifRollBack">是否回滚</param>
        public void CloseSession(bool ifRollBack = false)
        {
            if (null != m_currentSession)
            {
                try
                {
                    //关闭事务
                    FinishTransaction(ifRollBack);
                    m_currentSession.Close();
                    m_currentSession.Dispose();
                }
                catch (Exception)
                {
                    ;
                }
                finally
                {
                    m_currentSession = null;
                }
            }
        }

        /// <summary>
        /// 关闭事务
        /// </summary>
        /// <param name="infRollBack">是否回滚</param>
        private void FinishTransaction(bool infRollBack = false)
        {
            if (null != m_useTransaction)
            {
                try
                {
                    if (!infRollBack)
                    {
                        m_useTransaction.Commit();
                    }
                    else
                    {
                        m_useTransaction.Rollback();
                    }
                   
                }
                catch (Exception)
                {
                    m_useTransaction.Rollback();
                }
                finally
                {
                    m_useTransaction = null;
                }

            }
        }
        #endregion

        #region 增删库表
        /// <summary>
        /// 创建数据库架构
        /// </summary>
        private void CreatSchema()
        {
            SchemaMetadataUpdater.QuoteTableAndColumns(m_hibernateConfig, m_useConfiguartionPacker.GetUseDialect());

            new SchemaExport(m_hibernateConfig).Create(false, true);
        }

        /// <summary>
        /// 删除数据库架构
        /// </summary>
        private void DropSchema()
        {
            new SchemaExport(m_hibernateConfig).Drop(false, true);
        }
        #endregion

        #region 增查删改
        /// <summary>
        /// 根据Id获取
        /// </summary>
        /// <typeparam name="T">需获取的泛型</typeparam>
        /// <param name="inputId">输入的主键Id</param>
        /// <param name="findedValue">需要获取的值</param>
        /// <param name="ifMangeSession">是否管理会话</param>
        /// <returns>是否成功</returns>
        public bool TryGetById<T>(int inputId, out T findedValue,bool ifMangeSession = true)
            where T : BaseEntity
        {
            findedValue = null;

            bool ifRollBack = false;

            try
            {
                if (ifMangeSession)
                {
                    OpenSession();
                }

                findedValue = m_currentSession.Get<T>(inputId);

                return true;
            }
            catch (Exception)
            {
                ifRollBack = true;
                return false;
            }
            finally
            {
                if (ifMangeSession)
                {
                    CloseSession(ifRollBack);
                }
            }
        }

        /// <summary>
        /// 获得所有结果
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="finedValue">找到的结果</param>
        /// <param name="ifMangeSession">是否管理会话</param>
        /// <returns>是否成功</returns>
        public bool TryGetAll<T>(out List<T> finedValue, bool ifMangeSession = true)
             where T : BaseEntity
        {
            finedValue = null;
            bool ifRollBack = false;

            try
            {
                if (ifMangeSession)
                {
                    OpenSession();
                }


                //使用查询缓存
                ICriteria crit = m_currentSession.CreateCriteria(typeof(T)).SetCacheable(true);

                finedValue = crit.List<T>().ToList();


                return true;
            }
            catch (Exception)
            {
                ifRollBack = true;
                return false;
            }
            finally
            {
                if (ifMangeSession)
                {
                    CloseSession(ifRollBack);
                }
            }

        }

        /// <summary>
        /// 过滤性获取
        /// </summary>
        /// <typeparam name="T">使用的泛型</typeparam>
        /// <param name="useFilter">使用的过滤委托</param>
        /// <param name="finedValue">找到的值</param>
        /// <param name="ifMangeSession">是否管理会话</param>
        /// <returns>是否成功</returns>
        public bool TryGetByFilter<T>(EntityFilterDel<T> useFilter, out List<T> finedValue, bool ifMangeSession = true)
            where T : BaseEntity
        {
            finedValue = null;
            bool ifRollBack = false;

            try
            {
                if (ifMangeSession)
                {
                    OpenSession();
                }
                ICriteria crit = m_currentSession.CreateCriteria(typeof(T)).SetCacheable(true);
                finedValue = (from n in crit.List<T>() where true == useFilter(n) select n).ToList();

                return true;
            }
            catch (Exception)
            {
                ifRollBack = true;
                return false;
            }
            finally
            {
                if (ifMangeSession)
                {
                    CloseSession(ifRollBack);
                }
            }

        }

        /// <summary>
        /// 尝试新增或更新
        /// </summary>
        /// <param name="inputEntity">输入的对象</param>
        /// <param name="ifMangeSession">是否管理会话</param>
        /// <returns>是否成功</returns>
        public bool TrySaveOrUpDate(BaseEntity inputEntity, bool ifMangeSession = true)
        {
            bool ifRollBack = false;
            try
            {
                if (ifMangeSession)
                {
                    OpenSession();
                }

                m_currentSession.SaveOrUpdate(inputEntity);
                m_currentSession.Flush();
                return true;
            }
            catch (Exception)
            {
                ifRollBack = true;
                return false;
            }
            finally
            {
                if (ifMangeSession)
                {
                    CloseSession(ifRollBack);
                }
            }
        }

        /// <summary>
        /// 尝试删除
        /// </summary>
        /// <param name="inputEntity">输入的对象</param>
        /// <param name="ifMangeSession">是否管理会话</param>
        /// <returns>是否成功</returns>
        public bool TryDelete(BaseEntity inputEntity, bool ifMangeSession = true)
        {
            bool ifRollBack = false;
            try
            {
                if (ifMangeSession)
                {
                    OpenSession();
                }
                m_currentSession.Delete(inputEntity);
                m_currentSession.Flush();
                return true;
            }
            catch (Exception)
            {
                ifRollBack = true;
                return false;
            }
            finally
            {
                if (ifMangeSession)
                {
                    CloseSession(ifRollBack);
                }
            }
        }

        /// <summary>
        /// 尝试更新实体到数据库
        /// </summary>
        /// <param name="inputEntity">输入的实体</param>
        /// <param name="ifMangeSession">是否管理会话</param>
        /// <returns>是否成功</returns>
        private bool TryUpDate(BaseEntity inputEntity, bool ifMangeSession = true)
        {
            bool ifRollBack = false;
            try
            {
                if (ifMangeSession)
                {
                    OpenSession();
                }
                m_currentSession.Update(inputEntity);
                m_currentSession.Flush();
                return true;
            }
            catch (Exception)
            {
                ifRollBack = true;
                return false;
            }
            finally
            {
                if (ifMangeSession)
                {
                    CloseSession(ifRollBack);
                }
            }

        }

        /// <summary>
        /// 尝试保存
        /// </summary>
        /// <param name="inputEntity">输入的对象</param>
        /// <param name="ifMangeSession">是否管理会话</param>
        /// <returns>是否成功</returns>
        private bool TrySave(BaseEntity inputEntity,bool ifMangeSession = true)
        {
            bool ifRollBack = false;
            try
            {
                if (ifMangeSession)
                {
                    OpenSession();
                }
                m_currentSession.Save(inputEntity);
                m_currentSession.Flush();
                return true;
            }
            catch (Exception)
            {
                ifRollBack = true;
                return false;
            }
            finally
            {
                if (ifMangeSession)
                {
                    CloseSession(ifRollBack);
                }
            }

        }


        #endregion

    }

}
