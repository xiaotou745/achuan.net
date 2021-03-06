﻿using System;
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
            CodeName = CodeNameFactory.Create(generateConfig.CodeLayer).GetCodeName(generateConfig.SubNamespace,
                                                                                    generateConfig.ModelName);
        }

        #region IBuilderDTO Members

        public string GetServiceDTOCode()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("package com.renrentui.entity;");

            strclass.AppendLine("import java.util.Date;");
            strclass.AppendLine("/**");
            strclass.AppendLine(" * 实体类" + CodeName.ServiceDTOName + ". (属性说明自动提取数据库字段的描述信息)");
            strclass.AppendLine(" * @author edaisong.system");
            strclass.AppendLine(" * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendLine(" *");
            strclass.AppendLine(" */");

            strclass.AppendLine("public class " + CodeName.ServiceDTOName + " {");
            strclass.AppendLine(GeneratePropertiesCode());
            strclass.AppendLine("}");

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
                columnType = CodeCommon.DbTypeToCS(columnType);
                strclass2.AppendSpaceLine(1, "/**");
                strclass2.AppendSpaceLine(1, " * " + deText);
                strclass2.AppendSpaceLine(1, " */");
                strclass2.AppendSpaceLine(1, string.Format("private {0} {1};", columnType, columnName));
                strclass2.AppendSpaceLine(1, string.Format("public {0} get{1}() {", columnType, columnName));
                strclass2.AppendSpaceLine(2, string.Format("return {0};", columnName));
                strclass2.AppendSpaceLine(1, "}");
                strclass2.AppendLine();
                strclass2.AppendSpaceLine(1,
                    string.Format(@"public void set{0}({1} {0}) {", columnName, columnType));
                strclass2.AppendSpaceLine(2, string.Format("this.{0} = {0}", columnName));
                strclass2.AppendSpaceLine(1, "}");
            }
            strclass.Append(strclass2.Value);
            
            return strclass.ToString();
        }

        #endregion
    }
}