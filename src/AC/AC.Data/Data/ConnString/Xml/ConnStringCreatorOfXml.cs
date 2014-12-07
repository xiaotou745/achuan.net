using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using AC.Data.ConnString.Config;
using AC.Util;
using Common.Logging;

namespace AC.Data.ConnString.Xml
{
    /// <summary>
    /// 用来创建Xml配置文件中的定义的字符串
    /// </summary>
    public class ConnStringCreatorOfXml : IConnectionStringCreator
    {
        #region Logger Definition

        private readonly ILog logger = DataLogManager.GetConnectionStringLog();

        #endregion

        #region Private Instances and Fields

        /// <summary>
        /// Xml配置文件路径
        /// </summary>
        private string configFilePath;

        /// <summary>
        /// 配置文件中的字符串列表
        /// </summary>
        private List<IConnectionString> lstConnStrings;

        /// <summary>
        /// 获取web.config中配置的文件连接字符串xml配置文件路径
        /// </summary>
        protected string XmlFilePath
        {
            get { return ConfigUtils.GetConfigValue("ConnStringConfigPath", string.Empty); }
        }

        #endregion

        #region Private Constructor.

        /// <summary>
        /// 私有构造函数，创建此对象请使用静态方法:<see cref="Create"/>
        /// </summary>
        private ConnStringCreatorOfXml()
        {
        }

        /// <summary>
        /// 默认从web.config中读取配置文件
        /// </summary>
        /// <returns>返回一个<see cref="ConnStringCreatorOfXml"/>对象</returns>
        public static ConnStringCreatorOfXml Create()
        {
            var connStringCreatorOfXml = new ConnStringCreatorOfXml();
            if (string.IsNullOrEmpty(connStringCreatorOfXml.XmlFilePath))
            {
                connStringCreatorOfXml.lstConnStrings = new List<IConnectionString>();
            }
            else
            {
                connStringCreatorOfXml.Init(connStringCreatorOfXml.XmlFilePath);
            }
            return connStringCreatorOfXml;
        }

        /// <summary>
        /// Create a instance of class <see cref="ConnStringCreatorOfConfig"/>
        /// 创建一个本身的对象，唯一创建对象入口
        /// </summary>
        /// <returns>返回一个<see cref="ConnStringCreatorOfXml"/>对象</returns>
        public static ConnStringCreatorOfXml Create(string xmlFilePath)
        {
            var connStringCreatorOfXml = new ConnStringCreatorOfXml
                {
                    configFilePath = xmlFilePath
                };
            connStringCreatorOfXml.Init(xmlFilePath);
            return connStringCreatorOfXml;
        }

        /// <summary>
        /// 初始化,根据xml文件路径加载xml文件内容，并初始化所有连接字符串
        /// </summary>
        /// <param name="filePath">表示连接字符串设置的xml配置文件路径</param>
        private void Init(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("no file exists." + filePath);
            }

            var xml = new XmlDocument();
            xml.Load(filePath);

            XmlNodeList sectionNodes = xml.GetElementsByTagName("connectionString");

            foreach (XmlNode sectionNode in sectionNodes)
            {
                var connStringOfXml = new ConnStringOfXml
                    {
                        ConnectionString = sectionNode.Attributes["connectionString"].ToString(),
                        DbServer = sectionNode.Attributes["dbServer"].ToString(),
                        IsDefault = bool.Parse(sectionNode.Attributes["isDefault"].ToString()),
                        Name = sectionNode.Attributes["name"].ToString(),
                        ProviderName = sectionNode.Attributes["providerName"].ToString(),
                    };

                lstConnStrings.Add(connStringOfXml);
            }
        }

        #endregion

        /// <summary>
        /// ToString Override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return typeof (ConnStringCreatorOfXml).ToString();
        }

        #region IConnectionStringCreator Members

        public IList<IConnectionString> CreateConnStrings()
        {
            return lstConnStrings;
        }

        #endregion

        /// <summary>
        /// 获取所有的Db对象
        /// </summary>
        /// <returns>返回xml文件中表示的所有DbSettings对象</returns>
        public IList<string> GetDbServers()
        {
            IList<string> result = new List<string>();
            foreach (var connectionString in lstConnStrings)
            {
                var connStringOfXml = connectionString as ConnStringOfXml;
                result.Add(connStringOfXml.DbServer);
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
            IConnectionString connString = lstConnStrings.FirstOrDefault(c =>
                {
                    var connStringOfXml = (ConnStringOfXml) c;
                    return connStringOfXml.DbServer == dbServer;
                });
            if (connString == null)
            {
                return string.Empty;
            }
            return connString.ConnectionString;
        }
    }
}