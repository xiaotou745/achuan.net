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
    public class BuilderMyBatisMapper : IBuilderMyBatisMapper
    {
        #region 构造函数

        public BuilderMyBatisMapper()
        {
        }

        public BuilderMyBatisMapper(IDbObject idbobj, string dbname, string tablename,
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
                        IdentityType = CodeCommon.DbTypeToJava(key.TypeName);
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

        private string GetJdbcType(string columnType)
        {
            switch (columnType.ToUpper())
            {
                case "INT":
                    return "INTEGER";
                case "DATE":
                case "DATETIME":
                    return "TIMESTAMP";
                default:
                    return columnType.ToUpper();
            }
        }
        private string GenerateMapperHead()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            strclass.AppendLine();

            strclass.AppendLine("<!DOCTYPE mapper PUBLIC \"-//mybatis.org//DTD Mapper 3.0//EN\" \"http://mybatis.org/dtd/mybatis-3-mapper.dtd\" >");
            strclass.AppendLine();
            strclass.AppendLine(String.Format("<mapper namespace=\"{0}\">", CodeName.DomainFullName));
            strclass.AppendLine();
            
            return strclass.ToString();
        }

        private string GenerateBaseResultMap()
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(1, String.Format("<resultMap type=\"{0}\" id=\"baseResultMap\">", CodeName.ServiceDTOFullName));
            foreach (ColumnInfo field in Fieldlist)
            {
                if (field.IsIdentity)
                {
                    strclass.AppendSpaceLine(2,
                        String.Format("<id property=\"{0}\" column=\"{1}\" jdbcType=\"{2}\"></id>",
                            CodeCommon.SetFirstCharacterLower(field.ColumnName), field.ColumnName,
                            GetJdbcType(field.TypeName)));
                    continue;
                }
                strclass.AppendSpaceLine(2,
                    string.Format("<result property=\"{0}\" column=\"{1}\" jdbcType=\"{2}\" />",
                        CodeCommon.SetFirstCharacterLower(field.ColumnName), field.ColumnName, GetJdbcType(field.TypeName)));
            }
            strclass.AppendSpaceLine(1, "</resultMap>");
            strclass.AppendLine();
            return strclass.ToString();
        }
        private string GenerateBaseColumnsSql()
        {
            var strclass = new StringPlus();
            var strFields = new StringPlus();
            string tableAlias = CodeCommon.GetUpperChars(TableName).ToLower();
            strclass.AppendSpaceLine(1, "<sql id=\"baseColumnList\">");
            foreach (ColumnInfo field in Fieldlist)
            {
                strFields.Append(tableAlias + "." + field.ColumnName + ",");
            }
            strFields.DelLastComma();
            strclass.AppendSpaceLine(2, strFields.ToString());
            strclass.AppendSpaceLine(1, "</sql>");
            strclass.AppendLine();
            return strclass.ToString();
        }
        private string GenerateInsertSql()
        {
            var strclass = new StringPlus();
            var strInsertFields = new StringPlus();
            var strInsertValues = new StringPlus();
            var strIndentity = new StringPlus();
            
            foreach (ColumnInfo field in Fieldlist)
            {
                if (field.IsIdentity)
                {
                    strIndentity.Append(string.Format("useGeneratedKeys=\"true\" keyProperty=\"{0}\"",
                        CodeCommon.SetFirstCharacterLower(field.ColumnName)));
                    continue;
                }
                string columnName = field.ColumnName;
                strInsertFields.Append(columnName + ",");
                strInsertValues.AppendSpaceLine(3, "#{" + CodeCommon.SetFirstCharacterLower(columnName) + ",jdbcType=" + GetJdbcType(field.TypeName) + "},");
            }
            strclass.AppendSpaceLine(1,string.Format("<insert id=\"insert\" parameterType=\"{0}\" {1}>", 
                CodeName.ServiceDTOFullName,
                string.IsNullOrEmpty(strIndentity.ToString())?"":strIndentity.ToString()));
            //去掉最后的逗号
            strInsertFields.DelLastComma();
            strInsertValues.DelLastComma();

            strclass.AppendSpaceLine(2, "insert into " + TableName + "(" + strInsertFields + ")");
            strclass.AppendSpaceLine(2, "values(");
            strclass.AppendLine(strInsertValues.ToString());
            strclass.AppendSpaceLine(2, ")");

            strclass.AppendSpaceLine(1, "</insert>");
            strclass.AppendLine();
            
            return strclass.ToString();
        }
        protected string GetWhereExpression(List<ColumnInfo> paramKeys)
        {
            var strclass = new StringPlus();
            foreach (ColumnInfo key in paramKeys)
            {
                strclass.Append(key.ColumnName + "=#{" + CodeCommon.SetFirstCharacterLower(key.ColumnName) + ",jdbcType=" + GetJdbcType(key.TypeName) + "} and ");
            }
            strclass.DelLastChar("and");
            return strclass.Value;
        }
        private string GenerateUpdateSql()
        {
            var strclass = new StringPlus();
            var strSetValue = new StringPlus();
            strclass.AppendSpaceLine(1,
                string.Format("<update id=\"update\" parameterType=\"{0}\">", CodeName.ServiceDTOFullName));

            int index = 0;
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                if (field.IsIdentity || field.IsPK)
                {
                    continue;
                }
                if (index == 0)
                {
                    strSetValue.AppendLine(columnName + "=#{" + CodeCommon.SetFirstCharacterLower(columnName) +
                                           ",jdbcType=" +
                                           GetJdbcType(field.TypeName) + "},");
                }
                else
                {
                    strSetValue.AppendSpaceLine(3, columnName + "=#{" + CodeCommon.SetFirstCharacterLower(columnName) + ",jdbcType=" +
                                       GetJdbcType(field.TypeName) + "},");
                }
                index++;
            }

            //去掉最后的逗号
            strSetValue.DelLastComma();

            strclass.AppendSpaceLine(2, "update " + TableName);
            strclass.AppendSpaceLine(2, "set " + strSetValue);
            strclass.AppendSpaceLine(2, "where " +GetWhereExpression(Keys));

            strclass.AppendSpaceLine(1, "</update>");
            strclass.AppendLine();

            return strclass.ToString();
        }

        private string GenerateDeleteSql()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1,"<delete id=\"delete\" parameterType=\"java.lang.Integer\">");
            strclass.AppendSpaceLine(2, "delete from " + TableName + " where " + GetWhereExpression(Keys));
            strclass.AppendSpaceLine(1, "</delete>");
            strclass.AppendLine();

            return strclass.ToString();
        }

        private string GenerateGetByIdSql()
        {
            var strclass = new StringPlus();

            string tableAlias = CodeCommon.GetUpperChars(TableName).ToLower();

            strclass.AppendSpaceLine(1, "<select id=\"getById\" resultMap=\"baseResultMap\" parameterType=\"java.lang.Integer\">");
            strclass.AppendSpaceLine(2, "select <include refid=\"baseColumnList\"></include> from " + TableName + " " + tableAlias + "(nolock) where " + GetWhereExpression(Keys));
            strclass.AppendSpaceLine(1, "</select>");
            strclass.AppendLine();

            return strclass.ToString();
        }

        private string GenerateSelectSql()
        {
            var strclass = new StringPlus();

            string tableAlias = CodeCommon.GetUpperChars(TableName).ToLower();

            strclass.AppendSpaceLine(1, "<select id=\"select\" resultMap=\"baseResultMap\">");
            strclass.AppendSpaceLine(2, "select <include refid=\"baseColumnList\"></include> from " + TableName + " " + tableAlias + "(nolock) where " + GetWhereExpression(Keys));
            strclass.AppendSpaceLine(1, "</select>");
            strclass.AppendLine();

            return strclass.ToString();
        }

        public string GetXml()
        {
            var strclass = new StringPlus();
            strclass.Append(GenerateMapperHead());

            strclass.Append(GenerateBaseResultMap());

            strclass.Append(GenerateBaseColumnsSql());

            strclass.Append(GenerateInsertSql());

            strclass.Append(GenerateUpdateSql());

            strclass.Append(GenerateDeleteSql());

            strclass.Append(GenerateGetByIdSql());

            strclass.Append(GenerateSelectSql());

            strclass.AppendLine("</mapper>");

            return strclass.ToString();
        }

    }
}
