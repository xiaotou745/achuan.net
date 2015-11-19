using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;
using AC.Code.IBuilder;

namespace AC.Code.JavaBuilder
{
    /// <summary>
    /// Dao 代码生成基类
    /// </summary>
    public class BuilderDao : IBuilderDao
    {
        #region 构造函数

        public BuilderDao()
        {
        }

        public BuilderDao(IDbObject idbobj, string dbname, string tablename,
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

        private string GenerateDomainHead()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("package " + CodeName.DaoNamespace + ";");
            strclass.AppendLine();

            strclass.AppendLine("import java.util.List;");
            strclass.AppendLine("import java.util.HashMap;");
            strclass.AppendLine("import java.util.Map;");
            strclass.AppendLine("import org.springframework.stereotype.Repository;");
            var sss = new StringBuilder();
            string[] strings = CodeName.DaoNamespace.Split(new string[] {"."}, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < strings.Length-2; i++)
            {
                sss.AppendFormat("{0}.", strings[i]);
            }
            var namespaces = sss.ToString().Substring(0, sss.Length - 1);
            strclass.AppendLine("import " + namespaces + ".common.DaoBase;");
            strclass.AppendLine(string.Format("import {0};", CodeName.ServiceDTOFullName));
            strclass.AppendLine(string.Format("import {0};", CodeName.DomainFullName));
            strclass.AppendLine();

            strclass.AppendLine("/**");
            strclass.AppendLine(" * 数据访问对象 " + CodeName.DaoName + "");
            strclass.AppendLine(" * @author " + GenerateConfig.Author);
            strclass.AppendLine(" * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendLine(" *");
            strclass.AppendLine(" */");
            strclass.AppendLine("@Repository");

            strclass.AppendLine("public class " + CodeName.DaoName + " extends DaoBase implements " + CodeName.DomainName + " {");
            return strclass.ToString();
        }
        
        public string GetDaoCode()
        {
            var strclass = new StringPlus();
            strclass.Append(GenerateDomainHead());

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
            if (GenerateConfig.GetByIdMethodNeeded)
            {
                strclass.AppendLine(GenerateGetByIdCode());
            }
            if (GenerateConfig.QueryMethodNeeded)
            {
                strclass.AppendLine(GenerateQueryCode());
            }

            #endregion

            strclass.AppendLine("}");

            return strclass.ToString();
        }

        public string GenerateInsertCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * 新增一条记录");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, " * @param " + CodeName.CodeOfDTOParam + " 要新增的对象");
            if (IsHasIdentity)
            {
                strclass.AppendSpaceLine(1, " * @return  返回新增对象的自增Id");
            }
            strclass.AppendSpaceLine(1, " */");

            string returnType = "void";
            if (IsHasIdentity)
            {
                returnType = IdentityType;
            }
            strclass.AppendSpaceLine(1, "@Override");
            strclass.AppendSpaceLine(1, "public " + returnType + " insert(" + CodeName.CodeOfMethodDTOParam + ") {");
            if (returnType == "void")
            {
                strclass.AppendSpaceLine(2,
                    "getMasterSqlSessionUtil().insert(\"" + CodeName.DomainFullName + ".insert\", " +
                    CodeName.CodeOfDTOParam + ");");
            }
            else
            {
                strclass.AppendSpaceLine(2,
                    "return getMasterSqlSessionUtil().insert(\"" + CodeName.DomainFullName + ".insert\", " +
                    CodeName.CodeOfDTOParam + ");");
            }
            strclass.AppendSpaceLine(1, "}");

            return strclass.ToString();
        }

        public string GenerateUpdateCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * 更新一条记录");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, " * @param " + CodeName.CodeOfDTOParam + " 要更改的对象");
            strclass.AppendSpaceLine(1, " */");

            strclass.AppendSpaceLine(1, "@Override");
            strclass.AppendSpaceLine(1, "public void update(" + CodeName.CodeOfMethodDTOParam + ") {");
            strclass.AppendSpaceLine(2,
                    "getMasterSqlSessionUtil().update(\"" + CodeName.DomainFullName + ".update\", " +
                    CodeName.CodeOfDTOParam + ");");
            strclass.AppendSpaceLine(1, "}");

            return strclass.ToString();
        }

        public string GenerateDeleteCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * 删除一条记录");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            foreach (var key in Keys)
            {
                strclass.AppendSpaceLine(1,
                    " * @param " + CodeCommon.SetFirstCharacterLower(key.ColumnName) + " " + key.DeText);
            }
            strclass.AppendSpaceLine(1, " */");

            strclass.AppendSpaceLine(1, "@Override");
            strclass.AppendSpaceLine(1, "public void delete(" + CodeCommon.GetInParameterOfJava(Keys) + ") {");
            strclass.AppendSpaceLine(2,
                    "getMasterSqlSessionUtil().delete(\"" + CodeName.DomainFullName + ".delete\", " +
                    CodeCommon.GetFieldStringListWithFirstLower(Keys) + ");");
            strclass.AppendSpaceLine(1, "}");

            return strclass.ToString();
        }

        public string GenerateQueryCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * 查询方法");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, " * @param " + CodeName.CodeOfQueryDTOParam + " 查询条件");
            strclass.AppendSpaceLine(1, " */");
            strclass.AppendSpaceLine(1, "@Override");
            strclass.AppendSpaceLine(1, "public List<" + CodeName.ServiceDTOName + "> select(" + CodeName.CodeOfQueryMethodDTOParam + ") {");
            strclass.AppendSpaceLine(1, "}");

            return strclass.ToString();
        }

        public string GenerateGetByIdCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * 根据Id得到一个对象实体");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            foreach (var key in Keys)
            {
                strclass.AppendSpaceLine(1,
                    " * @param " + CodeCommon.SetFirstCharacterLower(key.ColumnName) + " " + key.DeText);
            }
            strclass.AppendSpaceLine(1, " */");

            strclass.AppendSpaceLine(1, "@Override");
            strclass.AppendSpaceLine(1, "public "+CodeName.ServiceDTOName + " getById(" + CodeCommon.GetInParameterOfJava(Keys) + ") {");
            strclass.AppendSpaceLine(2,
                    "return getMasterSqlSessionUtil().selectOne(\"" + CodeName.DomainFullName + ".getById\", " +
                    CodeCommon.GetFieldStringListWithFirstLower(Keys) + ");");
            strclass.AppendSpaceLine(1, "}");

            return strclass.ToString();
        }

        #endregion

    }
}