using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using AC.Code.Helper;

namespace AC.Code.DbObjects
{
    /// <summary>
    /// 数据库信息类。
    /// </summary>
    public class DbObject : IDbObject
    {
        #region  属性

        private readonly SqlConnection connect = new SqlConnection();
        private string dbconnectStr;
        private bool isdbosp;

        public DbType DbType
        {
            get { return DbType.SQL2005; }
        }

        public string DbName { get; set; }
        public string TableName { get; set; }
        public string ServerName { get; set; }

        public string DbConnectStr
        {
            set { dbconnectStr = value; }
            get { return dbconnectStr; }
        }
        public void ReSetConnString(bool sspi, string ip, string user, string pwd)
        {
            if (sspi)
            {
                dbconnectStr = "Integrated Security=SSPI;Data Source=" + ip + ";Initial Catalog=master";
                //constr="Provider=SQLOLEDB;Data Source="+ip+";Initial Catalog=master;Integrated Security=SSPI";//这是OleDbConnection连接的字符串
            }
            else
            {
                if (string.IsNullOrEmpty(pwd))
                {
                    dbconnectStr = "user id=" + user + ";initial catalog=master;data source=" + ip +
                                   ";Connect Timeout=30";
                }
                else
                {
                    dbconnectStr = "user id=" + user + ";password=" + pwd + ";initial catalog=master;data source=" + ip +
                                   ";Connect Timeout=30";
                }
            }
        }
        #endregion

        #region 构造函数，构造基本信息

        public DbObject()
        {
            IsDboSp();
        }

        /// <summary>
        /// 构造一个数据库连接
        /// </summary>
        /// <param name="connecionString">连接字符串</param>
        public DbObject(string connecionString)
        {
            dbconnectStr = connecionString;
            connect.ConnectionString = connecionString;
        }

        /// <summary>
        /// 构造一个连接字符串
        /// </summary>
        /// <param name="sspi">是否windows集成认证</param>
        /// <param name="ip">服务器IP</param>
        /// <param name="user">用户名</param>
        /// <param name="pwd">密码</param>
        public DbObject(bool sspi, string ip, string user, string pwd)
        {
            connect = new SqlConnection();
            if (sspi)
            {
                dbconnectStr = "Integrated Security=SSPI;Data Source=" + ip + ";Initial Catalog=master";
                //constr="Provider=SQLOLEDB;Data Source="+ip+";Initial Catalog=master;Integrated Security=SSPI";//这是OleDbConnection连接的字符串
            }
            else
            {
                if (string.IsNullOrEmpty(pwd))
                {
                    dbconnectStr = "user id=" + user + ";initial catalog=master;data source=" + ip +
                                   ";Connect Timeout=30";
                }
                else
                {
                    dbconnectStr = "user id=" + user + ";password=" + pwd + ";initial catalog=master;data source=" + ip +
                                   ";Connect Timeout=30";
                }
            }
            connect.ConnectionString = dbconnectStr;
        }

        #endregion

        #region  是否采用sp(存储过程)的方式获取数据结构信息

        /// <summary>
        /// 是否采用sp的方式获取数据结构信息
        /// </summary>
        /// <returns></returns>
        private void IsDboSp()
        {
            string isDboSp = ConfigurationManager.AppSettings["IsDboSp"];
            if (!string.IsNullOrEmpty(isDboSp))
            {
                if (isDboSp.Trim() == "1")
                {
                    isdbosp = true;
                }
            }
        }

        #endregion

