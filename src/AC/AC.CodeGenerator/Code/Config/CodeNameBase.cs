using System;
using System.Text;
using AC.Code.Helper;

namespace AC.Code.Config
{
    public class CodeNameFactory
    {
        private CodeLayer codeLayer;
        private CodeType codeType;
        private CustomCodeName customCodeName;

        private CodeNameFactory()
        {
        }

        public static CodeNameFactory Create(CodeLayer codeLayer)
        {
            return new CodeNameFactory {codeLayer = codeLayer, codeType = CodeType.CSharp};
        }

        public static CodeNameFactory Create(CodeLayer codeLayer, CustomCodeName customCodeName)
        {
            return new CodeNameFactory {codeLayer = codeLayer, customCodeName = customCodeName};
        }

        public static CodeNameFactory Create(CodeLayer codeLayer, CodeType codeType)
        {
            return new CodeNameFactory {codeLayer = codeLayer, codeType = codeType};
        }

        public CodeNameBase GetCodeName(string subNamespace, string modelName)
        {
            //如果使用自定义命名规则，则全部使用自定义的吧
            if (customCodeName != null)
            {
                return new CodeNameOfCustom(subNamespace, modelName, customCodeName);
            }
            if (codeType == CodeType.Java)
            {
                return new CodeNameOfJava(subNamespace, modelName);
            }
            switch (codeLayer)
            {
                case CodeLayer.ThreeLayer:
                    return new CodeNameOfThreeLayer(subNamespace, modelName);
                case CodeLayer.ServiceThreeLayer:
                    return new CodeNameOfServiceThreeLayer(subNamespace, modelName);
                case CodeLayer.ServiceLayerWithDomain:
                case CodeLayer.ServiceLayerWithoutDomain:
                    return new CodeNameOfServiceLayer(subNamespace, modelName);
                default:
                    return new CodeNameOfServiceLayer(subNamespace, modelName);
            }
        }
    }

    public abstract class CodeNameBase
    {
        #region Service Layer

        /// <summary>
        /// Service命名空间
        /// </summary>
        public abstract string ServiceNamespace { get; }

        public abstract string ServiceName { get; }

        public string ServiceFullName
        {
            get { return ServiceNamespace + "." + ServiceName; }
        }

        #endregion

        #region ServiceImpl Layer

        /// <summary>
        /// Service命名空间
        /// </summary>
        public abstract string ServiceImplNamespace { get; }

        public abstract string ServiceImplName { get; }

        public string ServiceImplFullName
        {
            get { return ServiceImplNamespace + "." + ServiceImplName; }
        }

        #endregion

        #region Domain

        public abstract string DomainNamespace { get; }

        public abstract string DomainName { get; }

        public string DomainFullName
        {
            get { return DomainNamespace + "." + DomainName; }
        }

        #endregion

        #region DTO Layer

        /// <summary>
        /// Service命名空间
        /// </summary>
        public abstract string ServiceDTONamespace { get; }

        public abstract string ServiceDTOName { get; }

        public string ServiceDTOFullName
        {
            get { return ServiceDTONamespace + "." + ServiceDTOName; }
        }

        /// <summary>
        /// DTO 参数
        /// </summary>
        public string CodeOfDTOParam
        {
            get { return CodeCommon.SetFirstCharacterLower(ServiceDTOName); }
        }

        public string CodeOfMethodDTOParam
        {
            get { return ServiceDTOName + " " + CodeOfDTOParam; }
        }

        #endregion

        #region Query DTO

        public abstract string ServiceQueryDTOName { get; }

        public string CodeOfQueryDTOParam
        {
            get { return CodeCommon.SetFirstCharacterLower(ServiceQueryDTOName); }
        }

        public string CodeOfQueryMethodDTOParam
        {
            get { return ServiceQueryDTOName + " " + CodeOfQueryDTOParam; }
        }

        #endregion

        #region Dao Layer

        /// <summary>
        /// Service命名空间
        /// </summary>
        public abstract string DaoNamespace { get; }

        public abstract string DaoName { get; }

        public string DaoFullName
        {
            get { return DaoNamespace + "." + DaoName; }
        }

        #endregion

        protected static string GetModelName(string modelName)
        {
            string[] names = modelName.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            foreach (string name in names)
            {
                sb.Append(CodeCommon.SetFirstCharacterUpper(name));
            }
            return sb.ToString();
        }

