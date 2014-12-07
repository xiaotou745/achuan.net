using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using AC.Data.ConnString.Config;
using AC.Security;
using AC.Util;
using Common.Logging;

namespace AC.Data.ConnString
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnStringUtil
    {
        #region Private Instances or Fields
        /// <summary>
        /// 重试次数
        /// </summary>
        private const int RETRY_COUNT = 5;

        /// <summary>
        /// 线程锁？
        /// </summary>
        private static readonly object obj = new object(); 

        /// <summary>
        /// [key:connStringName,value:connectionString]
        /// </summary>
        private static readonly IDictionary<string, string> ConnStringCache = new Dictionary<string, string>();

        /// <summary>
        /// [key:connStringName,value:dbProviderName]
        /// </summary>
        private static readonly IDictionary<string, string> ProviderNameCache = new Dictionary<string, string>();

        /// <summary>
        /// lock object.
        /// </summary>
        private static readonly object DbListObj = new object();

        /// <summary>
        /// the WMS Connection Strings.
        /// </summary>
        private static IList<IConnectionString> LstConnStrings;

        #endregion

        #region Logger Definition.

        private static readonly ILog Logger = DataLogManager.GetConnectionStringLog();

        #endregion

        #region Init ConnectionStrings.

        /// <summary>
        /// 初始连接字符串
        /// </summary>
        public static void InitConnList()
        {
            lock (obj) //对象同步 许亚修改2014.3.14
            {
                //获取连接字符串是否成功
                bool getConnectionSuccess = false;
                //获取连接字符串次数
                int getConnectionCounter = 0;

                while (!getConnectionSuccess && getConnectionCounter <= RETRY_COUNT)
                {
                    try
                    {
                        Logger.InfoFormat("第{0}次获取连接字符串开始...", getConnectionCounter + 1);
                        lock (DbListObj)
                        {
                            LstConnStrings = ConnectionStringFactory.Create(ConnStringCreatorOfConfig.Create())
                                                                     .GetConnectionStrings();

                            if (LstConnStrings.Count == 0)
                            {
                                throw new Exception("没有要使用的字符串.请检查配置文件");
                            }
                            //日志
                            if (Logger.IsInfoEnabled)
                            {
                                var stringBuilder = new StringBuilder();
                                stringBuilder.Append(string.Format("ConnectionString初始化完成，共有数据库连接字符串：{0}个",
                                                                   LstConnStrings.Count));
                                foreach (IConnectionString databaseModel in LstConnStrings)
                                {
                                    stringBuilder.AppendLine(databaseModel.ToString());
                                }
                                Logger.Info(stringBuilder.ToString());
                            }
                        }
                        getConnectionSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message, ex);
                        Thread.Sleep(500);
                        getConnectionCounter++;
                    }
                }
                foreach (IConnectionString connString in LstConnStrings)
                {
                    try
                    {
                        if (!IsPlainText(connString.ConnectionString))
                        {
                            connString.ConnectionString = DES.Decrypt3DES(connString.ConnectionString);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("解密连接串失败：name:" + connString.Name + "  message:" + ex.Message, ex);
                    }
                    if (!ConnStringCache.ContainsKey(connString.Name))
                    {
                        ConnStringCache.Add(connString.Name, connString.ConnectionString);
                    }
                    if (!ProviderNameCache.ContainsKey(connString.Name))
                    {
                        ProviderNameCache.Add(connString.Name, connString.ProviderName);
                    }
                }
            }
        }

        private static bool IsPlainText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return true;

            return text.Contains(";");
        }

        /// <summary>
        /// 如果没有初始化，则初始化之
        /// </summary>
        private static void CheckedInited()
        {
            if (LstConnStrings == null)
            {
                Logger.Info("ConnectionStringUtil not be initional, init it.");
                InitConnList();
            }
        }

        #endregion

        /// <summary>
        /// 获取所有WMS连接字符串
        /// </summary>
        /// <returns></returns>
        public static IList<IConnectionString> GetAllList()
        {
            CheckedInited();
            return LstConnStrings;
        }

        /// <summary>
        /// Get DB ProviderName
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        public static string GetDbProviderName(string connName)
        {
            AssertUtils.StringNotNullOrEmpty(connName, "connName");

            CheckedInited();

            if (ProviderNameCache.ContainsKey(connName))
            {
                return ProviderNameCache[connName];
            }

            foreach (IConnectionString connString in LstConnStrings)
            {
                if (connString.Name == connName)
                {
                    return connString.ProviderName;
                }
            }
            return null;
        }

        #region GetConnectionString.

        /// <summary>
        /// 根据字符串名称获取连接字符串
        /// </summary>
        /// <param name="connName">A ConnStringName.</param>
        /// <returns></returns>
        public static string GetConnectionString(string connName)
        {
            AssertUtils.StringNotNullOrEmpty(connName, "connName");

            CheckedInited();

            if (ConnStringCache.ContainsKey(connName))
            {
                Logger.Debug(string.Format("从connStringCache中获取字符串，字符串名称：{0}", connName));
                return ConnStringCache[connName];
            }

            foreach (IConnectionString connString in LstConnStrings)
            {
                if (connString.Name == connName)
                {
                    return connString.ConnectionString;
                }
            }
            return null;
        }

        #endregion
    }
}