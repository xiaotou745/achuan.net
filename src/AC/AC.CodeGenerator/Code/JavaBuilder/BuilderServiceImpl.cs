using System;
using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;
using AC.Code.IBuilder;

namespace AC.Code.JavaBuilder
{
    public class BuilderServiceImpl : IBuilderServiceImpl
    {
        protected bool IsHasIdentity { get; private set; }

        /// <summary>
        /// 自增长标识列类型
        /// </summary>
        protected string IdentityType { get; set; }

        protected CodeNameBase CodeName { get; private set; }

        #region 公共属性

        public List<ColumnInfo> Fieldlist { get; set; }
        public List<ColumnInfo> Keys { get; set; }
        public CodeGenerateConfig GenerateConfig { get; set; }
        public DbType DbType { get; set; }

        #endregion

        #region 构造函数

        public BuilderServiceImpl()
        {
        }

        public BuilderServiceImpl(List<ColumnInfo> keys, CodeGenerateConfig generateConfig)
        {
            GenerateConfig = generateConfig;
            Keys = keys;
            CodeName = generateConfig.CodeName;
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
        private string DaoParamCode()
        {
            if (GenerateConfig.CodeLayer == CodeLayer.ServiceLayerWithDomain)
            {
                return CodeCommon.GetInterfaceParameter(CodeName.DomainName);
            }
            return CodeCommon.SetFirstCharacterLower(CodeName.DaoName);
        }

        #region IBuilderServiceImpl Members


        public string GetServiceImplCode()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("using System.Collections.Generic;");
            strclass.AppendLine("using " + CodeName.ServiceDTONamespace + ";");
            strclass.AppendLine("using " + CodeName.ServiceNamespace + ";");
            if(GenerateConfig.CodeLayer == CodeLayer.ServiceLayerWithoutDomain)
            {
                strclass.AppendLine("using " + CodeName.DaoNamespace + ";");
            }
            else if(GenerateConfig.CodeLayer == CodeLayer.ServiceLayerWithDomain)
            {
                strclass.AppendLine("using " + CodeName.DomainNamespace + ";");
            }
            if (GenerateConfig.CallStyle == CallStyle.SpringNew)
            {
                strclass.AppendLine("using DZ.SpringUtils;");
            }

            strclass.AppendLine("namespace " + CodeName.ServiceImplNamespace);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// Service类" + CodeName.ServiceImplName + " 的摘要说明。");
            strclass.AppendSpaceLine(1, "/// Generate By: " + Environment.UserName);
            strclass.AppendSpaceLine(1, "/// Generate Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, "/// </summary>");
            if (GenerateConfig.CallStyle == CallStyle.SpringNew)
            {
                strclass.AppendSpaceLine(1,
                                         "[Spring(ConstructorArgs = \"" + DaoParamCode() + ":" +
                                         DaoParamCode() + "\")]");
            }
            strclass.AppendSpaceLine(1, "public class " + CodeName.ServiceImplName + " : " + CodeName.ServiceName);
            strclass.AppendSpaceLine(1, "{");
            if (GenerateConfig.CallStyle == CallStyle.SpringNew)
            {
                string daoTypeCode = GenerateConfig.CodeLayer == CodeLayer.ServiceLayerWithDomain
                                         ? CodeName.DomainName
                                         : CodeName.DaoName;
                strclass.AppendSpaceLine(2, "private readonly " + daoTypeCode + " " + DaoParamCode() + ";");
                strclass.AppendSpaceLine(2,
                                         "public " + CodeName.ServiceImplName + "(" + daoTypeCode + " " +
                                         DaoParamCode() + ")");
                strclass.AppendSpaceLine(2, "{");
                strclass.AppendSpaceLine(3, "this." + DaoParamCode() + " = " + DaoParamCode() + ";");
                strclass.AppendSpaceLine(2, "}");
            }
            else
            {
                strclass.AppendSpaceLine(2,
                                         "private readonly " + CodeName.DaoName + " " + DaoParamCode() + "= new " +
                                         CodeName.DaoName + "();");
                strclass.AppendSpaceLine(2, "public " + CodeName.ServiceImplName + "()");
                strclass.AppendSpaceLine(2, "{");
                strclass.AppendSpaceLine(2, "}");
            }

            #region  方法代码

            if (GenerateConfig.CreateMethodNeeded)
            {
                strclass.AppendLine(GenerateCreateCode());
            }
            if (GenerateConfig.ModifyMethodNeeded)
            {
                strclass.AppendLine(GenerateModifyCode());
            }
            if (GenerateConfig.RemoveMethodNeeded)
            {
                strclass.AppendLine(GenerateRemoveCode());
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

            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("}");
            strclass.AppendLine(string.Empty);

            return strclass.ToString();
        }

        public string GenerateCreateCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 新增一条记录");
            strclass.AppendSpaceLine(2, "/// </summary>");

            string returnType = "void";
            if ((DbType == DbType.SQL2005 || DbType == DbType.SQL2000) && (IsHasIdentity))
            {
                returnType = IdentityType;
            }
            strclass.AppendSpaceLine(2, "public " + returnType + " Create(" + CodeName.CodeOfMethodDTOParam + ")");
            strclass.AppendSpaceLine(2, "{");
            if (returnType == "void")
            {
                strclass.AppendSpaceLine(3, DaoParamCode() + ".Insert(" + CodeName.CodeOfDTOParam + ");");
            }
            else
            {
                strclass.AppendSpaceLine(3, "return " + DaoParamCode() + ".Insert(" + CodeName.CodeOfDTOParam + ");");
            }
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }

        public string GenerateModifyCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 修改一条记录");
            strclass.AppendSpaceLine(2, "/// </summary>");

            strclass.AppendSpaceLine(2, "public void Modify(" + CodeName.CodeOfMethodDTOParam + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, DaoParamCode() + ".Update(" + CodeName.CodeOfDTOParam + ");");
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }

        public string GenerateRemoveCode()
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 删除一条记录");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public void Remove(" + CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3,
                                     DaoParamCode() + ".Delete(" + CodeCommon.GetFieldStringListWithFirstLower(Keys) +
                                     ");");
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }

        public string GenerateQueryCode()
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 查询方法");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2,
                                     "public IList<" + CodeName.ServiceDTOName + "> Query(" +
                                     CodeName.CodeOfQueryMethodDTOParam + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "return " + DaoParamCode() + ".Query(" + CodeName.CodeOfQueryDTOParam + ");");
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }

        public string GenerateGetByIdCode()
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 得到一个对象实体");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2,
                                     "public " + CodeName.ServiceDTOName + " GetById(" +
                                     CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3,
                                     "return " + DaoParamCode() + ".GetById(" +
                                     CodeCommon.GetFieldStringListWithFirstLower(Keys) + ");");
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }

        #endregion
    }
}