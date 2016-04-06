using System;
using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;
using AC.Code.IBuilder;

namespace AC.Code.Builder
{
    public class DomainBuilder
    {
        private CodeGenerateConfig generateConfig;
        private List<ColumnInfo> keys;

        #region Create

        private DomainBuilder()
        {
        }

        public static DomainBuilder Create()
        {
            var serviceBuilder = new DomainBuilder();
            return serviceBuilder;
        }

        public static DomainBuilder Create(CodeGenerateConfig codeGenerateConfig, List<ColumnInfo> colKeys)
        {
            var serviceBuilder = new DomainBuilder();
            serviceBuilder.generateConfig = codeGenerateConfig;
            serviceBuilder.keys = colKeys;
            return serviceBuilder;
        }

        #endregion

        #region Set

        public DomainBuilder SetGenerateConfig(CodeGenerateConfig codeGenerateConfig)
        {
            generateConfig = codeGenerateConfig;
            return this;
        }

        public DomainBuilder SetKeys(List<ColumnInfo> colKeys)
        {
            keys = colKeys;
            return this;
        }

        #endregion

        #region Get

        public IBuilderDomain GetDomain()
        {
            if (generateConfig == null)
            {
                return null;
            }
            if (generateConfig.Language == CodeLanguage.CSharp)
            {
                return new BuilderDomain(keys, generateConfig);
            }
            else if (generateConfig.Language == CodeLanguage.Java)
            {
                return new JavaBuilder.BuilderDomain(keys, generateConfig);
            }
            return null;
        }

        #endregion
    }

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
                        IdentityType = CodeCommon.DbTypeToCS(key.TypeName);
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
            strclass.AppendLine("using System.Collections.Generic;");
            strclass.AppendLine("using " + CodeName.ServiceDTONamespace + ";");

            strclass.AppendLine("namespace " + CodeName.DomainNamespace);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// ҵ�����������" + CodeName.DomainName + " ��ժҪ˵����");
            strclass.AppendSpaceLine(1, "/// Generate By: " + Environment.UserName);
            strclass.AppendSpaceLine(1, "/// Generate Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpaceLine(1, "public interface " + CodeName.DomainName);
            strclass.AppendSpaceLine(1, "{");

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

            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("}");
            strclass.AppendLine(string.Empty);

            return strclass.ToString();
        }

        public string GenerateInsertCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ����һ����¼");
            //strclass.AppendSpaceLine(2, @"///<param name="{0}"></param>");}
            strclass.AppendSpaceLine(2, "/// </summary>");

            string returnType = "void";
            if (IsHasIdentity)
            {
                returnType = IdentityType;
            }
            strclass.AppendSpaceLine(2, returnType + " Insert(" + CodeName.CodeOfMethodDTOParam + ");");

            return strclass.ToString();
        }

        public string GenerateUpdateCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// �޸�һ����¼");
            strclass.AppendSpaceLine(2, "/// </summary>");

            strclass.AppendSpaceLine(2, "void Update(" + CodeName.CodeOfMethodDTOParam + ");");

            return strclass.ToString();
        }

        public string GenerateDeleteCode()
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ɾ��һ����¼");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "void Delete(" + CodeCommon.GetInParameter(Keys) + ");");
            return strclass.ToString();
        }

        public string GenerateQueryCode()
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ��ѯ����");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2,
                                     "IList<" + CodeName.ServiceDTOName + "> Query(" +
                                     CodeName.CodeOfQueryMethodDTOParam + ");");
            return strclass.ToString();
        }

        public string GenerateGetByIdCode()
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// �õ�һ������ʵ��");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2,
                                     CodeName.ServiceDTOName + " GetById(" +
                                     CodeCommon.GetInParameter(Keys) + ");");
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