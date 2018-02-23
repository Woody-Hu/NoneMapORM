using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Dialect;

namespace NoneMapORM
{
    /// <summary>
    /// Sqlite连接设置
    /// </summary>
    public class SqlliteConfigurationPacker : ConfigurationPacker
    {
        /// <summary>
        /// 使用的Sqlite数据库文件
        /// </summary>
        private string m_useFileName;

        /// <summary>
        /// 构造连接设置
        /// </summary>
        /// <param name="inputDbFileFullName">使用的数据库问题</param>
        /// <param name="inputPassWord">使用的密码</param>
        public SqlliteConfigurationPacker(string inputDbFileFullName,string inputPassWord = null) : base()
        {
            //配置初始值
            m_useFileName = inputDbFileFullName;
            m_usePassWord = inputPassWord;
            m_strUseDirverFileName = "System.Data.SQLite.dll";
            m_dicUseProperties.Add(m_strDriverclass, "NHibernate.Driver.SQLite20Driver");
            m_dicUseProperties.Add(m_strDialect, "NHibernate.Dialect.SQLiteDialect");

        }

        /// <summary>
        /// 制作连接字符串
        /// </summary>
        /// <returns></returns>
        protected override string MakeConnectionString()
        {
            string returnValue = string.Empty;
            if (string.IsNullOrWhiteSpace(m_usePassWord))
            {
                returnValue = "Data Source = " + m_useFileName + ";Version=3";
            }
            else
            {
                returnValue = "Data Source = " + m_useFileName + ";Version=3;Password=" + m_usePassWord;
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
