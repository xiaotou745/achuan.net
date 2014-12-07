using System;
using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;
using AC.Code.IBuilder;

namespace AC.Code.JavaBuilder
{
    public class ServiceBuilder
    {
        private CodeGenerateConfig generateConfig;
        private List<ColumnInfo> keys;

        #region Create

        private ServiceBuilder()
        {
        }

        public static ServiceBuilder Create()
        {
            var serviceBuilder = new ServiceBuilder();
            return serviceBuilder;
        }

        public static ServiceBuilder Create(CodeGenerateConfig codeGenerateConfig, List<ColumnInfo> colKeys)
        {
            var serviceBuilder = new ServiceBuilder();
            serviceBuilder.generateConfig = codeGenerateConfig;
            serviceBuilder.keys = colKeys;
            return serviceBuilder;
        }

        #endregion

        #region Set

        public ServiceBuilder SetGenerateConfig(CodeGenerateConfig codeGenerateConfig)
        {
            generateConfig = codeGenerateConfig;
            return this;
        }

        public ServiceBuilder SetKeys(List<ColumnInfo> colKeys)
        {
            keys = colKeys;
            return this;
        }

        #endregion

        #region Get

        public IBuilderService GetServiceBuilder()
        {
            if (generateConfig == null)
            {
                return null;
            }
            switch (generateConfig.CodeLayer)
            {
                case CodeLayer.ThreeLayer:
                case CodeLayer.ServiceThreeLayer:
                    return new BuilderServiceOfThreeLayer(keys, generateConfig);
                case CodeLayer.ServiceLayerWithDomain:
                case CodeLayer.ServiceLayerWithoutDomain:
                    return new BuilderServiceOfSerivceLayer(keys, generateConfig);
                default:
                    return new BuilderServiceOfThreeLayer(keys, generateConfig);
            }
        }

        #endregion
    }

    public abstract class BuilderServiceBase : IBuilderService
    {
        protected bool IsHasIdentity { get; private set; }

        /// <summary>
        /// 自增长标识列类型
        /// </summary>
        protected string IdentityType { get; set; }

        protected CodeNameBase CodeName { get; private set; }

        #region 公共属性

        public List<ColumnInfo> Keys { get; set; }
        public CodeGenerateConfig GenerateConfig { get; set; }
        public DbType DbType { get; set; }

        #endregion

        #region 构造函数

        protected BuilderServiceBase()
        {
        }

        protected BuilderServiceBase(List<ColumnInfo> keys, CodeGenerateConfig generateConfig)
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

        #region IBuilderService Members

        public virtual string GetServiceCode()
        {
            var strclass = new StringPlus();
            strclass.Append(GenerateServiceHead());

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

        public abstract string GenerateCreateCode();
        public abstract string GenerateModifyCode();
        public abstract string GenerateRemoveCode();
        public abstract string GenerateQueryCode();
        public abstract string GenerateGetByIdCode();

        #endregion

        public abstract string GenerateServiceHead();
    }

    public class BuilderServiceOfSerivceLayer : BuilderServiceBase
    {
        public BuilderServiceOfSerivceLayer()
        {
        }

        public BuilderServiceOfSerivceLayer(List<ColumnInfo> keys, CodeGenerateConfig generateConfig)
            : base(keys, generateConfig)
        {
        }

        public override string GenerateServiceHead()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("using System.Collections.Generic;");
            strclass.AppendLine("using " + CodeName.ServiceDTONamespace + ";");

            strclass.AppendLine("namespace " + CodeName.ServiceNamespace);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// 业务逻辑类" + CodeName.ServiceName + " 的摘要说明。");
            strclass.AppendSpaceLine(1, "/// Generate By: " + Environment.UserName);
            strclass.AppendSpaceLine(1, "/// Generate Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpaceLine(1, "public interface " + CodeName.ServiceName);
            strclass.AppendSpaceLine(1, "{");

            return strclass.ToString();
        }

        public override string GenerateCreateCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 新增一条记录");
            strclass.AppendSpaceLine(2, string.Format("///<param name=\"{0}\">要新增的对象</param>", CodeName.CodeOfDTOParam));
            strclass.AppendSpaceLine(2, "/// </summary>");

