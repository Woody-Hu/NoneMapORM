using NHibernate.Dialect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoneMapORM
{
    /// <summary>
    /// Mysql连接设置
    /// </summary>
    public class MysqlConfigurationPacker:ConfigurationPacker
    {
        #region 私有字段
        /// <summary>
        /// 使用的Mysql数据库名称
        /// </summary>
        private string m_useDatabaseName;

        /// <summary>
        /// 使用的Mysql数据库地址
        /// </summary>
        private string m_useDbIP;

        /// <summary>
        /// 使用的Mysql数据库端口号
        /// </summary>
        private string m_usePort;

        /// <summary>
        /// Mysql默认端口号
        /// </summary>
        private const string DEFAULTPORT = "3306"; 
        #endregion

        /// <summary>
        /// 构造连接设置
        /// </summary>
        /// <param name="inputDbName">数据库名称</param>
        /// <param name="inputDbIP">数据库IP</param>
        /// <param name="inputUserName">用户名</param>
        /// <param name="inputPassWord">密码</param>
        /// <param name="inputPort">端口号</param>
        public MysqlConfigurationPacker(string inputDbName, string inputDbIP, 
            string inputUserName, string inputPassWord = null, string inputPort = DEFAULTPORT) : base()
        {
            //配置初始值
            m_useDatabaseName = inputDbName;
            m_useDbIP = inputDbIP;
            m_usePort = inputPort;
            m_usePassWord = inputPassWord;
            m_useRootName = inputUserName;
            m_strUseDirverFileName = "MySql.Data.dll";
            m_dicUseProperties.Add(m_strDriverclass, "NHibernate.Driver.MySqlDataDriver");
            m_dicUseProperties.Add(m_strDialect, "NHibernate.Dialect.MySQLDialect");
            m_dicUseProperties.Add("hbm2ddl.keywords", "none");    
              
        }

        /// <summary>
        /// 制作连接字符串
        /// </summary>
        /// <returns></returns>
        protected override string MakeConnectionString()
        {
            string returnValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(m_usePassWord) && !string.IsNullOrWhiteSpace(m_useRootName))
            {
                returnValue = "Database=" + m_useDatabaseName + ";Data Source=" + m_useDbIP + ";User Id=" + m_useRootName
                    + ";Password=" + m_usePassWord + ";port=" + m_usePort + ";pooling=false;CharSet=utf8" ; 
            }
            else if (!string.IsNullOrWhiteSpace(m_useRootName) && string.IsNullOrWhiteSpace(m_usePassWord))
            {
                returnValue = "Database=" + m_useDatabaseName + ";Data Source=" + m_useDbIP + ";User Id=" + m_useRootName;
            }

            return returnValue;
        }

        /// <summary>
        /// 获取使用的方言
        /// </summary>
        /// <returns></returns>
        internal override Dialect GetUseDialect()
        {
            return new SQLiteDialect();
        }

    }
}
