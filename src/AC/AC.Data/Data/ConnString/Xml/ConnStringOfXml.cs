namespace AC.Data.ConnString.Xml
{
    /// <summary>
    /// Xml文件配置字符串
    /// </summary>
    public class ConnStringOfXml : IConnectionString
    {
        #region IConnectionString Members

        /// <summary>
        /// 获取或设置字符串名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 获取或设置字符串提供商名称
        /// </summary>
        public string ProviderName { get; set; }

        #endregion

        /// <summary>
        /// 获取或设置该字符串服务器地址
        /// </summary>
        public string DbServer { get; set; }

        /// <summary>
        /// 获取或设置该字符串是否为默认字符串
        /// </summary>
        public bool IsDefault { get; set; }
    }
}