            string returnType = "void";
            if (IsHasIdentity)
            {
                returnType = IdentityType;
            }
            strclass.AppendSpaceLine(2, returnType + " Create(" + CodeName.CodeOfMethodDTOParam + ");");

            return strclass.ToString();
        }

        public override string GenerateModifyCode()
        {
            var strclass = new StringPlus();

            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 修改一条记录");
            strclass.AppendSpaceLine(2, string.Format("///<param name=\"{0}\">要修改的对象</param>", CodeName.CodeOfDTOParam));
            strclass.AppendSpaceLine(2, "/// </summary>");

            strclass.AppendSpaceLine(2, "void Modify(" + CodeName.CodeOfMethodDTOParam + ");");

            return strclass.ToString();
        }

        public override string GenerateRemoveCode()
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 删除一条记录");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "void Remove(" + CodeCommon.GetInParameter(Keys) + ");");
            return strclass.ToString();
        }

        public override string GenerateQueryCode()
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 查询方法");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2,
                                     "IList<" + CodeName.ServiceDTOName + "> Query(" +
                                     CodeName.CodeOfQueryMethodDTOParam + ");");
            return strclass.ToString();
        }

        public override string GenerateGetByIdCode()
        {
            var strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 根据Id得到一个对象实体");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, CodeName.ServiceDTOName + " GetById(" + CodeCommon.GetInParameter(Keys) + ");");
            return strclass.ToString();
        }
    }

    public class BuilderServiceOfThreeLayer : BuilderServiceBase
    {
        public BuilderServiceOfThreeLayer()
        {
        }

        public BuilderServiceOfThreeLayer(List<ColumnInfo> keys, CodeGenerateConfig generateConfig)
            : base(keys, generateConfig)
        {
        }

        private string DaoParamCode()
        {
            return CodeCommon.SetFirstCharacterLower(CodeName.DaoName);
        }

        public override string GenerateServiceHead()
        {
            var strclass = new StringPlus();
            strclass.AppendLine("using System.Collections.Generic;");
            strclass.AppendLine("using " + CodeName.ServiceDTONamespace + ";");
            strclass.AppendLine("using " + CodeName.DaoNamespace + ";");
            if (GenerateConfig.CallStyle == CallStyle.SpringNew)
            {
                strclass.AppendLine("using DZ.SpringNet;");
            }

            strclass.AppendLine("namespace " + CodeName.ServiceNamespace);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// Service类" + CodeName.ServiceName + " 的摘要说明。");
            strclass.AppendSpaceLine(1, "/// Generate By: " + Environment.UserName);
            strclass.AppendSpaceLine(1, "/// Generate Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            strclass.AppendSpaceLine(1, "/// </summary>");
            if (GenerateConfig.CallStyle == CallStyle.SpringNew)
            {
                strclass.AppendSpaceLine(1,
                                         "[Spring(ConstructorArgs = \"" + DaoParamCode() + ":" +
                                         DaoParamCode() + "\")]");
            }
            strclass.AppendSpaceLine(1, "public class " + CodeName.ServiceName);
            strclass.AppendSpaceLine(1, "{");
            if (GenerateConfig.CallStyle == CallStyle.SpringNew)
            {
                strclass.AppendSpaceLine(2, "private readonly " + CodeName.DaoName + " " + DaoParamCode() + ";");
                strclass.AppendSpaceLine(2,
                                         "public " + CodeName.ServiceName + "(" + CodeName.DaoName + " " +
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
                strclass.AppendSpaceLine(2, "public " + CodeName.ServiceName + "()");
                strclass.AppendSpaceLine(2, "{");
                strclass.AppendSpaceLine(2, "}");
            }
            return strclass.ToString();
        }

        public override string GenerateCreateCode()
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

        public override string GenerateModifyCode()
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

        public override string GenerateRemoveCode()
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

        public override string GenerateQueryCode()
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

        public override string GenerateGetByIdCode()
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
    }
}