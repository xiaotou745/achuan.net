using System;
using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;
using AC.Code.IBuilder;

namespace AC.Code.JavaBuilder
{
    public class BuilderDomain : IBuilderDomain
    {
        #region  ���캯��

        public BuilderDomain()
        {
        }

        public BuilderDomain(List<ColumnInfo> keys, CodeGenerateConfig generateConfig)
        {
            Keys = keys;
            GenerateConfig = generateConfig;
            CodeName = generateConfig.CodeName;

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

        protected CodeNameBase CodeName { get; set; }

        /// <summary>
        /// ��������ʶ������
        /// </summary>
        protected string IdentityType { get; set; }

        protected bool IsHasIdentity { get; set; }

        #region IBuilderDomain Members

        public string GetDomainCode()
        {
            var strclass = new StringPlus();
            strclass.Append(GenerateDomainHead());

            #region  ��������

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
        private string GenerateDomainHead()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("package " + CodeName.DomainNamespace + ";");
            strclass.AppendLine();

            strclass.AppendLine("import java.util.List;");
            strclass.AppendLine(string.Format("import {0};", CodeName.ServiceDTOFullName));
            string requestBase = CodeName.getRequestBaseOfJava();
            if (!string.IsNullOrEmpty(requestBase))
            {
                strclass.AppendLine(string.Format("import {0}", requestBase));
            }
            strclass.AppendLine();

            strclass.AppendLine("/**");
            strclass.AppendLine(" * �������ӿ� " + CodeName.DomainName + "");
            strclass.AppendLine(" * @author " + GenerateConfig.Author);
            strclass.AppendLine(" * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendLine(" *");
            strclass.AppendLine(" */");

            strclass.AppendLine("public interface " + CodeName.DomainName + " {");

            return strclass.ToString();
        }
        public string GenerateInsertCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * ����һ����¼");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, " * @param " + CodeName.CodeOfDTOParam + " Ҫ�����Ķ���");
            if (IsHasIdentity)
            {
                strclass.AppendSpaceLine(1, " * @return  �����������������Id");
            }
            strclass.AppendSpaceLine(1, " */");

            string returnType = "void";
            if (IsHasIdentity)
            {
                returnType = IdentityType;
            }
            strclass.AppendSpaceLine(1, returnType + " insert(" + CodeName.CodeOfMethodDTOParam + ");");

            return strclass.ToString();
        }

        public string GenerateUpdateCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * ����һ����¼");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, " * @param " + CodeName.CodeOfDTOParam + " Ҫ���ĵĶ���");
            strclass.AppendSpaceLine(1, " */");

            strclass.AppendSpaceLine(1, "void update(" + CodeName.CodeOfMethodDTOParam + ");");

            return strclass.ToString();
        }

        public string GenerateDeleteCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * ɾ��һ����¼");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            foreach (var key in Keys)
            {
                strclass.AppendSpaceLine(1,
                    " * @param " + CodeCommon.SetFirstCharacterLower(key.ColumnName) + " " + key.DeText);
            }
            strclass.AppendSpaceLine(1, " */");

            strclass.AppendSpaceLine(1, "void delete(" + CodeCommon.GetInParameterOfJava(Keys) + ");");

            return strclass.ToString();
        }

        public string GenerateQueryCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * ��ѯ����");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, " * @param " + CodeName.CodeOfQueryDTOParam + " ��ѯ����");
            strclass.AppendSpaceLine(1, " */");
            strclass.AppendSpaceLine(1, "List<" + CodeName.ServiceDTOName + "> select(" + CodeName.CodeOfQueryMethodDTOParam + ");");

            return strclass.ToString();
        }

        public string GenerateGetByIdCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * ����Id�õ�һ������ʵ��");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            foreach (var key in Keys)
            {
                strclass.AppendSpaceLine(1,
                    " * @param " + CodeCommon.SetFirstCharacterLower(key.ColumnName) + " " + key.DeText);
            }
            strclass.AppendSpaceLine(1, " */");

            strclass.AppendSpaceLine(1, CodeName.ServiceDTOName + " getById(" + CodeCommon.GetInParameterOfJava(Keys) + ");");

            return strclass.ToString();
        }

        #endregion

        #region ��������

        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public DbType DbType { get; set; }

        public List<ColumnInfo> Keys { get; set; }
        public CodeGenerateConfig GenerateConfig { get; set; }

        #endregion
    }
}