        #region 打开数据库 OpenDB(string DbName)

        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="dbName">要打开的数据库</param>
        /// <returns></returns>
        private SqlCommand OpenDB(string dbName)
        {
            try
            {
                if (string.IsNullOrEmpty(connect.ConnectionString))
                {
                    connect.ConnectionString = dbconnectStr;
                }
                if (connect.ConnectionString != dbconnectStr)
                {
                    connect.Close();
                    connect.ConnectionString = dbconnectStr;
                }
                var dbCommand = new SqlCommand {Connection = connect};
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }
                dbCommand.CommandText = "use [" + dbName + "]";
                dbCommand.ExecuteNonQuery();
                return dbCommand;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region ADO.NET 操作

        public int ExecuteSql(string dbName, string sqlString)
        {
            SqlCommand dbCommand = OpenDB(dbName);
            dbCommand.CommandText = sqlString;
            int rows = dbCommand.ExecuteNonQuery();
            return rows;
        }

        public DataSet Query(string dbName, string sqlString)
        {
            var ds = new DataSet();
            OpenDB(dbName);
            var command = new SqlDataAdapter(sqlString, connect);
            command.Fill(ds, "ds");
            return ds;
        }

        public SqlDataReader ExecuteReader(string dbName, string strSQL)
        {
            OpenDB(dbName);
            var cmd = new SqlCommand(strSQL, connect);
            SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return myReader;
        }

        public object GetSingle(string dbName, string sqlString)
        {
            try
            {
                SqlCommand dbCommand = OpenDB(dbName);
                dbCommand.CommandText = sqlString;
                object obj = dbCommand.ExecuteScalar();
                if ((Equals(obj, null)) || (Equals(obj, DBNull.Value)))
                {
                    return null;
                }
                return obj;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="dbName"> </param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedure(string dbName, string storedProcName, IDataParameter[] parameters, string tableName)
        {
            OpenDB(dbName);
            var dataSet = new DataSet();
            var sqlDa = new SqlDataAdapter {SelectCommand = BuildQueryCommand(connect, storedProcName, parameters)};
            sqlDa.Fill(dataSet, tableName);

            return dataSet;
        }

        private SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName,
                                             IEnumerable<IDataParameter> parameters)
        {
            var command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput ||
                         parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        #endregion

        #region 得到数据库的名字列表 GetDBList()

        /// <summary>
        /// 得到数据库的名字列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetDBList()
        {
            var dblist = new List<string>();
            const string strSql = "select name from sysdatabases order by name";
            //return Query("master",strSql).Tables[0];
            SqlDataReader reader = ExecuteReader("master", strSql);
            while (reader.Read())
            {
                dblist.Add(reader.GetString(0));
            }
            reader.Close();
            return dblist;
        }

        #endregion

        #region  得到数据库的所有表和视图 的名字

        /// <summary>
        /// 得到数据库的所有表名
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        public List<string> GetTables(string dbName)
        {
            if (isdbosp)
            {
                return GetTablesSP(dbName);
            }
            const string strSql =
                "select [name] from sysobjects where xtype='U'and [name]<>'dtproperties' order by [name]";
            //order by id
            //return Query(DbName,strSql).Tables[0];
            var tabNames = new List<string>();
            SqlDataReader reader = ExecuteReader(dbName, strSql);
            while (reader.Read())
            {
                tabNames.Add(reader.GetString(0));
            }
            reader.Close();
            return tabNames;
        }

        /// <summary>
        /// 得到数据库的所有视图名
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        public DataTable GetVIEWs(string dbName)
        {
            if (isdbosp)
            {
                return GetViewsSP(dbName);
            }
            const string strSql =
                "select [name] from sysobjects where xtype='V' and [name]<>'syssegments' and [name]<>'sysconstraints' order by [name]";
            //order by id
            return Query(dbName, strSql).Tables[0];
        }

        /// <summary>
        /// 得到数据库的所有表和视图名
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        public DataTable GetTabViews(string dbName)
        {
            if (isdbosp)
            {
                return GetTabViewsSP(dbName);
            }
            const string strSql = @"
select  [name], sysobjects.xtype type
from    sysobjects
where   ( xtype = 'U'
          or xtype = 'V'
          or xtype = 'P'
        )
        and [name] <> 'dtproperties'
        and [name] <> 'syssegments'
        and [name] <> 'sysconstraints'
order by xtype, [name]";

            return Query(dbName, strSql).Tables[0];
        }

        /// <summary>
        /// 得到数据库的所有存储过程名
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        public DataTable GetProcs(string dbName)
        {
            const string strSql =
                "select [name] from sysobjects where xtype='P'and [name]<>'dtproperties' order by [name]";
            //order by id
            return Query(dbName, strSql).Tables[0];
        }

        public List<string> GetTablesSP(string dbName)
        {
            IDataParameter[] parameters =
                {
                    new SqlParameter("@table_name", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_owner", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_qualifier", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_type", SqlDbType.VarChar, 100)
                };
            parameters[0].Value = null;
            parameters[1].Value = null;
            parameters[2].Value = null;
            parameters[3].Value = "'TABLE'";

            DataSet ds = RunProcedure(dbName, "sp_tables", parameters, "ds");
            var tabNames = new List<string>();
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                //dt.Columns["TABLE_QUALIFIER"].ColumnName = "db";
                //dt.Columns["TABLE_OWNER"].ColumnName = "cuser";
                //dt.Columns["TABLE_NAME"].ColumnName = "name";
                //dt.Columns["TABLE_TYPE"].ColumnName = "type";
                //dt.Columns["REMARKS"].ColumnName = "remarks";
                for (int n = 0; n < dt.Rows.Count; n++)
                {
                    tabNames.Add(dt.Rows[n]["TABLE_NAME"].ToString());
                }
                return tabNames;
            }
            return null;
        }

        /// <summary>
        /// 得到数据库的所有视图名
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        public DataTable GetViewsSP(string dbName)
        {
            IDataParameter[] parameters =
                {
                    new SqlParameter("@table_name", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_owner", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_qualifier", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_type", SqlDbType.VarChar, 100)
                };
            parameters[0].Value = null;
            parameters[1].Value = null;
            parameters[2].Value = null;
            parameters[3].Value = "'VIEW'";

            DataSet ds = RunProcedure(dbName, "sp_tables", parameters, "ds");
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                dt.Columns["TABLE_QUALIFIER"].ColumnName = "db";
                dt.Columns["TABLE_OWNER"].ColumnName = "cuser";
                dt.Columns["TABLE_NAME"].ColumnName = "name";
                dt.Columns["TABLE_TYPE"].ColumnName = "type";
                dt.Columns["REMARKS"].ColumnName = "remarks";
                return dt;
            }
            return null;
        }

        /// <summary>
        /// 得到数据库的所有表和视图名
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        public DataTable GetTabViewsSP(string dbName)
        {
            IDataParameter[] parameters =
                {
                    new SqlParameter("@table_name", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_owner", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_qualifier", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_type", SqlDbType.VarChar, 100)
                };
            parameters[0].Value = null;
            parameters[1].Value = null;
            parameters[2].Value = null;
            parameters[3].Value = "'TABLE','VIEW'";

            DataSet ds = RunProcedure(dbName, "sp_tables", parameters, "ds");
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                dt.Columns["TABLE_QUALIFIER"].ColumnName = "db";
                dt.Columns["TABLE_OWNER"].ColumnName = "cuser";
                dt.Columns["TABLE_NAME"].ColumnName = "name";
                dt.Columns["TABLE_TYPE"].ColumnName = "type";
                dt.Columns["REMARKS"].ColumnName = "remarks";
                return dt;
            }
            return null;
        }

        #endregion

        #region  得到数据库的所有表和视图 的列表信息 

        /// <summary>
        /// 得到数据库的所有表的详细信息
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        public List<TableInfo> GetTablesInfo(string dbName)
        {
            const string strSql = @"
select  sysobjects.[name] name, sysusers.name cuser, sysobjects.xtype type,
        sysobjects.crdate dates
from    sysobjects ,
        sysusers
where   sysusers.uid = sysobjects.uid
        and sysobjects.xtype = 'U'
        and sysobjects.[name] <> 'dtproperties'
order by sysobjects.[name] ";
            //strSql.Append("order by sysobjects.id");
            //return Query(DbName,strSql.ToString()).Tables[0];

            var tablist = new List<TableInfo>();
            SqlDataReader reader = ExecuteReader(dbName, strSql);
            while (reader.Read())
            {
                var tab = new TableInfo
                              {
                                  TabName = reader.GetString(0),
                                  TabDate = reader.GetValue(3).ToString(),
                                  TabType = reader.GetString(2),
                                  TabUser = reader.GetString(1)
                              };
                tablist.Add(tab);
            }
            reader.Close();
            return tablist;
        }

        /// <summary>
        /// 得到数据库的所有视图的详细信息
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        public List<TableInfo> GetVIEWsInfo(string dbName)
        {
            const string strSql = @"
select  sysobjects.[name] name, sysusers.name cuser, sysobjects.xtype type,
        sysobjects.crdate dates
from    sysobjects ,
        sysusers
where   sysusers.uid = sysobjects.uid
        and sysobjects.xtype = 'V'
        and sysobjects.[name] <> 'syssegments'
        and sysobjects.[name] <> 'sysconstraints'
order by sysobjects.[name] ";

            var tablist = new List<TableInfo>();
            SqlDataReader reader = ExecuteReader(dbName, strSql);
            while (reader.Read())
            {
                var tab = new TableInfo
                              {
                                  TabName = reader.GetString(0),
                                  TabDate = reader.GetValue(3).ToString(),
                                  TabType = reader.GetString(2),
                                  TabUser = reader.GetString(1)
                              };
                tablist.Add(tab);
            }
            reader.Close();
            return tablist;
        }

        /// <summary>
        /// 得到数据库的所有表和视图的详细信息
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        public List<TableInfo> GetTabViewsInfo(string dbName)
        {
            const string strSql = @"
select  sysobjects.[name] name, sysusers.name cuser, sysobjects.xtype type,
        sysobjects.crdate dates
from    sysobjects ,
        sysusers
where   sysusers.uid = sysobjects.uid
        and ( sysobjects.xtype = 'U'
              or sysobjects.xtype = 'V'
              or sysobjects.xtype = 'P'
            )
        and sysobjects.[name] <> 'dtproperties'
        and sysobjects.[name] <> 'syssegments'
        and sysobjects.[name] <> 'sysconstraints'
order by sysobjects.xtype, sysobjects.[name] ";

            var tablist = new List<TableInfo>();
            SqlDataReader reader = ExecuteReader(dbName, strSql);
            while (reader.Read())
            {
                var tab = new TableInfo
                              {
                                  TabName = reader.GetString(0),
                                  TabDate = reader.GetValue(3).ToString(),
                                  TabType = reader.GetString(2),
                                  TabUser = reader.GetString(1)
                              };
                tablist.Add(tab);
            }
            reader.Close();
            return tablist;
        }

        /// <summary>
        /// 得到数据库的所有存储过程的详细信息
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <returns></returns>
        public List<TableInfo> GetProcInfo(string dbName)
        {
            const string strSql = @"
select  sysobjects.[name] name, sysusers.name cuser, sysobjects.xtype type,
        sysobjects.crdate dates
from    sysobjects ,
        sysusers
where   sysusers.uid = sysobjects.uid
        and sysobjects.xtype = 'P' 
        //and  sysobjects.[name]<>'dtproperties' 
        //order by sysobjects.id
order by sysobjects.[name] ";

            var tablist = new List<TableInfo>();
            SqlDataReader reader = ExecuteReader(dbName, strSql);
            while (reader.Read())
            {
                var tab = new TableInfo
                              {
                                  TabName = reader.GetString(0),
                                  TabDate = reader.GetValue(3).ToString(),
                                  TabType = reader.GetString(2),
                                  TabUser = reader.GetString(1)
                              };
                tablist.Add(tab);
            }
            reader.Close();
            return tablist;
        }

        #endregion

        #region 得到对象定义语句

        /// <summary>
        /// 得到视图或存储过程的定义语句
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <param name="objName"> </param>
        /// <returns></returns>
        public string GetObjectInfo(string dbName, string objName)
        {
            string strSql = @"select  b.text
from    sysobjects a ,
        syscomments b
where   a.xtype = 'p'
        and a.id = b.id
        and a.name = '" + objName + "'";
            return GetSingle(dbName, strSql).ToString();
        }

        #endregion

        #region 得到(快速)数据库里表的列名和类型 GetColumnList(string DbName,string TableName)

        /// <summary>
        /// 得到数据库里表或视图的列名和类型
        /// </summary>
        /// <param name="dbName">库</param>
        /// <param name="tableName">表</param>
        /// <returns></returns>
        public List<ColumnInfo> GetColumnList(string dbName, string tableName)
        {
            try
            {
                if (isdbosp)
                {
                    return GetColumnListSP(dbName, tableName);
                }
                string strSql = @"
select  a.colorder as colorder, a.name as ColumnName, b.name as TypeName
from    syscolumns a ,
        systypes b ,
        sysobjects c
where   a.xtype = b.xusertype
        and a.id = c.id
        and c.name = '" + tableName + "'"
                                + "order by a.colorder";

                //return Query(DbName,strSql.ToString()).Tables[0];
                var colexist = new ArrayList(); //是否已经存在
                var collist = new List<ColumnInfo>();
                SqlDataReader reader = ExecuteReader(dbName, strSql);
                while (reader.Read())
                {
                    var col = new ColumnInfo
                                  {
                                      Colorder = reader.GetValue(0).ToString(),
                                      ColumnName = reader.GetString(1),
                                      TypeName = reader.GetString(2),
                                      Length = "",
                                      Preci = "",
                                      Scale = "",
                                      IsPK = false,
                                      cisNull = false,
                                      DefaultVal = "",
                                      IsIdentity = false,
                                      DeText = ""
                                  };
                    if (!colexist.Contains(col.ColumnName))
                    {
                        collist.Add(col);
                        colexist.Add(col.ColumnName);
                    }
                }
                reader.Close();
                return collist;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ColumnInfo> GetColumnListSP(string dbName, string tableName)
        {
            IDataParameter[] parameters =
                {
                    new SqlParameter("@table_name", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_owner", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_qualifier", SqlDbType.NVarChar, 384),
                    new SqlParameter("@column_name", SqlDbType.VarChar, 100)
                };
            parameters[0].Value = tableName;
            parameters[1].Value = null;
            parameters[2].Value = null;
            parameters[3].Value = null;

            DataSet ds = RunProcedure(dbName, "sp_columns", parameters, "ds");
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                int r = dt.Rows.Count;
                DataTable dtkey = CreateColumnTable();
                for (int m = 0; m < r; m++)
                {
                    DataRow nrow = dtkey.NewRow();
                    nrow["colorder"] = dt.Rows[m]["ORDINAL_POSITION"];
                    nrow["ColumnName"] = dt.Rows[m]["COLUMN_NAME"];
                    string tn = dt.Rows[m]["TYPE_NAME"].ToString().Trim();
                    nrow["TypeName"] = (tn == "int identity") ? "int" : tn;
                    nrow["Length"] = dt.Rows[m]["LENGTH"];
                    nrow["Preci"] = dt.Rows[m]["PRECISION"];
                    nrow["Scale"] = dt.Rows[m]["SCALE"];
                    nrow["IsIdentity"] = (tn == "int identity") ? "√" : "";
                    nrow["isPK"] = "";
                    nrow["cisNull"] = (dt.Rows[m]["NULLABLE"].ToString().Trim() == "1") ? "√" : "";
                    nrow["defaultVal"] = dt.Rows[m]["COLUMN_DEF"];
                    nrow["deText"] = dt.Rows[m]["REMARKS"];
                    dtkey.Rows.Add(nrow);
                }
                return CodeCommon.GetColumnInfos(dtkey);
            }
            return null;
        }

        #endregion

        #region 得到表的列的详细信息 GetColumnInfoList(string DbName,string TableName)

        /// <summary>
        /// 得到数据库里表或视图的列的详细信息
        /// </summary>
        /// <param name="dbName">库</param>
        /// <param name="tableName">表</param>
        /// <returns></returns>
        public List<ColumnInfo> GetColumnInfoList(string dbName, string tableName)
        {
            if (isdbosp)
            {
                return GetColumnInfoListSP(dbName, tableName);
            }
            string strSql = @"
select  colorder = C.column_id, ColumnName = C.name, TypeName = T.name, 
            --//Length=C.max_length, 
        Length = case when T.name = 'nchar' then C.max_length / 2
                      when T.name = 'nvarchar' then C.max_length / 2
                      else C.max_length
                 end, Preci = C.precision, Scale = C.scale,
        IsIdentity = case when C.is_identity = 1 then N'√'
                          else N''
                     end, isPK = isnull(IDX.PrimaryKey, N''),
        Computed = case when C.is_computed = 1 then N'√'
                        else N''
                   end, IndexName = isnull(IDX.IndexName, N''),
        IndexSort = isnull(IDX.Sort, N''), Create_Date = O.Create_Date,
        Modify_Date = O.Modify_date,
        cisNull = case when C.is_nullable = 1 then N'√'
                       else N''
                  end, defaultVal = isnull(D.definition, N''),
        deText = isnull(PFD.[value], N'')
from    sys.columns C
        inner join sys.objects O on C.[object_id] = O.[object_id]
                                    and ( O.type = 'U'
                                          or O.type = 'V'
                                        )
                                    and O.is_ms_shipped = 0
        inner join sys.types T on C.user_type_id = T.user_type_id
        left join sys.default_constraints D on C.[object_id] = D.parent_object_id
                                               and C.column_id = D.parent_column_id
                                               and C.default_object_id = D.[object_id]
        left join sys.extended_properties PFD on PFD.class = 1
                                                 and C.[object_id] = PFD.major_id
                                                 and C.column_id = PFD.minor_id 
--//			--AND PFD.name='Caption'  -- 字段说明对应的描述名称(一个字段可以添加多个不同name的描述) 
        left join sys.extended_properties PTB on PTB.class = 1
                                                 and PTB.minor_id = 0
                                                 and C.[object_id] = PTB.major_id 
--//			-- AND PFD.name='Caption'  -- 表说明对应的描述名称(一个表可以添加多个不同name的描述)   
        left join --// -- 索引及主键信息
        ( select    IDXC.[object_id], IDXC.column_id,
                    Sort = case indexkey_property(IDXC.[object_id],
                                                  IDXC.index_id,
                                                  IDXC.index_column_id,
                                                  'IsDescending')
                             when 1 then 'DESC'
                             when 0 then 'ASC'
                             else ''
                           end,
                    PrimaryKey = case when IDX.is_primary_key = 1 then N'√'
                                      else N''
                                 end, IndexName = IDX.Name
          from      sys.indexes IDX
                    inner join sys.index_columns IDXC on IDX.[object_id] = IDXC.[object_id]
                                                         and IDX.index_id = IDXC.index_id
                    left join sys.key_constraints KC on IDX.[object_id] = KC.[parent_object_id]
                                                        and IDX.index_id = KC.unique_index_id
                    inner join --// 对于一个列包含多个索引的情况,只显示第1个索引信息
                    ( select    [object_id], Column_id,
                                index_id = min(index_id)
                      from      sys.index_columns
                      group by  [object_id], Column_id
                    ) IDXCUQ on IDXC.[object_id] = IDXCUQ.[object_id]
                                and IDXC.Column_id = IDXCUQ.Column_id
                                and IDXC.index_id = IDXCUQ.index_id
        ) IDX on C.[object_id] = IDX.[object_id]
                 and C.column_id = IDX.column_id
where   O.name = N'" + tableName + "'"
                            + "order by O.name, C.column_id  ";

            //return Query(DbName,strSql.ToString()).Tables[0];

            var colexist = new ArrayList(); //是否已经存在

            var collist = new List<ColumnInfo>();
            SqlDataReader reader = ExecuteReader(dbName, strSql);
            while (reader.Read())
            {
                var col = new ColumnInfo
                              {
                                  Colorder = reader.GetValue(0).ToString(),
                                  ColumnName = reader.GetString(1),
                                  TypeName = reader.GetString(2),
                                  Length = reader.GetValue(3).ToString(),
                                  Preci = reader.GetValue(4).ToString(),
                                  Scale = reader.GetValue(5).ToString(),
                                  IsIdentity = (reader.GetString(6) == "√") ? true : false,
                                  IsPK = (reader.GetString(7) == "√") ? true : false,
                                  cisNull = (reader.GetString(13) == "√") ? true : false,
                                  DefaultVal = reader.GetString(14),
                                  DeText = reader.GetString(15)
                              };
                if (!colexist.Contains(col.ColumnName))
                {
                    collist.Add(col);
                    colexist.Add(col.ColumnName);
                }
            }
            reader.Close();
            return collist;
        }

        public List<ColumnInfo> GetColumnInfoListSP(string dbName, string tableName)
        {
            IDataParameter[] parameters =
                {
                    new SqlParameter("@table_name", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_owner", SqlDbType.NVarChar, 384),
                    new SqlParameter("@table_qualifier", SqlDbType.NVarChar, 384),
                    new SqlParameter("@column_name", SqlDbType.VarChar, 100)
                };
            parameters[0].Value = tableName;
            parameters[1].Value = null;
            parameters[2].Value = null;
            parameters[3].Value = null;

            DataSet ds = RunProcedure(dbName, "sp_columns", parameters, "ds");
            int n = ds.Tables.Count;
            if (n > 0)
            {
                DataTable dt = ds.Tables[0];
                int r = dt.Rows.Count;
                DataTable dtkey = CreateColumnTable();
                for (int m = 0; m < r; m++)
                {
                    DataRow nrow = dtkey.NewRow();
                    nrow["colorder"] = dt.Rows[m]["ORDINAL_POSITION"];
                    nrow["ColumnName"] = dt.Rows[m]["COLUMN_NAME"];
                    string tn = dt.Rows[m]["TYPE_NAME"].ToString().Trim();
                    nrow["TypeName"] = (tn == "int identity") ? "int" : tn;
                    nrow["Length"] = dt.Rows[m]["LENGTH"];
                    nrow["Preci"] = dt.Rows[m]["PRECISION"];
                    nrow["Scale"] = dt.Rows[m]["SCALE"];
                    nrow["IsIdentity"] = (tn == "int identity") ? "√" : "";
                    nrow["isPK"] = "";
                    nrow["cisNull"] = (dt.Rows[m]["NULLABLE"].ToString().Trim() == "1") ? "√" : "";
                    nrow["defaultVal"] = dt.Rows[m]["COLUMN_DEF"];
                    nrow["deText"] = dt.Rows[m]["REMARKS"];
                    dtkey.Rows.Add(nrow);
                }
                return CodeCommon.GetColumnInfos(dtkey);
            }
            return null;
        }

        #endregion

        #region 得到数据库里表的主键 GetKeyName(string DbName,string TableName)

        //创建列信息表

        /// <summary>
        /// 得到数据库里表的主键
        /// </summary>
        /// <param name="dbName">库</param>
        /// <param name="tableName">表</param>
        /// <returns></returns>
        public DataTable GetKeyName(string dbName, string tableName)
        {
            DataTable dtkey = CreateColumnTable();
            //DataTable dt = GetColumnInfoList(DbName, TableName);

            List<ColumnInfo> collist = GetColumnInfoList(dbName, tableName);
            DataTable dt = CodeCommon.GetColumnInfoDt(collist);
            DataRow[] rows = dt.Select(" isPK='√' or IsIdentity='√' ");
            foreach (DataRow row in rows)
            {
                DataRow nrow = dtkey.NewRow();
                nrow["colorder"] = row["colorder"];
                nrow["ColumnName"] = row["ColumnName"];
                nrow["TypeName"] = row["TypeName"];
                nrow["Length"] = row["Length"];
                nrow["Preci"] = row["Preci"];
                nrow["Scale"] = row["Scale"];
                nrow["IsIdentity"] = row["IsIdentity"];
                nrow["isPK"] = row["isPK"];
                nrow["cisNull"] = row["cisNull"];
                nrow["defaultVal"] = row["defaultVal"];
                nrow["deText"] = row["deText"];
                dtkey.Rows.Add(nrow);
            }
            return dtkey;

            #region

            //StringBuilder strSql = new StringBuilder();
            //strSql.Append("select * from ");
            //strSql.Append("( ");

            //strSql.Append("SELECT ");
            //strSql.Append("colorder=C.column_id,");
            //strSql.Append("ColumnName=C.name,");
            //strSql.Append("TypeName=T.name, ");
            //strSql.Append("Length=C.max_length, ");
            //strSql.Append("Preci=C.precision, ");
            //strSql.Append("Scale=C.scale, ");
            //strSql.Append("IsIdentity=CASE WHEN C.is_identity=1 THEN N'√'ELSE N'' END,");
            //strSql.Append("isPK=ISNULL(IDX.PrimaryKey,N''),");

            //strSql.Append("Computed=CASE WHEN C.is_computed=1 THEN N'√'ELSE N'' END, ");
            //strSql.Append("IndexName=ISNULL(IDX.IndexName,N''), ");
            //strSql.Append("IndexSort=ISNULL(IDX.Sort,N''), ");
            //strSql.Append("Create_Date=O.Create_Date, ");
            //strSql.Append("Modify_Date=O.Modify_date, ");

            //strSql.Append("cisNull=CASE WHEN C.is_nullable=1 THEN N'√'ELSE N'' END, ");
            //strSql.Append("defaultVal=ISNULL(D.definition,N''), ");
            //strSql.Append("deText=ISNULL(PFD.[value],N'') ");

            //strSql.Append("FROM sys.columns C ");
            //strSql.Append("INNER JOIN sys.objects O ");
            //strSql.Append("ON C.[object_id]=O.[object_id] ");
            //strSql.Append("AND O.type='U' ");
            //strSql.Append("AND O.is_ms_shipped=0 ");
            //strSql.Append("INNER JOIN sys.types T ");
            //strSql.Append("ON C.user_type_id=T.user_type_id ");
            //strSql.Append("LEFT JOIN sys.default_constraints D ");
            //strSql.Append("ON C.[object_id]=D.parent_object_id ");
            //strSql.Append("AND C.column_id=D.parent_column_id ");
            //strSql.Append("AND C.default_object_id=D.[object_id] ");
            //strSql.Append("LEFT JOIN sys.extended_properties PFD ");
            //strSql.Append("ON PFD.class=1  ");
            //strSql.Append("AND C.[object_id]=PFD.major_id  ");
            //strSql.Append("AND C.column_id=PFD.minor_id ");
            ////strSql.Append("--AND PFD.name='Caption'  -- 字段说明对应的描述名称(一个字段可以添加多个不同name的描述) ");
            //strSql.Append("LEFT JOIN sys.extended_properties PTB ");
            //strSql.Append("ON PTB.class=1 ");
            //strSql.Append("AND PTB.minor_id=0  ");
            //strSql.Append("AND C.[object_id]=PTB.major_id ");
            ////strSql.Append("-- AND PFD.name='Caption'  -- 表说明对应的描述名称(一个表可以添加多个不同name的描述)   ");
            //strSql.Append("LEFT JOIN ");// -- 索引及主键信息
            //strSql.Append("( ");
            //strSql.Append("SELECT  ");
            //strSql.Append("IDXC.[object_id], ");
            //strSql.Append("IDXC.column_id, ");
            //strSql.Append("Sort=CASE INDEXKEY_PROPERTY(IDXC.[object_id],IDXC.index_id,IDXC.index_column_id,'IsDescending') ");
            //strSql.Append("WHEN 1 THEN 'DESC' WHEN 0 THEN 'ASC' ELSE '' END, ");
            //strSql.Append("PrimaryKey=CASE WHEN IDX.is_primary_key=1 THEN N'√'ELSE N'' END, ");
            //strSql.Append("IndexName=IDX.Name ");
            //strSql.Append("FROM sys.indexes IDX ");
            //strSql.Append("INNER JOIN sys.index_columns IDXC ");
            //strSql.Append("ON IDX.[object_id]=IDXC.[object_id] ");
            //strSql.Append("AND IDX.index_id=IDXC.index_id ");
            //strSql.Append("LEFT JOIN sys.key_constraints KC ");
            //strSql.Append("ON IDX.[object_id]=KC.[parent_object_id] ");
            //strSql.Append("AND IDX.index_id=KC.unique_index_id ");
            //strSql.Append("INNER JOIN  ");// 对于一个列包含多个索引的情况,只显示第1个索引信息
            //strSql.Append("( ");
            //strSql.Append("SELECT [object_id], Column_id, index_id=MIN(index_id) ");
            //strSql.Append("FROM sys.index_columns ");
            //strSql.Append("GROUP BY [object_id], Column_id ");
            //strSql.Append(") IDXCUQ ");
            //strSql.Append("ON IDXC.[object_id]=IDXCUQ.[object_id] ");
            //strSql.Append("AND IDXC.Column_id=IDXCUQ.Column_id ");
            //strSql.Append("AND IDXC.index_id=IDXCUQ.index_id ");
            //strSql.Append(") IDX ");
            //strSql.Append("ON C.[object_id]=IDX.[object_id] ");
            //strSql.Append("AND C.column_id=IDX.column_id  ");
            //strSql.Append("WHERE O.name=N'" + TableName + "' ");

            //strSql.Append(") Keyname ");
            //strSql.Append(" where isPK='√' or (IsIdentity='√' or colorder=1)");
            //return Query(DbName, strSql.ToString()).Tables[0];

            #endregion
        }

        public DataTable CreateColumnTable()
        {
            var table = new DataTable();

            var col = new DataColumn {DataType = Type.GetType("System.String"), ColumnName = "colorder"};
            table.Columns.Add(col);

            col = new DataColumn {DataType = Type.GetType("System.String"), ColumnName = "ColumnName"};
            table.Columns.Add(col);

            col = new DataColumn {DataType = Type.GetType("System.String"), ColumnName = "TypeName"};
            table.Columns.Add(col);

            col = new DataColumn {DataType = Type.GetType("System.String"), ColumnName = "Length"};
            table.Columns.Add(col);

            col = new DataColumn {DataType = Type.GetType("System.String"), ColumnName = "Preci"};
            table.Columns.Add(col);

            col = new DataColumn {DataType = Type.GetType("System.String"), ColumnName = "Scale"};
            table.Columns.Add(col);

            col = new DataColumn {DataType = Type.GetType("System.String"), ColumnName = "IsIdentity"};
            table.Columns.Add(col);

            col = new DataColumn {DataType = Type.GetType("System.String"), ColumnName = "isPK"};
            table.Columns.Add(col);

            col = new DataColumn {DataType = Type.GetType("System.String"), ColumnName = "cisNull"};
            table.Columns.Add(col);

            col = new DataColumn {DataType = Type.GetType("System.String"), ColumnName = "defaultVal"};
            table.Columns.Add(col);

            col = new DataColumn {DataType = Type.GetType("System.String"), ColumnName = "deText"};
            table.Columns.Add(col);

            return table;
        }

        #endregion

        #region 得到数据表里的数据 GetTabData(string DbName,string TableName)

        /// <summary>
        /// 得到数据表里的数据
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetTabData(string dbName, string tableName)
        {
            var strSql = new StringBuilder();
            strSql.Append("select * from [" + tableName + "]");
            return Query(dbName, strSql.ToString()).Tables[0];
        }

        /// <summary>
        /// 根据SQL查询得到数据表里的数据
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="strSQL"> </param>
        /// <returns></returns>
        public DataTable GetTabDataBySQL(string dbName, string strSQL)
        {
            return Query(dbName, strSQL).Tables[0];
        }

        #endregion

        #region 数据库属性操作

        /// <summary>
        /// 修改表名称
        /// </summary>
        /// <param name="dbName"> </param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool RenameTable(string dbName, string oldName, string newName)
        {
            try
            {
                SqlCommand dbCommand = OpenDB(dbName);
                dbCommand.CommandText = "EXEC sp_rename '" + oldName + "', '" + newName + "'";
                dbCommand.ExecuteNonQuery();
                return true;
            }
            catch //(System.Exception ex)
            {
                //string str=ex.Message;	
                return false;
            }
        }

        /// <summary>
        /// 删除表
        /// </summary>	
        public bool DeleteTable(string dbName, string tableName)
        {
            try
            {
                SqlCommand dbCommand = OpenDB(dbName);
                dbCommand.CommandText = "DROP TABLE [" + tableName + "]";
                dbCommand.ExecuteNonQuery();
                return true;
            }
            catch //(System.Exception ex)
            {
                //string str=ex.Message;	
                return false;
            }
        }

        /// <summary>
        /// 得到版本号
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            try
            {
                const string strSql = "execute master..sp_msgetversion "; //select @@version
                return Query("master", strSql).Tables[0].Rows[0][0].ToString();
            }
            catch //(System.Exception ex)
            {
                //string str=ex.Message;	
                return "";
            }
        }

        #endregion
    }
}