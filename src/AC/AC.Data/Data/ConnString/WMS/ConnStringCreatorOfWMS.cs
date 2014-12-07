using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using AC.Dao;
using AC.Data.ConnString.Config;
using AC.Data.Generic;
using Common.Logging;
using AC.Security;
using AC.Util;

namespace AC.Data.ConnString.WMS
{
	/// <summary>
	/// Vancl.WMS 的连接串创建类
	/// </summary>
	/// <remarks>
	/// <p>
	/// 需要在应用程序配置文件中的<connectionStrings></connectionStrings>配置节中配置两个字符串
	/// 字符串名称分别固定为dbConfigConnStringTest和dbConfigConnString，分别用来表示测试环境连接字符串
	/// 和生产环境连接字符串
	/// </p>
	/// <p>
	/// 在<appSettings></appSettings>配置节中添加节点key="IsDebug" value="false"用来标识是用测试还是生产环境的连接字符串
	/// 如果不进行配置，默认为false.
	/// </p>
	/// </remarks>
	/// <example>
	/// <code>
	/// <connectionStrings>
	///		<add name="dbConfigConnStringTest" connectionString="" providerName="System.Data.SqlClient"/>
	///		<add name="dbConfigConnString" connectionString="" providerName="System.Data.SqlClient"/>
	/// </connectionStrings>
	/// <appSettings>
	///		<add key="IsDebug" value="true"/>
	/// </appSettings>
	/// </code>
	/// </example>
	public class ConnStringCreatorOfWMS : IConnectionStringCreator
	{
		#region Logger Definition

		private readonly ILog logger = DataLogManager.GetConnectionStringLog();

		#endregion

		private const string SELECT_SQL =
			@"select wd.EnumValue,wd.DatabaseName,wd.WarehouseId,wd.WebCookieValue,wd.ConnectionString,wd.DbType
from WMSDatabases wd(nolock)";

		#region Private Constructor.

		private ConnStringCreatorOfWMS()
		{
		}

		#endregion

		#region Properties.

		/// <summary>
		/// 是否使用测试连接字符串
		/// </summary>
		private bool IsDebug
		{
			get
			{
				string configValue = ConfigUtils.GetConfigValue("IsDebug", "true");
				return bool.Parse(configValue);
			}
		}

		/// <summary>
		/// 指向数据库地址的连接字符串
		/// </summary>
		private IConnectionString ConfigConnString
		{
			get
			{
				string connName = IsDebug ? "dbConfigConnStringTest" : "dbConfigConnString";

			    IConnectionString configConnString = ConnectionStringFactory.Create(ConnStringCreatorOfConfig.Create())
			                                                                .GetConnectionString(connName);
				if (configConnString == null)
				{
					throw new ConnStringCreateException(string.Format("[ConnStringCreatorOfWMS.ConfigConnString]配置文件中连接字符串{0}没有进行设置，请设置后重试.", connName));
				}
				return configConnString;
			}
		}

		#endregion

		#region IConnectionStringCreator Members

		/// <summary>
		/// 从WMSDatabases表中获取WMS连接字符串
		/// </summary>
		/// <returns></returns>
		public IList<IConnectionString> CreateConnStrings()
		{
			try
			{
				var dbHelper = new DbHelper(ConfigConnString.ProviderName);
				string connString = DES.Decrypt3DES(ConfigConnString.ConnectionString);

			    IList<ConnStringOfWMS> lstWMSConnStrings = dbHelper.QueryWithRowMapper(connString, SELECT_SQL,
			                                                                           new WMSConnStringRowMapper());

				logger.Info("Execute sql by DbHelper:" + SELECT_SQL );
				if(logger.IsDebugEnabled)
				{
					logger.Debug("Execute sql on Database:" + connString);
				}

				IList<IConnectionString> results = new List<IConnectionString>();
				foreach (ConnStringOfWMS wmsConnetionString in lstWMSConnStrings)
				{
					results.Add(wmsConnetionString);
				}

				if (logger.IsInfoEnabled)
				{
					var logText = new StringBuilder();
					logText.AppendLine(string.Format("WMS数据库WMSDatabase字符串配置表中共有字符串数量：{0}.", results.Count));
					foreach (IConnectionString connectionString in results)
					{
						logText.AppendLine(string.Format("WMSConenctionString:{0}", connectionString));
					}
					logger.Info(logText.ToString());
				}
				return results;
			}
			catch (Exception exception)
			{
				string errorMsg = string.Format("[ConnStringCreatorOfWMS.CreateConnStrings] Error:{0}", exception.Message);
				logger.Error(errorMsg);
				throw new ConnStringCreateException(errorMsg, exception);
			}
		}

		#endregion

		/// <summary>
		/// Create a instance of the class <see cref="ConnStringCreatorOfWMS"/>
		/// </summary>
		/// <returns>returns  a instance of the class <see cref="ConnStringCreatorOfWMS"/></returns>
		public static ConnStringCreatorOfWMS Create()
		{
			return new ConnStringCreatorOfWMS();
		}

		#region Nested type: WMSConnStringRowMapper

		private class WMSConnStringRowMapper : IDataTableRowMapper<ConnStringOfWMS>
		{
			#region IDataTableRowMapper<ConnStringOfWMS> Members

			public ConnStringOfWMS MapRow(DataRow dataRow)
			{
				var connString = new ConnStringOfWMS();

				connString.ConnectionString = DES.Decrypt3DES(dataRow["ConnectionString"].ToString());
				connString.DatabaseName = dataRow["DatabaseName"].ToString();
				connString.DatabaseType = (DatabaseType) Enum.Parse(typeof (DatabaseType), dataRow["DbType"].ToString());
				connString.EnumValue = dataRow["EnumValue"].ToString();
				connString.Name = connString.EnumValue + connString.DatabaseType;
				connString.ProviderName = "System.Data.SqlClient";
				connString.WarehouseId = dataRow["WarehouseId"].ToString();
				connString.WebCookieValue = dataRow["WebCookieValue"].ToString();

				return connString;
			}

			#endregion
		}

		#endregion
	}
}