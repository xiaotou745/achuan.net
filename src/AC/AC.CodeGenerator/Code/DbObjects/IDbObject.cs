using System.Collections.Generic;
using System.Data;
using AC.Code.Helper;

namespace AC.Code.DbObjects
{
    /// <summary>
    /// 获取数据库信息类的接口定义。
    /// </summary>
    public interface IDbObject
    {
        /// <summary>
        ///  数据库类型
        /// </summary>
        DbType DbType { get; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        string DbName { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// 服务器名
        /// </summary>
        string ServerName { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string DbConnectStr { set; get; }

        #region db操作

        int ExecuteSql(string dbName, string sqlString);
        DataSet Query(string dbName, string sqlString);

        #endregion

        void ReSetConnString(bool sspi, string ip, string user, string pwd);
        #region 得到数据库的名字列表 GetDBList()

        /// <summary>
        /// 得到数据库的名字列表
        /// </summary>
        /// <returns></returns>
        List<string> GetDBList();

        #endregion

        #region  得到数据库的所有用户表名 GetTables(string DbName)

        /// <summary>
        /// 得到数据库的所有表名
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        List<string> GetTables(string dbName);

        DataTable GetVIEWs(string dbName);

        /// <summary>
        /// 得到数据库的所有表和视图名
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        DataTable GetTabViews(string dbName);

        DataTable GetProcs(string dbName);

        #endregion

        #region  得到数据库的所有表的详细信息 GetTablesInfo(string DbName)

        /// <summary>
        /// 得到数据库的所有表的详细信息
        /// </summary>
        /// <param name="DbName">数据库</param>
        /// <returns></returns>
        List<TableInfo> GetTablesInfo(string DbName);

        List<TableInfo> GetVIEWsInfo(string dbName);

        /// <summary>
        /// 得到数据库的所有表和视图的详细信息
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        List<TableInfo> GetTabViewsInfo(string dbName);

        List<TableInfo> GetProcInfo(string dbName);

        #endregion

        #region 得到对象定义语句

        /// <summary>
        /// 得到视图或存储过程的定义语句
        /// </summary>  
        string GetObjectInfo(string dbName, string objName);

        #endregion

        #region 得到(快速)数据库里表的列名和类型 GetColumnList(string DbName,string TableName)

        /// <summary>
        /// 得到数据库里表的列名和类型
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<ColumnInfo> GetColumnList(string dbName, string tableName);

        #endregion

        #region 得到数据库里表的列的详细信息 GetColumnInfoList(string DbName,string TableName)

        /// <summary>
        /// 得到数据库里表的列的详细信息
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<ColumnInfo> GetColumnInfoList(string dbName, string tableName);

        #endregion

        #region 得到数据库里表的主键 GetKeyName(string DbName,string TableName)

        /// <summary>
        /// 得到数据库里表的主键
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DataTable GetKeyName(string dbName, string tableName);

        #endregion

        #region 得到数据表里的数据 GetTabData(string DbName,string TableName)

        /// <summary>
        /// 得到数据表里的数据
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DataTable GetTabData(string dbName, string tableName);

        #endregion

        #region 数据库属性操作

        /// <summary>
        /// 修改数据库名称
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        bool RenameTable(string dbName, string oldName, string newName);

        /// <summary>
        /// 删除表
        /// </summary>	
        bool DeleteTable(string dbName, string tableName);

        string GetVersion();

        #endregion
    }
}