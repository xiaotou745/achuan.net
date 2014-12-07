using System.Collections.Generic;
using System.IO;
using System.Xml;
using AC.Code.DbObjects;

namespace AC.Code.Config
{
    /// <summary>
    /// 数据库设置类
    /// </summary>
    public class DbSettings
    {
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string DbServer { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnString { get; set; }
        
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbType DbType { get; set; }
    }

    /// <summary>
    /// 数据库设置工厂类
    /// </summary>
    public class DbSettingsBuilder
    {
        private readonly IList<DbSettings> lstDbSettings = new List<DbSettings>();

        #region 私有的构造函数

        private DbSettingsBuilder()
        {
        }

        #endregion

        #region 创建本工厂对象的静态方法入口

        /// <summary>
        /// 创建一个本身的对象，唯一创建对象入口
        /// </summary>
        /// <param name="filePath">表示数据库设置的xml配置文件路径</param>
        /// <returns>返回一个<see cref="DbSettingsBuilder"/>对象</returns>
        public static DbSettingsBuilder Create(string filePath)
        {
            var result = new DbSettingsBuilder();
            result.Init(filePath);
            return result;
        }

        /// <summary>
        /// 初始化,根据xml文件路径加载xml文件内容，并初始化lstDbSettings
        /// </summary>
        /// <param name="filePath">表示数据库设置的xml配置文件路径</param>
        private void Init(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("no file exists." + filePath);
            }

            var xml = new XmlDocument();
            xml.Load(filePath);

            XmlNodeList sectionNodes = xml.GetElementsByTagName("dbserver");

            foreach (XmlNode sectionNode in sectionNodes)
            {
                string servername = sectionNode.Attributes["servername"].Value;
                string connstring = sectionNode.Attributes["connstring"].Value;

                lstDbSettings.Add(new DbSettings {ConnString = connstring, DbServer = servername});
            }
        }

        #endregion

        #region 公共方法 对外

        /// <summary>
        /// 获取所有的DbSettings对象
        /// </summary>
        /// <returns>返回xml文件中表示的所有DbSettings对象</returns>
        public IList<string> GetDbServers()
        {
            IList<string> result = new List<string>();
            foreach (DbSettings dbSetting in lstDbSettings)
            {
                result.Add(dbSetting.DbServer);
            }
            return result;
        }

        /// <summary>
        /// 获取指定dbServer对应的数据库连接字符串
        /// </summary>
        /// <param name="dbServer">DbServer</param>
        /// <returns>获取指定dbServer对应的数据库连接字符串</returns>
        public string GetConnString(string dbServer)
        {
            foreach (DbSettings dbSetting in lstDbSettings)
            {
                if (dbSetting.DbServer == dbServer)
                {
                    return dbSetting.ConnString;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 创建dbServer对应的<see cref="IDbObject"/>对象，默认类型为SQL2005
        /// </summary>
        /// <param name="dbServer">DbServer</param>
        /// <returns>返回dbServer对应的<see cref="IDbObject"/>对象，默认类型为SQL2005</returns>
        public IDbObject CreateDbObject(string dbServer)
        {
            return DbFactory.CreateDbObj(DbType.SQL2005, GetConnString(dbServer));
        }

        #endregion
    }
}