using System;
using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.Helper;
using AC.Code.IBuilder;

namespace AC.Code.Builder
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
            strclass.AppendLine("using System;");
            strclass.AppendLine("namespace " + CodeName.ServiceDTONamespace);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// 实体类" + CodeName.ServiceDTOName + " 。(属性说明自动提取数据库字段的描述信息)");
            strclass.AppendSpaceLine(1, "/// Generate By: " + Environment.UserName);
            strclass.AppendSpaceLine(1, "/// Generate Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, "/// </summary>");
            //strclass.AppendSpaceLine(1, "[Serializable]");
            strclass.AppendSpaceLine(1, "public class " + CodeName.ServiceDTOName);
            strclass.AppendSpaceLine(1, "{");
            strclass.AppendSpaceLine(2, "public " + CodeName.ServiceDTOName + "() { }");
            //strclass.AppendSpaceLine(2, "{}");
            strclass.AppendLine(GeneratePropertiesCode());
            strclass.AppendSpaceLine(1, "}");
            if (GenerateConfig.QueryMethodNeeded)
            {
                strclass.AppendSpaceLine(1, "/// <summary>");
                strclass.AppendSpaceLine(1,
                                         "/// 查询对象类" + CodeName.ServiceQueryDTOName + " 。(属性说明自动提取数据库字段的描述信息)");
                strclass.AppendSpaceLine(1, "/// Generate By: " + Environment.UserName);
                strclass.AppendSpaceLine(1, "/// Generate Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                strclass.AppendSpaceLine(1, "/// </summary>");
                strclass.AppendSpaceLine(1, "public class " + CodeName.ServiceQueryDTOName);
                strclass.AppendSpaceLine(1, "{");
                strclass.AppendSpaceLine(2, "public " + CodeName.ServiceQueryDTOName + "() { }");
                strclass.AppendSpaceLine(1, "}");
            }

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
                bool isIdentity = field.IsIdentity;
                bool ispk = field.IsPK;
                bool cisnull = field.cisNull;
                string deText = field.DeText;
                columnType = CodeCommon.DbTypeToCS(columnType);
                string isnull = "";
                if (CodeCommon.IsValueType(columnType))
                {
                    if ((!isIdentity) && (!ispk) && (cisnull))
                    {
                        isnull = "?"; //代表可空类型
                    }
                }
                //strclass1.AppendSpaceLine(2, "private " + columnType + isnull + " _" + columnName.ToLower() + ";");//私有变量
                strclass2.AppendSpaceLine(2, "/// <summary>");
                strclass2.AppendSpaceLine(2, "/// " + deText);
                strclass2.AppendSpaceLine(2, "/// </summary>");
                strclass2.AppendSpaceLine(2, "public " + columnType + isnull + " " + columnName + " { get; set; }");
                //属性
                //strclass2.AppendSpaceLine(2, "{");
                //strclass2.AppendSpaceLine(3, "set{" + " _" + columnName.ToLower() + "=value;}");
                //strclass2.AppendSpaceLine(3, "get{return " + "_" + columnName.ToLower() + ";}");
                //strclass2.AppendSpaceLine(2, "}");
            }
            //strclass.Append(strclass1.Value);
            strclass.Append(strclass2.Value);
            //strclass.AppendSpaceLine(2, "#endregion Model");

            return strclass.ToString();
        }

        #endregion
    }
}