using System;
using System.Globalization;
using AC.Code.DbObjects;

namespace AC.Code.Config
{
    /// <summary>
    /// 代码方法配置
    /// </summary>
    public class CodeGenerateConfig
    {
        private DbType dbType = DbType.SQL2005;

        #region 方法选择

        private bool createMethodNeeded = true;
        private bool getByIdMethodNeeded = true;
        private bool modifyMethodNeeded = true;
        private bool queryMethodNeeded = true;
        private bool removeMethodNeeded = true;

        /// <summary>
        /// 是否生成Create Method
        /// </summary>
        public bool CreateMethodNeeded
        {
            get { return createMethodNeeded; }
            set { createMethodNeeded = value; }
        }

        /// <summary>
        /// 是否生成Modify Method
        /// </summary>
        public bool ModifyMethodNeeded
        {
            get { return modifyMethodNeeded; }
            set { modifyMethodNeeded = value; }
        }

        /// <summary>
        /// 是否生成Remove Method
        /// </summary>
        public bool RemoveMethodNeeded
        {
            get { return removeMethodNeeded; }
            set { removeMethodNeeded = value; }
        }

        /// <summary>
        /// 是否生成GetById Method
        /// </summary>
        public bool GetByIdMethodNeeded
        {
            get { return getByIdMethodNeeded; }
            set { getByIdMethodNeeded = value; }
        }

        /// <summary>
        /// 是否生成Query Method
        /// </summary>
        public bool QueryMethodNeeded
        {
            get { return queryMethodNeeded; }
            set { queryMethodNeeded = value; }
        }

        #endregion

        #region CallStyle

        private string callStyleHashCode;

        public string CallStyleHashCode
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = 1.ToString(CultureInfo.InvariantCulture);
                }
                callStyleHashCode = value;
                CallStyle = (CallStyle) Enum.Parse(typeof (CallStyle), value);
            }
            get { return callStyleHashCode; }
        }

        /// <summary>
        /// 调用方式
        /// </summary>
        public CallStyle CallStyle { get; set; }

        #endregion

        #region CodeLayer

        private string codeLayerHashCode;

        public string CodeLayerHashCode
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = 1.ToString();
                }
                codeLayerHashCode = value;
                CodeLayer = (CodeLayer) Enum.Parse(typeof (CodeLayer), value);
            }
            get { return codeLayerHashCode; }
        }

        /// <summary>
        /// 代码分层
        /// </summary>
        public CodeLayer CodeLayer { get; set; }

        #endregion

        #region DaoStyle

        private string daoStyleHashCode;

        public string DaoStyleHashCode
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = 1.ToString();
                }
                daoStyleHashCode = value;
                DaoStyle = (DaoStyle) Enum.Parse(typeof (DaoStyle), value);
            }
            get { return daoStyleHashCode; }
        }

        /// <summary>
        /// Dao层实现方式
        /// </summary>
        public DaoStyle DaoStyle { get; set; }

        #endregion

        /// <summary>
        /// 数据库类型，默认是SQL2005
        /// </summary>
        public DbType DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }

        public CodeNameBase CodeName
        {
            get { return CodeNameFactory.Create(CodeLayer).GetCodeName(SubNamespace, ModelName); }
        }

        #region 类名 命名空间名

        /// <summary>
        /// 默认取表名，DTO类名
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 子命名空间
        /// </summary>
        public string SubNamespace { get; set; }

        #endregion
    }
}