using System;
using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;
using AC.Code.IBuilder;

namespace AC.Code.Builder
{
    /// <summary>
    /// Dao代码生成器工厂类
    /// </summary>
    public class DaoBuilder
    {
        private List<ColumnInfo> colFields;
        private string dbName;
        private IDbObject dbObject;
        private CodeGenerateConfig generateConfig;
        private List<ColumnInfo> keys;
        private string tableName;

        #region Get

        public IBuilderDao GetDaoBuilder()
        {
            if (generateConfig == null)
            {
                return null;
            }
            switch (generateConfig.DaoStyle)
            {
                case DaoStyle.DbHelper:
                    return new BuilderDaoOfDbHelper(dbObject, dbName, tableName, colFields, keys, generateConfig);
                case DaoStyle.SqlHelper:
                    return new BuilderDaoOfSqlHelper(dbObject, dbName, tableName, colFields, keys, generateConfig);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Create

        private DaoBuilder()
        {
        }

        public static DaoBuilder Create()
        {
            var serviceBuilder = new DaoBuilder();
            return serviceBuilder;
        }

        #endregion

        #region Set

        public DaoBuilder SetGenerateConfig(CodeGenerateConfig codeGenerateConfig)
        {
            generateConfig = codeGenerateConfig;
            return this;
        }

        public DaoBuilder SetDbObj(IDbObject idbObj)
        {
            dbObject = idbObj;
            return this;
        }

        public DaoBuilder SetDbName(string dbname)
        {
            dbName = dbname;
            return this;
        }

        public DaoBuilder SetTableName(string tablename)
        {
            tableName = tablename;
            return this;
        }

        public DaoBuilder SetColFields(List<ColumnInfo> fields)
        {
            colFields = fields;
            return this;
        }

        public DaoBuilder SetKeys(List<ColumnInfo> colKeys)
        {
            keys = colKeys;
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Dao 代码生成基类
    /// </summary>
    public abstract class BuilderDaoBase : IBuilderDao
    {
        #region 构造函数

        protected BuilderDaoBase()
        {
        }

        protected BuilderDaoBase(IDbObject idbobj, string dbname, string tablename,
                                 List<ColumnInfo> fieldlist, List<ColumnInfo> keys, CodeGenerateConfig generateConfig)
        {
            DbObject = idbobj;
            DbName = dbname;
            TableName = tablename;
            Fieldlist = fieldlist;
            Keys = keys;
            GenerateConfig = generateConfig;
            CodeName = GenerateConfig.CodeName;
            if (Keys.Count > 0)
            {
                foreach (ColumnInfo key in Keys)
                {
                    if (key.IsIdentity)
                    {
                        IsHasIdentity = true;
                        IdentityType = CodeCommon.DbTypeToCS(key.TypeName);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 主键或条件字段中是否有标识列
        /// </summary>
        protected bool IsHasIdentity { get; set; }

        /// <summary>
        /// 自增长标识列类型
        /// </summary>
        protected string IdentityType { get; set; }

        /// <summary>
        /// 命名规范
        /// </summary>
        protected CodeNameBase CodeName { get; private set; }

        #region IBuilderDao Members

        public IDbObject DbObject { get; set; }
        public string DbName { get; set; }
        public string TableName { get; set; }
        public CodeGenerateConfig GenerateConfig { get; set; }
        public List<ColumnInfo> Fieldlist { get; set; }
        public List<ColumnInfo> Keys { get; set; }

        #endregion

        #region 方法

        public abstract string GetDaoCode();

        public abstract string GenerateInsertCode();

        public abstract string GenerateUpdateCode();

        public abstract string GenerateDeleteCode();

        public abstract string GenerateQueryCode();

        public abstract string GenerateGetByIdCode();

        #endregion

        protected bool IsSqlServer()
        {
            return (DbObject.DbType == DbType.SQL2000 || DbObject.DbType == DbType.SQL2005 ||
                    DbObject.DbType == DbType.SQL2008);
        }

        /// <summary>
        /// 得到Where条件语句 - Parameter方式 (例如：用于Exists  Delete  GetModel 的where)
        /// </summary>
        /// <param name="paramKeys"></param>
        /// <returns></returns>
        protected string GetWhereExpression(List<ColumnInfo> paramKeys)
        {
            var strclass = new StringPlus();
            foreach (ColumnInfo key in paramKeys)
            {
                strclass.Append(key.ColumnName + "=@" + key.ColumnName + " and ");
            }
            strclass.DelLastChar("and");
            return strclass.Value;
        }
    }

    /// <summary>
    /// DbHelper Dao方法生成类
    /// </summary>
    public class BuilderDaoOfDbHelper : BuilderDaoBase
    {
        #region 构造函数

        public BuilderDaoOfDbHelper()
        {
        }

        public BuilderDaoOfDbHelper(IDbObject idbobj, string dbname, string tablename,
                                    List<ColumnInfo> fieldlist, List<ColumnInfo> keys, CodeGenerateConfig generateConfig)
            : base(idbobj, dbname, tablename, fieldlist, keys, generateConfig)
        {
        }

        #endregion

        private string ConnectionString
        {
            get { return "ConnStringOfBBS"; }
        }

        public override string GetDaoCode()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("#region 引用");
            strclass.AppendLine("using System;");
            strclass.AppendLine("using System.Data;");
            strclass.AppendLine("using System.Text;");
            strclass.AppendLine("using System.Collections.Generic;");
            if (GenerateConfig.CallStyle == CallStyle.SpringNew)
            {
                strclass.AppendLine("using DZ.SpringUtils;");
            }
            if (GenerateConfig.CodeLayer == CodeLayer.ServiceLayerWithDomain)
            {
                strclass.AppendLine("using " + CodeName.DomainNamespace + ";");
            }
            strclass.AppendLine("using DZ.Data;");
            strclass.AppendLine("using DZ.Data.ConnString;");
            strclass.AppendLine("using DZ.Data.Core;");
            strclass.AppendLine("using DZ.Data.Generic;");
            strclass.AppendLine("using " + CodeName.ServiceDTONamespace + ";");
            strclass.AppendLine("#endregion");
            strclass.AppendLine();

            strclass.AppendLine("namespace " + CodeName.DaoNamespace);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// 数据访问类" + CodeName.DaoName + "。");
            strclass.AppendSpaceLine(1, "/// Generate By: " + Environment.UserName);
            strclass.AppendSpaceLine(1, "/// Generate Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, "/// </summary>");
            if (GenerateConfig.CallStyle == CallStyle.SpringNew)
            {
                strclass.AppendSpaceLine(1, "[Spring]");
            }
            strclass.AppendSpace(1, "public class " + CodeName.DaoName + " : Daobase"
                                    + (GenerateConfig.CodeLayer ==
                                       CodeLayer.ServiceLayerWithDomain
                                           ? ", " + CodeName.DomainName
                                           : string.Empty));
            strclass.AppendSpaceLine(1, "{");
            strclass.AppendSpaceLine(2, "public " + CodeName.DaoName + "()");
            strclass.AppendSpaceLine(2, "{}");
            strclass.AppendSpaceLine(2, "#region " + CodeName.DomainName + "  Members");

            #region  方法代码

            if (GenerateConfig.CreateMethodNeeded)
            {
                strclass.AppendLine(GenerateInsertCode());
            }
            if (GenerateConfig.ModifyMethodNeeded)
            {
                strclass.AppendLine(GenerateUpdateCode());
            }
            if (GenerateConfig.RemoveMethodNeeded)
            {
                strclass.AppendLine(GenerateDeleteCode());
            }
            if (GenerateConfig.QueryMethodNeeded)
            {
                strclass.AppendLine(GenerateQueryCode());
            }
            if (GenerateConfig.GetByIdMethodNeeded)
            {
                strclass.AppendLine(GenerateGetByIdCode());
            }

            #endregion

            strclass.AppendSpaceLine(2, "#endregion");
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "#region  Nested type: " + TableName + "RowMapper");
            strclass.AppendLine(GenerateRowMapperCode());
            strclass.AppendSpaceLine(2, "#endregion");
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "#region  Other Members");
            if (GenerateConfig.QueryMethodNeeded)
            {
                strclass.AppendLine(GenerateBindQueryCriteriaCode());
            }

            strclass.AppendSpaceLine(2, "#endregion");
            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("}");

            return strclass.ToString();
        }

        public override string GenerateInsertCode()
        {
            var strclass = new StringPlus();
            var strInsertFields = new StringPlus();
            var strInsertValues = new StringPlus();
            var strInsertParameters = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 增加一条记录");
            strclass.AppendSpaceLine(2, "/// </summary>");
            string strretu = "void";
            if (IsHasIdentity)
            {
                strretu = IdentityType;
            }

            strInsertParameters.AppendSpaceLine(3, "IDbParameters dbParameters = DbHelper.CreateDbParameters();");
            foreach (ColumnInfo field in Fieldlist)
            {
                if (field.IsIdentity)
                {
                    continue;
                }
                string columnName = field.ColumnName;
                strInsertFields.Append(columnName + ",");
                strInsertValues.Append("@" + columnName + ",");
                strInsertParameters.AppendSpaceLine(3,
                                                    "dbParameters.AddWithValue(\"" + columnName + "\", " +
                                                    CodeName.CodeOfDTOParam + "." +
                                                    columnName + ");");
            }
            //去掉最后的逗号
            strInsertFields.DelLastComma();
            strInsertValues.DelLastComma();

            //方法定义头
            strclass.AppendSpaceLine(2, "public " + strretu + " Insert(" + CodeName.CodeOfMethodDTOParam + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "const string INSERT_SQL = @\"");

            strclass.AppendLine("insert into " + TableName + "(" + strInsertFields + ")");
            strclass.AppendLine("values(" + strInsertValues + ")");
            if (IsSqlServer() && (IsHasIdentity))
            {
                strclass.AppendLine();
                strclass.AppendLine("select @@IDENTITY\";");
            }
            else
            {
                strclass.AppendLine("\";");
            }
            strclass.AppendLine();
            strclass.AppendLine(strInsertParameters.Value);
            strclass.AppendLine();

            //重新定义方法头
            if (IsSqlServer() && (IsHasIdentity))
            {
                strclass.AppendSpaceLine(3,
                                         "object result = DbHelper.ExecuteScalar(ConnStringName, INSERT_SQL, dbParameters);");
                strclass.AppendSpaceLine(3, "if (result == null)");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "return 0;");
                strclass.AppendSpaceLine(3, "}");
                strclass.AppendSpaceLine(3, "return " + IdentityType + ".Parse(result.ToString());");
            }
            else
            {
                strclass.AppendSpaceLine(3,
                                         "DbHelper.ExecuteNonQuery(" + "ConnStringName" +
                                         ", INSERT_SQL, dbParameters);");
            }
            strclass.AppendSpace(2, "}");
            return strclass.ToString();
        }


        /// <summary>
        /// 生成sql语句中的参数列表(例如：用于Add  Exists  Update Delete  GetModel 的参数传入)
        /// </summary>
        /// <param name="paramKeys"></param>
        /// <returns></returns>
        public string GetPreParameter(List<ColumnInfo> paramKeys)
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(3, "IDbParameters dbParameters = DbHelper.CreateDbParameters();");

            //strclass.AppendSpaceLine(3, "SqlParameter[] parameters = {");
            foreach (ColumnInfo key in paramKeys)
            {
                strclass.AppendSpaceLine(3, "dbParameters.AddWithValue(\"" + key.ColumnName + "\", " +
                                            CodeCommon.SetFirstCharacterLower(key.ColumnName) + ");");
            }
            strclass.AppendLine();
            return strclass.Value;
        }

        public override string GenerateUpdateCode()
        {
            var strclass = new StringPlus();
            var strSetValue = new StringPlus();
            var strParameters = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 更新一条记录");
            strclass.AppendSpaceLine(2, "/// </summary>");
            //方法定义头
            string strFun = CodeCommon.Space(2) + "public void Update(" + CodeName.CodeOfMethodDTOParam + ")";
            strclass.AppendLine(strFun);
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "const string UPDATE_SQL = @\"");
            strclass.AppendLine("update  " + TableName);
            strParameters.AppendSpaceLine(3, "IDbParameters dbParameters = DbHelper.CreateDbParameters();");
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                strParameters.AppendSpaceLine(3,
                                              "dbParameters.AddWithValue(\"" + columnName + "\", " +
                                              CodeName.CodeOfDTOParam + "." +
                                              columnName + ");");
                if (field.IsIdentity || field.IsPK)
                {
                    continue;
                }
                strSetValue.Append(columnName + "=@" + columnName + ",");
            }

            //去掉最后的逗号
            strSetValue.DelLastComma();
            strclass.AppendLine("set  " + strSetValue.Value);
            strclass.AppendLine("where  " + GetWhereExpression(Keys) + "\";");
            strclass.AppendLine();
            strclass.AppendLine(strParameters.Value);
            strclass.AppendLine();

            strclass.AppendSpaceLine(3, "DbHelper.ExecuteNonQuery(ConnStringName,  UPDATE_SQL, dbParameters);");
            strclass.AppendSpace(2, "}");
            return strclass.ToString();
        }

        public override string GenerateDeleteCode()
        {
            var strclass = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 删除一条记录");
            strclass.AppendSpaceLine(2, "/// </summary>");
            //方法定义头
            string strFun = CodeCommon.Space(2) + "public void Delete(" + CodeCommon.GetInParameter(Keys) + ")";
            strclass.AppendLine(strFun);
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3,
                                     "const string DELETE_SQL = @\"delete from " + TableName + " where " +
                                     GetWhereExpression(Keys) + "\";");
            strclass.AppendLine();

            strclass.AppendLine(GetPreParameter(Keys));

            strclass.AppendLine();

            strclass.AppendSpaceLine(3,
                                     "DbHelper.ExecuteNonQuery(" + ConnectionString +
                                     ", DELETE_SQL, dbParameters);");
            strclass.AppendSpace(2, "}");
            return strclass.ToString();
        }

        public override string GenerateQueryCode()
        {
            var strclass = new StringPlus();
            var strGetFields = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 查询对象");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2,
                                     "public IList<" + CodeName.ServiceDTOName + "> Query(" +
                                     CodeName.CodeOfQueryMethodDTOParam + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3,
                                     "string condition = BindQueryCriteria(" + CodeName.CodeOfDTOParam +
                                     ");");
            strclass.AppendSpaceLine(3, "string QUERY_SQL = @\"");

            foreach (ColumnInfo field in Fieldlist)
            {
                strGetFields.Append(field.ColumnName + ",");
            }

            strGetFields.DelLastComma();
            strclass.AppendLine("select  " + strGetFields.Value);
            strclass.Append("from  " + TableName + " (nolock)\" + condition;");
            strclass.AppendLine();

            strclass.AppendSpaceLine(3,
                                     "return DbHelper.QueryWithRowMapper(" + ConnectionString + ",QUERY_SQL,new " +
                                     TableName +
                                     "RowMapper());");

            strclass.AppendSpaceLine(2, "}");

            return strclass.ToString();
        }

