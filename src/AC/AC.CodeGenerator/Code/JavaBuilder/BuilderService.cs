using System;
using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;
using AC.Code.IBuilder;

namespace AC.Code.JavaBuilder
{
    public class BuilderService : IBuilderService
    {
        #region ��������

        public List<ColumnInfo> Keys { get; set; }
        public CodeGenerateConfig GenerateConfig { get; set; }
        public DbType DbType { get; set; }

        protected CodeNameBase CodeName { get; private set; }
        protected bool IsHasIdentity { get; private set; }

        /// <summary>
        /// ��������ʶ������
        /// </summary>
        protected string IdentityType { get; set; }
        #endregion

        #region ���캯��

        public BuilderService()
        {
        }

        public BuilderService(List<ColumnInfo> keys, CodeGenerateConfig generateConfig)
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
                        IdentityType = CodeCommon.DbTypeToJava(key.TypeName);
                    }
                }
            }
        }

        #endregion

        public string GetServiceCode()
        {
            var strclass = new StringPlus();
            strclass.Append(GenerateServiceHead());

            #region  ��������

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

            strclass.AppendSpaceLine(1,"/**");
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
            strclass.AppendSpaceLine(1, returnType + " create(" + CodeName.CodeOfMethodDTOParam + ");");

            return strclass.ToString();
        }

        public string GenerateModifyCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(1, "/**");
            strclass.AppendSpaceLine(1, " * ����һ����¼");
            strclass.AppendSpaceLine(1, " * @author " + GenerateConfig.Author);
            strclass.AppendSpaceLine(1, " * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, " * @param " + CodeName.CodeOfDTOParam + " Ҫ���ĵĶ���");
            strclass.AppendSpaceLine(1, " */");

            strclass.AppendSpaceLine(1, "void modify(" + CodeName.CodeOfMethodDTOParam + ");");

            return strclass.ToString();
        }

        public string GenerateRemoveCode()
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

            strclass.AppendSpaceLine(1, "void remove(" + CodeCommon.GetInParameterOfJava(Keys) + ");");

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
            strclass.AppendSpaceLine(1, "List<" + CodeName.ServiceDTOName + "> query(" + CodeName.CodeOfQueryMethodDTOParam + ");");

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

            strclass.AppendSpaceLine(1, CodeName.ServiceDTOName+" getById(" + CodeCommon.GetInParameterOfJava(Keys) + ");");

            return strclass.ToString();
        }

        

        private string GenerateServiceHead()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("package " + CodeName.ServiceNamespace + ";");
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
            strclass.AppendLine(" * ����ӿ� " + CodeName.ServiceName + "");
            strclass.AppendLine(" * @author " + GenerateConfig.Author);
            strclass.AppendLine(" * @date " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendLine(" *");
            strclass.AppendLine(" */");

            strclass.AppendLine("public interface " + CodeName.ServiceName + " {");

            return strclass.ToString();
        }
    }
}