using System;
using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.Helper;
using AC.Code.IBuilder;

namespace AC.Code.JavaBuilder
{
    public class BuilderDTO : IBuilderDTO
    {
        #region Properties

        public List<ColumnInfo> Fieldlist { get; set; }
        public CodeGenerateConfig GenerateConfig { get; set; }

        #endregion

        protected CodeNameBase CodeName;

        public BuilderDTO(CodeGenerateConfig generateConfig, List<ColumnInfo> fields)
        {
            GenerateConfig = generateConfig;
            Fieldlist = fields;
            CodeName = CodeNameFactory.Create(generateConfig.CodeLayer, CodeType.Java)
                .GetCodeName(generateConfig.SubNamespace, generateConfig.ModelName);
        }

        #region IBuilderDTO Members

        public string GetServiceDTOCode()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("package com.renrentui.entity;");
            strclass.AppendLine();

            strclass.AppendLine("import java.util.Date;");
            strclass.AppendLine();

            strclass.AppendLine("/**");
            strclass.AppendLine(" * 实体类" + CodeName.ServiceDTOName + ". (属性说明自动提取数据库字段的描述信息)");
            strclass.AppendLine(" * @author edaisong.tools");
            strclass.AppendLine(" * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendLine(" *");
            strclass.AppendLine(" */");

            strclass.AppendLine("public class " + CodeName.ServiceDTOName + " {");
            strclass.AppendLine(GenerateFieldsCode());
            strclass.AppendLine(GeneratePropertiesCode());
            strclass.AppendLine("}");

            return strclass.ToString();
        }

        public string GenerateFieldsCode()
        {
            var strclass = new StringPlus();
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                string deText = field.DeText;
                columnType = CodeCommon.DbTypeToJava(columnType);
                strclass.AppendSpaceLine(1, "/**");
                strclass.AppendSpaceLine(1, " * " + deText);
                strclass.AppendSpaceLine(1, " */");
                strclass.AppendSpaceLine(1, string.Format("private {0} {1};", columnType, CodeCommon.SetFirstCharacterLower(columnName)));
                strclass.AppendLine();
            }
            return strclass.ToString();
        }

        public string GeneratePropertiesCode()
        {
            var strclass = new StringPlus();
            var strclass2 = new StringPlus();
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                string deText = field.DeText;
                columnType = CodeCommon.DbTypeToJava(columnType);
                strclass2.AppendSpaceLine(1, "/**");
                strclass2.AppendSpaceLine(1, " * 获取" + deText);
                strclass2.AppendSpaceLine(1, " */");
                //strclass2.AppendSpaceLine(1, string.Format("private {0} {1};", columnType, CodeCommon.SetFirstCharacterLower(columnName)));
                strclass2.AppendSpaceLine(1, string.Format(@"public {0} get{1}() ", columnType, columnName)+"{");
                strclass2.AppendSpaceLine(2, string.Format("return {0};", CodeCommon.SetFirstCharacterLower(columnName)));
                strclass2.AppendSpaceLine(1, "}");
                strclass2.AppendSpaceLine(1, "/**");
                strclass2.AppendSpaceLine(1, " * 设置" + deText);
                strclass2.AppendSpaceLine(1, " * @param " + CodeCommon.SetFirstCharacterLower(columnName) + " " + deText);
                strclass2.AppendSpaceLine(1, " */");
                strclass2.AppendSpaceLine(1,
                    string.Format(@"public void set{0}({1} {2}) ", columnName, columnType, CodeCommon.SetFirstCharacterLower(columnName)) + "{");
                strclass2.AppendSpaceLine(2, string.Format("this.{0} = {0};", CodeCommon.SetFirstCharacterLower(columnName)));
                strclass2.AppendSpaceLine(1, "}");
                strclass2.AppendLine();
            }
            strclass.Append(strclass2.Value);
            
            return strclass.ToString();
        }

        #endregion
    }
}