        public override string GenerateGetByIdCode()
        {
            var strclass = new StringPlus();
            var strGetFields = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 根据ID获取对象");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2,
                                     "public " + CodeName.ServiceDTOName + " GetById(" +
                                     CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "const string GETBYID_SQL = @\"");

            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                strGetFields.Append(columnName + ",");
            }

            strGetFields.DelLastComma();
            strclass.AppendLine("select  " + strGetFields.Value);
            strclass.AppendLine("from  " + TableName + " (nolock)");
            strclass.AppendLine("where  " + GetWhereExpression(Keys) + "\";");
            strclass.AppendLine();
            strclass.AppendLine(GetPreParameter(Keys));
            strclass.AppendSpaceLine(3,
                                     "return DbHelper.QueryForObject(" + ConnectionString + ", GETBYID_SQL, dbParameters, new " +
                                     TableName +
                                     "RowMapper());");

            strclass.AppendSpaceLine(2, "}");

            return strclass.ToString();
        }

        protected string GenerateBindQueryCriteriaCode()
        {
            var strclass = new StringPlus();

            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 构造查询条件");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2,
                                     "public static string BindQueryCriteria(" +
                                     CodeName.CodeOfQueryMethodDTOParam + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "var stringBuilder = new StringBuilder(\" where 1=1 \");");
            strclass.AppendSpaceLine(3, "if (" + CodeName.CodeOfQueryDTOParam + " == null)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return stringBuilder.ToString();");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendLine();
            strclass.AppendSpaceLine(3, "//TODO:在此加入查询条件构建代码");
            strclass.AppendLine();
            strclass.AppendSpaceLine(3, "return stringBuilder.ToString();");
            strclass.AppendSpaceLine(2, "}");

            return strclass.ToString();
        }

        protected string GenerateRowMapperCode()
        {
            var strclass = new StringPlus();

            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 绑定对象");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "private class " + TableName
                                        + "RowMapper : IDataTableRowMapper<" + CodeName.ServiceDTOName + ">");
            strclass.AppendSpaceLine(2, "{");

            strclass.AppendSpaceLine(3, "public " + CodeName.ServiceDTOName + " MapRow(DataRow dataReader)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "var result = new " + CodeName.ServiceDTOName + "();");
            strclass.AppendSpaceLine(4, "object obj;");

            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                switch (CodeCommon.DbTypeToCS(columnType))
                {
                    case "int":
                    case "long":
                    case "decimal":
                    case "float":
                    case "DateTime":
                        {
                            strclass.AppendSpaceLine(4, "obj = dataReader[\"" + columnName + "\"];");

                            strclass.AppendSpaceLine(4, "if (obj != null && obj != DBNull.Value)");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5,
                                                     "result." + columnName + " = " + CodeCommon.DbTypeToCS(columnType) +
                                                     ".Parse(obj.ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "string":
                        {
                            strclass.AppendSpaceLine(4,
                                                     "result." + columnName + " = dataReader[\"" + columnName +
                                                     "\"].ToString();");
                        }
                        break;
                    case "bool":
                        {
                            strclass.AppendSpaceLine(4, "obj = dataReader[\"" + columnName + "\"];");

                            strclass.AppendSpaceLine(4, "if (obj != null && obj != DBNull.Value)");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5,
                                                     "if(obj.ToString()==1.ToString() || obj.ToString().ToLower()==\"true\")");

                            strclass.AppendSpaceLine(5, "{");
                            strclass.AppendSpaceLine(6, "result." + columnName + "=true;");
                            strclass.AppendSpaceLine(5, "}");
                            strclass.AppendSpaceLine(5, "else");
                            strclass.AppendSpaceLine(5, "{");
                            strclass.AppendSpaceLine(6, "result." + columnName + "=false;");
                            strclass.AppendSpaceLine(5, "}");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "byte[]":
                        {
                            strclass.AppendSpaceLine(4, "obj = dataReader[\"" + columnName + "\"];");

                            strclass.AppendSpaceLine(4, "if (obj != null && obj != DBNull.Value)");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "result." + columnName + "=(byte[])obj;");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "Guid":
                        {
                            strclass.AppendSpaceLine(4, "obj = dataReader[\"" + columnName + "\"];");

                            strclass.AppendSpaceLine(4, "if (obj != null && obj != DBNull.Value)");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "result." + columnName + "=new Guid(obj.ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    default:
                        strclass.AppendSpaceLine(5,
                                                 "//result." + columnName + "=dataReader[\"" + columnName +
                                                 "\"].ToString();");
                        break;
                }
            }

            strclass.AppendLine();
            strclass.AppendSpaceLine(4, "return result;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");

            return strclass.ToString();
        }
    }

    /// <summary>
    /// SqlHelper Dao方法生成类
    /// </summary>
    public class BuilderDaoOfSqlHelper : BuilderDaoBase
    {
        #region 构造函数

        public BuilderDaoOfSqlHelper()
        {
        }

        public BuilderDaoOfSqlHelper(IDbObject idbobj, string dbname, string tablename,
                                     List<ColumnInfo> fieldlist, List<ColumnInfo> keys,
                                     CodeGenerateConfig generateConfig)
            : base(idbobj, dbname, tablename, fieldlist, keys, generateConfig)
        {
        }

        #endregion

        private string ConnectionString
        {
            get { return "ConnStringName"; }
        }

        /// <summary>
        /// 生成sql语句中的参数列表(例如：用于Add  Exists  Update Delete  GetModel 的参数传入)
        /// </summary>
        /// <param name="paramKeys"></param>
        /// <returns></returns>
        public string GetPreParameter(List<ColumnInfo> paramKeys)
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(3, "SqlParameter[] parameters = {");
            foreach (ColumnInfo key in paramKeys)
            {
                strclass.AppendSpaceLine(5,
                                         "new SqlParameter(\"@" + "" + key.ColumnName + "\", SqlDbType." +
                                         CodeCommon.DbTypeLength(DbObject.DbType.ToString(), key.TypeName, "") +
                                         ") {Value =" +
                                         CodeCommon.SetFirstCharacterLower(key.ColumnName) + "},");
            }
            strclass.DelLastComma();
            strclass.AppendLine();
            strclass.AppendSpaceLine(3, "};");
            return strclass.Value;
        }

        public override string GetDaoCode()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("#region 引用");
            strclass.AppendLine("using System;");
            strclass.AppendLine("using System.Data;");
            strclass.AppendLine("using System.Data.SqlClient;");
            strclass.AppendLine("using System.Text;");
            strclass.AppendLine("using System.Collections.Generic;");
            strclass.AppendLine("using Microsoft.ApplicationBlocks.Data;");
            if (GenerateConfig.CallStyle == CallStyle.SpringNew)
            {
                strclass.AppendLine("using DZ.SpringUtils;");
                strclass.AppendLine("using " + CodeName.DomainNamespace + ";");
            }
            strclass.AppendLine("using " + CodeName.ServiceDTONamespace + ";");
            strclass.AppendLine("#endregion");
            strclass.AppendLine();

            strclass.AppendLine("namespace " + CodeName.DaoNamespace);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// 数据访问类" + CodeName.DaoName + "。");
            strclass.AppendSpaceLine(1, "/// Generate By: " + Environment.UserName);
            strclass.AppendSpaceLine(1, "/// Generate Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, "/// </summary>");
            if (GenerateConfig.CallStyle == CallStyle.SpringNew)
            {
                strclass.AppendSpaceLine(1, "[Spring]");
            }

            strclass.AppendSpace(1, "public class " + CodeName.DaoName + " : Daobase"
                                    + (GenerateConfig.CodeLayer ==
                                       CodeLayer.ServiceLayerWithDomain
                                           ? ", " + CodeName.DomainName
                                           : string.Empty));
            strclass.AppendSpaceLine(1, "{");
            strclass.AppendSpaceLine(2, "public " + CodeName.DaoName + "()");
            strclass.AppendSpaceLine(2, "{}");
            strclass.AppendSpaceLine(2, "#region " + CodeName.DomainName + "  Members");

            #region  方法代码

            if (GenerateConfig.CreateMethodNeeded)
            {
                strclass.AppendLine(GenerateInsertCode());
            }
            if (GenerateConfig.ModifyMethodNeeded)
            {
                strclass.AppendLine(GenerateUpdateCode());
            }
            if (GenerateConfig.RemoveMethodNeeded)
            {
                strclass.AppendLine(GenerateDeleteCode());
            }
            if (GenerateConfig.QueryMethodNeeded)
            {
                strclass.AppendLine(GenerateQueryCode());
            }
            if (GenerateConfig.GetByIdMethodNeeded)
            {
                strclass.AppendLine(GenerateGetByIdCode());
            }

            #endregion

            strclass.AppendSpaceLine(2, "#endregion");
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "#region  Other Members");
            if (GenerateConfig.QueryMethodNeeded)
            {
                strclass.AppendLine(GenerateBindQueryCriteriaCode());
            }
            if (GenerateConfig.QueryMethodNeeded || GenerateConfig.GetByIdMethodNeeded)
            {
                strclass.AppendLine(GenerateReaderBindCode());
            }

            strclass.AppendSpaceLine(2, "#endregion");
            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("}");

            return strclass.ToString();
        }

        public override string GenerateInsertCode()
        {
            var strclass = new StringPlus();
            var strInsertFields = new StringPlus();
            var strInsertValues = new StringPlus();
            var strInsertParameters = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 增加一条记录");
            strclass.AppendSpaceLine(2, "/// </summary>");
            string strretu = "void";
            if (IsHasIdentity)
            {
                strretu = IdentityType;
            }

            
            //方法定义头
            string strFun = CodeCommon.Space(2) + "public " + strretu + " Insert(" + CodeName.CodeOfMethodDTOParam + ")";
            strclass.AppendLine(strFun);
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "const string INSERT_SQL = @\"");
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                string length = field.Length;
                if (field.IsIdentity)
                {
                    continue;
                }
                strInsertFields.Append(columnName + ",");
                strInsertValues.Append("@" + columnName + ",");
                strInsertParameters.AppendSpaceLine(5,
                                                    "new SqlParameter(\"@" + columnName + "\",SqlDbType." +
                                                    CodeCommon.DbTypeLength(DbObject.DbType.ToString(), columnType,
                                                                            length) + ") {Value = " +
                                                    CodeName.CodeOfDTOParam + "." + columnName + "},");
            }

            //去掉最后的逗号
            strInsertFields.DelLastComma();
            strInsertValues.DelLastComma();
            strInsertParameters.DelLastComma();

            strclass.AppendLine("insert into " + TableName + "(" + strInsertFields + ")");
            strclass.Append("values(" + strInsertValues + ")");
            if (IsSqlServer() && (IsHasIdentity))
            {
                strclass.AppendLine();
                strclass.AppendLine("select @@IDENTITY\";");
            }
            else
            {
                strclass.AppendLine("\";");
            }
            strclass.AppendLine();
            strclass.AppendSpaceLine(3, "SqlParameter[] parameters = {");
            strclass.AppendLine(strInsertParameters.Value);
            strclass.AppendSpaceLine(3, "};");
            strclass.AppendLine();

            //重新定义方法头
            if (IsSqlServer() && (IsHasIdentity))
            {
                strclass.AppendSpaceLine(3,
                                         "object result = SqlHelper.ExecuteScalar(" + ConnectionString +
                                         ", CommandType.Text, INSERT_SQL, parameters);");
                strclass.AppendSpaceLine(3, "if (result == null)");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "return 0;");
                strclass.AppendSpaceLine(3, "}");
                strclass.AppendSpaceLine(3, "else");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "return " + IdentityType + ".Parse(obj.ToString());");
                strclass.AppendSpaceLine(3, "}");
            }
            else
            {
                strclass.AppendSpaceLine(3,
                                         "SqlHelper.ExecuteNonQuery(" + ConnectionString +
                                         ", CommandType.Text, INSERT_SQL, parameters);");
            }
            strclass.AppendSpace(2, "}");
            return strclass.ToString();
        }

        public override string GenerateUpdateCode()
        {
            var strclass = new StringPlus();
            var strSetValue = new StringPlus();
            var strParameters = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 更新一条记录");
            strclass.AppendSpaceLine(2, "/// </summary>");
            //方法定义头
            string strFun = CodeCommon.Space(2) + "public void Update(" + CodeName.CodeOfMethodDTOParam + ")";
            strclass.AppendLine(strFun);
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "const string UPDATE_SQL = @\"");
            strclass.AppendLine("update  " + TableName);
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                string length = field.Length;
                strParameters.AppendSpaceLine(5,
                                              "new SqlParameter(\"@" + columnName + "\",SqlDbType." +
                                              CodeCommon.DbTypeLength(DbObject.DbType.ToString(), columnType, length) +
                                              ") {Value = " +
                                              CodeName.CodeOfDTOParam + "." + columnName + "},");
                if (field.IsIdentity || field.IsPK)
                {
                    continue;
                }
                strSetValue.Append(columnName + "=@" + columnName + ",");
            }

            //去掉最后的逗号
            strSetValue.DelLastComma();
            strParameters.DelLastComma();
            strclass.AppendLine("set  " + strSetValue.Value);
            strclass.AppendLine("where  " + GetWhereExpression(Keys) + "\";");
            strclass.AppendLine();
            strclass.AppendSpaceLine(3, "SqlParameter[] parameters = {");
            strclass.AppendLine(strParameters.Value);
            strclass.AppendSpaceLine(3, "};");
            strclass.AppendLine();

            strclass.AppendSpaceLine(3,
                                     "SqlHelper.ExecuteNonQuery(" + ConnectionString +
                                     ", CommandType.Text, UPDATE_SQL, parameters);");
            strclass.AppendSpace(2, "}");
            return strclass.ToString();
        }

        public override string GenerateDeleteCode()
        {
            var strclass = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 删除一条记录");
            strclass.AppendSpaceLine(2, "/// </summary>");
            //方法定义头
            string strFun = CodeCommon.Space(2) + "public void Delete(" + CodeCommon.GetInParameter(Keys) + ")";
            strclass.AppendLine(strFun);
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3,
                                     "const string DELETE_SQL = @\"delete from " + TableName + " where " +
                                     GetWhereExpression(Keys) + "\";");
            strclass.AppendLine();

            strclass.AppendLine(GetPreParameter(Keys));

            strclass.AppendLine();

            strclass.AppendSpaceLine(3,
                                     "SqlHelper.ExecuteNonQuery(" + ConnectionString +
                                     ", CommandType.Text, DELETE_SQL, parameters);");
            strclass.AppendSpace(2, "}");
            return strclass.ToString();
        }

        public override string GenerateQueryCode()
        {
            var strclass = new StringPlus();
            var strGetFields = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 查询对象");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2,
                                     "public IList<" + CodeName.ServiceDTOName + "> Query(" +
                                     CodeName.CodeOfQueryMethodDTOParam + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3,
                                     "string condition = BindQueryCriteria(" + CodeName.CodeOfQueryMethodDTOParam +
                                     ");");
            strclass.AppendSpaceLine(3, "string QUERY_SQL = @\"");

            foreach (ColumnInfo field in Fieldlist)
            {
                strGetFields.Append(field.ColumnName + ",");
            }

            strGetFields.DelLastComma();
            strclass.AppendLine("select  " + strGetFields.Value);
            strclass.Append("from  " + TableName + " (nolock)\" + condition;");
            strclass.AppendLine();
            strclass.AppendSpaceLine(3,
                                     "IList<" + CodeName.ServiceDTOName + "> result = new List<" +
                                     CodeName.ServiceDTOName + ">();");
            strclass.AppendLine();
            strclass.AppendSpaceLine(3,
                                     "DataSet dataset = SqlHelper.ExecuteDataset(" + ConnectionString +
                                     ", CommandType.Text, QUERY_SQL);");

            strclass.AppendSpaceLine(3, "if (dataset == null || dataset.Tables.Count == 0)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return result;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendLine();
            strclass.AppendSpaceLine(3, "foreach (DataRow dataRow in dataset.Tables[0].Rows)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "result.Add(ReaderBind(dataRow));");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendLine();
            strclass.AppendSpaceLine(3, "return result;");
            strclass.AppendSpaceLine(2, "}");

            return strclass.ToString();
        }

        public override string GenerateGetByIdCode()
        {
            var strclass = new StringPlus();
            var strGetFields = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 根据ID获取对象");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2,
                                     "public " + CodeName.ServiceDTOName + " GetById(" +
                                     CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "const string GETBYID_SQL = @\"");

            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                strGetFields.Append(columnName + ",");
            }

            strGetFields.DelLastComma();
            strclass.AppendLine("select  " + strGetFields.Value);
            strclass.AppendLine("from  " + TableName + " (nolock)");
            strclass.AppendLine("where  " + GetWhereExpression(Keys) + "\";");
            strclass.AppendLine();
            strclass.AppendLine(GetPreParameter(Keys));
            strclass.AppendSpaceLine(3, CodeName.ServiceDTOName + " result = null;");
            strclass.AppendLine();
            strclass.AppendSpaceLine(3,
                                     "DataSet dataset = SqlHelper.ExecuteDataset(" + ConnectionString +
                                     ", CommandType.Text, GETBYID_SQL, parameters);");

            strclass.AppendSpaceLine(3, "if (dataset != null && dataset.Tables[0].Rows.Count > 0)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "result = ReaderBind(dataset.Tables[0].Rows[0]);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendLine();
            strclass.AppendSpaceLine(3, "return result;");
            strclass.AppendSpaceLine(2, "}");

            return strclass.ToString();
        }

        public string GenerateBindQueryCriteriaCode()
        {
            var strclass = new StringPlus();

            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 构造查询条件");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2,
                                     "public static string BindQueryCriteria(" +
                                     CodeName.CodeOfQueryMethodDTOParam + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "var stringBuilder = new StringBuilder(\" where 1=1 \");");
            strclass.AppendSpaceLine(3, "if (" + CodeName.CodeOfQueryDTOParam + " == null)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return stringBuilder.ToString();");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendLine();
            strclass.AppendSpaceLine(3, "//TODO:在此加入查询条件构建代码");
            strclass.AppendLine();
            strclass.AppendSpaceLine(3, "return stringBuilder.ToString();");
            strclass.AppendSpaceLine(2, "}");

            return strclass.ToString();
        }

        public string GenerateReaderBindCode()
        {
            var strclass = new StringPlus();

            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 绑定对象");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public static " + CodeName.ServiceDTOName + " ReaderBind(DataRow dataReader)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "var result = new " + CodeName.ServiceDTOName + "();");
            strclass.AppendSpaceLine(3, "object obj;");

            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                switch (CodeCommon.DbTypeToCS(columnType))
                {
                    case "int":
                    case "long":
                    case "decimal":
                    case "float":
                    case "DateTime":
                        {
                            strclass.AppendSpaceLine(3, "obj = dataReader[\"" + columnName + "\"];");

                            strclass.AppendSpaceLine(3, "if (obj != null && obj != DBNull.Value)");
                            strclass.AppendSpaceLine(3, "{");
                            strclass.AppendSpaceLine(4,
                                                     "result." + columnName + " = " + CodeCommon.DbTypeToCS(columnType) +
                                                     ".Parse(obj.ToString());");
                            strclass.AppendSpaceLine(3, "}");
                        }
                        break;
                    case "string":
                        {
                            strclass.AppendSpaceLine(3,
                                                     "result." + columnName + " = dataReader[\"" + columnName +
                                                     "\"].ToString();");
                        }
                        break;
                    case "bool":
                        {
                            strclass.AppendSpaceLine(3, "obj = dataReader[\"" + columnName + "\"];");

                            strclass.AppendSpaceLine(3, "if (obj != null && obj != DBNull.Value)");
                            strclass.AppendSpaceLine(3, "{");
                            strclass.AppendSpaceLine(4,
                                                     "if(obj.ToString()==1.ToString() || obj.ToString().ToLower()==\"true\")");

                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "result." + columnName + "=true;");
                            strclass.AppendSpaceLine(4, "}");
                            strclass.AppendSpaceLine(4, "else");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "result." + columnName + "=false;");
                            strclass.AppendSpaceLine(4, "}");
                            strclass.AppendSpaceLine(3, "}");
                        }
                        break;
                    case "byte[]":
                        {
                            strclass.AppendSpaceLine(3, "obj = dataReader[\"" + columnName + "\"];");

                            strclass.AppendSpaceLine(3, "if (obj != null && obj != DBNull.Value)");
                            strclass.AppendSpaceLine(3, "{");
                            strclass.AppendSpaceLine(4, "result." + columnName + "=(byte[])obj;");
                            strclass.AppendSpaceLine(3, "}");
                        }
                        break;
                    case "Guid":
                        {
                            strclass.AppendSpaceLine(3, "obj = dataReader[\"" + columnName + "\"];");

                            strclass.AppendSpaceLine(3, "if (obj != null && obj != DBNull.Value)");
                            strclass.AppendSpaceLine(3, "{");
                            strclass.AppendSpaceLine(5, "result." + columnName + "=new Guid(obj.ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    default:
                        strclass.AppendSpaceLine(4,
                                                 "//result." + columnName + "=dataReader[\"" + columnName +
                                                 "\"].ToString();");
                        break;
                }
            }

            strclass.AppendLine();
            strclass.AppendSpaceLine(3, "return result;");
            strclass.AppendSpaceLine(2, "}");

            return strclass.ToString();
        }
    }
}