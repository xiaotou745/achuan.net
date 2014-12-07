using System;
using System.Text;
using AC.Code.Helper;

namespace AC.Code.Config
{
    public class CodeNameFactory
    {
        private CodeLayer codeLayer;

        private CodeNameFactory()
        {
        }

        public static CodeNameFactory Create(CodeLayer codeLayer)
        {
            return new CodeNameFactory {codeLayer = codeLayer};
        }

        public CodeNameBase GetCodeName(string subNamespace, string modelName)
        {
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