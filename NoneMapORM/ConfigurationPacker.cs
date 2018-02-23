using NHibernate.Cfg;
using NHibernate.Dialect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NoneMapORM
{
    /// <summary>
    /// 使用的基础设置类
    /// </summary>
    public abstract class ConfigurationPacker
    {
        #region 保护字段
        /// <summary>
        /// 数据库方言元数据
        /// </summary>
        protected const string m_strDialect = "dialect";

        /// <summary>
        /// 连接提供器元数据
        /// </summary>
        protected const string m_strProvider = "connection.provider";

        /// <summary>
        /// 驱动元数据
        /// </summary>
        protected const string m_strDriverclass = "connection.driver_class";

        /// <summary>
        /// 连接字符串元数据
        /// </summary>
        protected const string m_strConnectionstring = "connection.connection_string";

        /// <summary>
        /// 默认的连接提供器
        /// </summary>
        protected const string m_strDefualtProvider = "NHibernate.Connection.DriverConnectionProvider";

        /// <summary>
        /// 是否在Console中显示Sql
        /// </summary>
        protected const string m_strShowSqlInConsole = "show_sql";

        /// <summary>
        /// Schema自动行为描述
        /// </summary>
        protected const string m_strSchemaAuto = "hbm2ddl.auto";

        /// <summary>
        /// 更新
        /// </summary>
        protected const string m_strUpdate = "update";

        /// <summary>
        /// 二级缓存提供程序特性
        /// </summary>
        protected const string m_strCache = "cache.provider_class";

        /// <summary>
        /// 默认的二级缓存提供类
        /// </summary>
        protected const string m_strDefualtChacheProvider = "NHibernate.Cache.HashtableCacheProvider";

        /// <summary>
        /// 开启二级缓存特性
        /// </summary>
        protected const string m_strUseChache = "cache.use_second_level_cache";

        /// <summary>
        /// 二级缓存应用查询特性
        /// </summary>
        protected const string m_strUseChacheForQuery = "cache.use_query_cache";

        /// <summary>
        /// 缓存工厂特性
        /// </summary>
        protected const string m_strUseCacheFactory = "cache.query_cache_factory";

        /// <summary>
        /// 逗号字符串
        /// </summary>
        protected const string m_strComma = ",";

        /// <summary>
        /// 使用的账号
        /// </summary>
        protected string m_useRootName;

        /// <summary>
        /// 使用的密码
        /// </summary>
        protected string m_usePassWord;

        /// <summary>
        /// 使用的数据库驱动文件
        /// </summary>
        protected string m_strUseDirverFileName = null;

        /// <summary>
        /// 使用的特性字典
        /// </summary>
        protected Dictionary<string, string> m_dicUseProperties = new Dictionary<string, string>();
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        protected ConfigurationPacker()
        {
            m_dicUseProperties.Add(m_strProvider, m_strDefualtProvider);
            //sql输出到标注输出
            m_dicUseProperties.Add(m_strShowSqlInConsole, true.ToString());
            m_dicUseProperties.Add(m_strSchemaAuto, m_strUpdate);
            m_dicUseProperties.Add(m_strCache, m_strDefualtChacheProvider);
            //默认使用二级缓存
            m_dicUseProperties.Add(m_strUseChacheForQuery, true.ToString());
            m_dicUseProperties.Add(m_strUseChache, true.ToString());
           
            //配置缓存工厂
            var useType = typeof(UseCacheFactory);
            m_dicUseProperties.Add(m_strUseCacheFactory, useType.FullName + m_strComma + useType.Assembly.FullName);


        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="rootName">账号</param>
        /// <param name="passWord">密码</param>
        protected ConfigurationPacker(string rootName, string passWord)
        {
            m_useRootName = rootName;
            m_usePassWord = passWord;
        }

        /// <summary>
        /// 制作连接字符串
        /// </summary>
        /// <returns>连接字符串</returns>
        protected abstract string MakeConnectionString();

        /// <summary>
        /// 获取使用的数据库方言
        /// </summary>
        /// <returns></returns>
        internal abstract Dialect GetUseDialect();

        /// <summary>
        /// 检查驱动是否存在
        /// </summary>
        /// <returns></returns>
        internal bool CheckDriverFileExist()
        {
            if (!string.IsNullOrWhiteSpace(m_strUseDirverFileName))
            {
                return false;
            }

            FileInfo useFileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);

            DirectoryInfo useDirectoryInfo = useFileInfo.Directory;

            return File.Exists(useDirectoryInfo.FullName + @"\" + m_strUseDirverFileName);

        }

        /// <summary>
        /// 向NHibernate添加设置
        /// </summary>
        /// <param name="inputNHibernateCfg"></param>
        internal void AddToNHibernateConfiguration(Configuration inputNHibernateCfg)
        {
            //若没有连接字符串
            if (!m_dicUseProperties.ContainsKey(m_strConnectionstring))
            {
                var connectString = MakeConnectionString();
                m_dicUseProperties.Add(m_strConnectionstring, connectString);
            }

            foreach (var oneKVP in m_dicUseProperties)
            {
                inputNHibernateCfg.Properties.Add(oneKVP);
            }

        }


    }
}
