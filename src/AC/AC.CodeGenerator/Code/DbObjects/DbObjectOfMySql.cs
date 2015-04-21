using System;
using System.Collections.Generic;
using System.Data;
using AC.Code.Helper;

namespace AC.Code.DbObjects
{
    public class DbObjectOfMySql : IDbObject
    {
        public DbType DbType
        {
            get { return DbType.MySql; }
        }

        public string DbName { get; set; }
        public string TableName { get; set; }
        public string ServerName { get; set; }
        public string DbConnectStr { get; set; }

        public int ExecuteSql(string dbName, string sqlString)
        {
            throw new NotImplementedException();
        }

        public DataSet Query(string dbName, string sqlString)
        {
            throw new NotImplementedException();
        }

        public void ReSetConnString(bool sspi, string ip, string user, string pwd)
        {
            throw new NotImplementedException();
        }

        public List<string> GetDBList()
        {
            string sql = @"select SCHEMA_NAME from information_schema.SCHEMATA;";

            throw new NotImplementedException();
        }

        public List<string> GetTables(string dbName)
        {
            string sql = @"select TABLE_NAME,TABLE_COMMENT
from information_schema.`TABLES`
where TABLE_SCHEMA='superman'
	and TABLE_TYPE='base table';";
            throw new NotImplementedException();
        }

        public DataTable GetVIEWs(string dbName)
        {
            throw new NotImplementedException();
        }

        public DataTable GetTabViews(string dbName)
        {
            throw new NotImplementedException();
        }

        public DataTable GetProcs(string dbName)
        {
            throw new NotImplementedException();
        }

        public List<TableInfo> GetTablesInfo(string DbName)
        {
            throw new NotImplementedException();
        }

        public List<TableInfo> GetVIEWsInfo(string dbName)
        {
            throw new NotImplementedException();
        }

        public List<TableInfo> GetTabViewsInfo(string dbName)
        {
            throw new NotImplementedException();
        }

        public List<TableInfo> GetProcInfo(string dbName)
        {
            throw new NotImplementedException();
        }

        public string GetObjectInfo(string dbName, string objName)
        {
            throw new NotImplementedException();
        }

        public List<ColumnInfo> GetColumnList(string dbName, string tableName)
        {
            string sql = @"select * 
from information_schema.columns 
where table_name='account'
	and TABLE_SCHEMA='superman'";
            throw new NotImplementedException();
        }

        public List<ColumnInfo> GetColumnInfoList(string dbName, string tableName)
        {
            throw new NotImplementedException();
        }

        public DataTable GetKeyName(string dbName, string tableName)
        {
            throw new NotImplementedException();
        }

        public DataTable GetTabData(string dbName, string tableName)
        {
            throw new NotImplementedException();
        }

        public bool RenameTable(string dbName, string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteTable(string dbName, string tableName)
        {
            throw new NotImplementedException();
        }

        public string GetVersion()
        {
            throw new NotImplementedException();
        }


        public bool UpdateProperty(string dbName, string tableName, string columnName, string desc)
        {
            throw new NotImplementedException();
        }
    }
}