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
            return CodeCommon.GetInterfaceParameter(CodeName.DomainName);
        }

        #region IBuilderServiceImpl Members

        private string GenerateDomainHead()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("package " + CodeName.ServiceImplNamespace + ";");
            strclass.AppendLine();

            strclass.AppendLine("import java.util.List;");
            strclass.AppendLine("import org.springframework.beans.factory.annotation.Autowired;");
            strclass.AppendLine("import org.springframework.stereotype.Service;");
            strclass.AppendLine(string.Format("import {0};", CodeName.ServiceDTOFullName));
            strclass.AppendLine(string.Format("import {0};", CodeName.ServiceFullName));
            strclass.AppendLine(string.Format("import {0};", CodeName.DomainFullName));
            strclass.AppendLine();

            strclass.AppendLine("/**");
            strclass.AppendLine(" * 服务提供对象 " + CodeName.ServiceImplName + "");
            strclass.AppendLine(" * @author " + GenerateConfig.Author);
            strclass.AppendLine(" * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendLine(" *");
            strclass.AppendLine(" */");
            strclass.AppendLine("@Service");

            strclass.AppendLine("public class " + CodeName.ServiceImplName + " implements "+CodeName.ServiceName+" {");
            strclass.AppendSpaceLine(1, "@Autowired");
            strclass.AppendSpaceLine(1, "private " + CodeName.DomainName + " " + DaoParamCode() + ";");
            return strclass.ToString();
        }
        
        public string GetServiceImplCode()
        {
            var strclass = new StringPlus();
            strclass.Append(GenerateDomainHead());

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

            strclass.AppendLine("}");

            return strclass.ToString();
        }

        public string GenerateCreateCode()
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
            strclass.AppendSpaceLine(1, "public "+returnType + " create(" + CodeName.CodeOfMethodDTOParam + ") {");
            if (returnType == "void")
            {
                strclass.AppendSpaceLine(2, DaoParamCode() + ".insert(" + CodeName.CodeOfDTOParam + ");");
            }
            else
            {
                strclass.AppendSpaceLine(2, "return " + DaoParamCode() + ".insert(" + CodeName.CodeOfDTOParam + ");");
            }
            strclass.AppendSpaceLine(1, "}");
            
            return strclass.ToString();
        }

        public string GenerateModifyCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * 更新一条记录");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, " * @param " + CodeName.CodeOfDTOParam + " 要更改的对象");
            strclass.AppendSpaceLine(1, " */");

            strclass.AppendSpaceLine(1, "public void modify(" + CodeName.CodeOfMethodDTOParam + ") {");
            strclass.AppendSpaceLine(2, DaoParamCode() + ".update(" + CodeName.CodeOfDTOParam + ");");
            strclass.AppendSpaceLine(1, "}");

            return strclass.ToString();
        }

        public string GenerateRemoveCode()
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

            strclass.AppendSpaceLine(1, "public void remove(" + CodeCommon.GetInParameterOfJava(Keys) + ") {");
            strclass.AppendSpaceLine(2,
                DaoParamCode() + ".delete(" + CodeCommon.GetFieldStringListWithFirstLower(Keys) + ");");
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
            strclass.AppendSpaceLine(1, "public List<" + CodeName.ServiceDTOName + "> query(" + CodeName.CodeOfQueryMethodDTOParam + ") {");
            strclass.AppendSpaceLine(2, "return " + DaoParamCode() + ".select(" + CodeName.CodeOfQueryDTOParam + ");");
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

            strclass.AppendSpaceLine(1, "public "+CodeName.ServiceDTOName + " getById(" + CodeCommon.GetInParameterOfJava(Keys) + ") {");
            strclass.AppendSpaceLine(2,
                                     "return " + DaoParamCode() + ".getById(" +
                                     CodeCommon.GetFieldStringListWithFirstLower(Keys) + ");");
            strclass.AppendSpaceLine(1, "}");

            return strclass.ToString();
        }

        #endregion
    }
}