        public string getRequestBaseOfJava()
        {
            string requestBase = string.Empty;
            int lastIndexOf = ServiceDTONamespace.LastIndexOf(".", System.StringComparison.Ordinal);
            if (lastIndexOf >= 0)
            {
                requestBase = ServiceDTONamespace.Substring(0, lastIndexOf) + ".common.RequestBase;";
            }
            return requestBase;
        }
    }

    public class CustomCodeName
    {
        public string Author { get; set; }

        public string CommonNamespace { get; set; }

        public string DtoOrDomainNamespace { get; set; }

        public string DaoInterNamespace { get; set; }

        public string DaoNamespace { get; set; }

        public string ServiceInterNamespace { get; set; }

        public string ServiceNamespace { get; set; }
    }

    public class CodeNameOfCustom : CodeNameBase
    {
        protected string ModelName;
        protected string SubNamespace;
        protected CustomCodeName CustomCodeName;

        public CodeNameOfCustom(string subNamespace, string modelName, CustomCodeName customCodeName)
        {
            SubNamespace = subNamespace;
            CustomCodeName = customCodeName;
            var sb = GetModelName(modelName);
            ModelName = sb;
        }

        #region Service Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string ServiceNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                    ? CustomCodeName.ServiceInterNamespace
                    : string.Format("{0}.{1}", CustomCodeName.ServiceInterNamespace, SubNamespace);
            }
        }

        public override string ServiceName
        {
            get { return "I" + ModelName + "Service"; }
        }

        #endregion

        #region DTO Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string ServiceDTONamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                    ? CustomCodeName.DtoOrDomainNamespace
                    : CustomCodeName.DtoOrDomainNamespace + "." + SubNamespace;
            }
        }

        public override string ServiceDTOName
        {
            get { return ModelName; }
        }

        public override string ServiceQueryDTOName
        {
            get { return "RequestBase"; }
        }

        #endregion

        #region Dao Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string DaoNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                    ? CustomCodeName.DaoNamespace
                    : CustomCodeName.DaoNamespace + "." + SubNamespace;
            }
        }

        public override string DaoName
        {
            get { return ModelName + "Dao"; }
        }

        #endregion

        #region ServiceImpl

        public override string ServiceImplNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                    ? CustomCodeName.ServiceNamespace
                    : CustomCodeName.ServiceNamespace + "." + SubNamespace;
            }
        }

        public override string ServiceImplName
        {
            get { return ModelName + "Service"; }
        }

        #endregion

        #region Domain

        public override string DomainNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                    ? CustomCodeName.DaoInterNamespace
                    : CustomCodeName.DaoInterNamespace + "." + SubNamespace;
            }
        }

        public override string DomainName
        {
            get { return "I" + ModelName + "Dao"; }
        }

        #endregion
    }
    public class CodeNameOfJava : CodeNameBase
    {
        protected string ModelName;
        protected string SubNamespace;

        public CodeNameOfJava(string subNamespace, string modelName)
        {
            SubNamespace = subNamespace;
            var sb = GetModelName(modelName);
            ModelName = sb;
        }

        #region Service Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string ServiceNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Services"
                           : string.Format("DZ.Services.{0}", SubNamespace);
            }
        }

        public override string ServiceName
        {
            get { return "I" + ModelName + "Service"; }
        }

        #endregion

        #region DTO Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string ServiceDTONamespace
        {
            get { return ServiceNamespace + ".DTO"; }
        }

        public override string ServiceDTOName
        {
            get { return ModelName; }
        }

        public override string ServiceQueryDTOName
        {
            get { return ModelName + "QueryInfo"; }
        }

        #endregion

        #region Dao Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string DaoNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Dao"
                           : string.Format("DZ.Dao.{0}", SubNamespace);
            }
        }

        public override string DaoName
        {
            get { return ModelName + "Dao"; }
        }

        #endregion

        #region ServiceImpl

        public override string ServiceImplNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Services.Impl"
                           : string.Format(@"DZ.Services.Impl.{0}", SubNamespace);
            }
        }

        public override string ServiceImplName
        {
            get { return ModelName + "Service"; }
        }

        #endregion

        #region Domain

        public override string DomainNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Domain.Repository"
                           : string.Format(@"DZ.Domain.Repository.{0}", SubNamespace);
            }
        }

        public override string DomainName
        {
            get { return "I" + ModelName + "Repos"; }
        }

        #endregion
    }

    public class CodeNameOfServiceLayer : CodeNameBase
    {
        protected string ModelName;
        protected string SubNamespace;

        public CodeNameOfServiceLayer(string subNamespace, string modelName)
        {
            SubNamespace = subNamespace;
            var sb = GetModelName(modelName);
            ModelName = sb;
        }

        #region Service Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string ServiceNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Services"
                           : string.Format("DZ.Services.{0}", SubNamespace);
            }
        }

        public override string ServiceName
        {
            get { return "I" + ModelName + "Service"; }
        }

        #endregion

        #region DTO Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string ServiceDTONamespace
        {
            get { return ServiceNamespace + ".DTO"; }
        }

        public override string ServiceDTOName
        {
            get { return ModelName + "DTO"; }
        }

        public override string ServiceQueryDTOName
        {
            get { return ModelName + "QueryDTO"; }
        }

        #endregion

        #region Dao Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string DaoNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Dao"
                           : string.Format("DZ.Dao.{0}", SubNamespace);
            }
        }

        public override string DaoName
        {
            get { return ModelName + "Dao"; }
        }

        #endregion

        #region ServiceImpl

        public override string ServiceImplNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Services.Impl"
                           : string.Format(@"DZ.Services.Impl.{0}", SubNamespace);
            }
        }

        public override string ServiceImplName
        {
            get { return ModelName + "Service"; }
        }

        #endregion

        #region Domain

        public override string DomainNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Domain.Repository"
                           : string.Format(@"DZ.Domain.Repository.{0}", SubNamespace);
            }
        }

        public override string DomainName
        {
            get { return "I" + ModelName + "Repos"; }
        }

        #endregion
    }

    public class CodeNameOfServiceThreeLayer : CodeNameBase
    {
        protected string ModelName;
        protected string SubNamespace;

        public CodeNameOfServiceThreeLayer(string subNamespace, string modelName)
        {
            SubNamespace = subNamespace;
            var sb = GetModelName(modelName);
            ModelName = sb;
        }

        #region Service Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string ServiceNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Services"
                           : string.Format("DZ.Services.{0}", SubNamespace);
            }
        }

        public override string ServiceName
        {
            get { return ModelName + "Service"; }
        }

        #endregion

        #region DTO Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string ServiceDTONamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Services.DTO"
                           : string.Format("DZ.Services.DTO.{0}", SubNamespace);
            }
        }

        public override string ServiceDTOName
        {
            get { return ModelName + "DTO"; }
        }

        public override string ServiceQueryDTOName
        {
            get { return ModelName + "QueryDTO"; }
        }
        #endregion

        #region Dao Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string DaoNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Dao"
                           : string.Format("DZ.Dao.{0}", SubNamespace);
            }
        }

        public override string DaoName
        {
            get { return ModelName + "Dao"; }
        }

        #endregion

        public override string ServiceImplNamespace
        {
            get { return string.Empty; }
        }

        public override string ServiceImplName
        {
            get { return string.Empty; }
        }

        public override string DomainNamespace
        {
            get { return string.Empty; }
        }

        public override string DomainName
        {
            get { return string.Empty; }
        }
    }

    public class CodeNameOfThreeLayer : CodeNameBase
    {
        protected string ModelName;
        protected string SubNamespace;

        public CodeNameOfThreeLayer(string subNamespace, string modelName)
        {
            SubNamespace = subNamespace;
            var sb = GetModelName(modelName);
            ModelName = sb;
        }

        #region BLL(Service) Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string ServiceNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.BLL"
                           : string.Format("DZ.BLL.{0}", SubNamespace);
            }
        }

        public override string ServiceName
        {
            get { return ModelName + "BLL"; }
        }

        #endregion

        #region Model(DTO) Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string ServiceDTONamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.Model"
                           : string.Format("DZ.Model.{0}", SubNamespace);
            }
        }

        public override string ServiceDTOName
        {
            get { return ModelName + "Info"; }
        }

        public override string ServiceQueryDTOName
        {
            get { return ModelName + "QueryInfo"; }
        }
        #endregion

        #region DAL (Dao) Name

        /// <summary>
        /// Service命名空间
        /// </summary>
        public override string DaoNamespace
        {
            get
            {
                return string.IsNullOrEmpty(SubNamespace)
                           ? @"DZ.DAL"
                           : string.Format("DZ.DAL.{0}", SubNamespace);
            }
        }

        public override string DaoName
        {
            get { return ModelName + "DAL"; }
        }

        #endregion

        public override string ServiceImplNamespace
        {
            get { return string.Empty; }
        }

        public override string ServiceImplName
        {
            get { return string.Empty; }
        }

        public override string DomainNamespace
        {
            get { return string.Empty; }
        }

        public override string DomainName
        {
            get { return string.Empty; }
        }
    }
}